using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ikvm.extensions;
using Org.Kevoree.Library.Protocol;
using Org.Kevoree.Library.Util;
using WebSocketSharp;

namespace Org.Kevoree.Library.Client
{
    class WSGroupClient
    {
        private readonly WSGroup _wSGroup;

        public WSGroupClient(WSGroup _wSGroup)
        {
            this._wSGroup = _wSGroup;
        }

        internal void Start()
        {
            StartGroupWebsocket();

            while (!_wSGroup.GetStop())
            {
                Thread.Sleep(1000);
            }
        }


        private void StartGroupWebsocket()
        {
            var hosts = ResolveMasterIps();
            var port = ResolveMasterPort();


            // try every potential IP by try and fail.
            // we keeps the first successfully connect websocket.
            foreach (var host in hosts)
            {
                _wSGroup.GetLogger().Debug("Try connection to " + host + ":" + port);
                try
                {
                    WebSocket ws = new WebSocket(ForgeUrl(host, port));
                    ws.OnOpen += (sender, args) =>
                    {
                        var nodeName = WSGroupServices.GetContext().getNodeName();
                        var currentModel = WSGroupServices.GetModelService().getPendingModel().serialize();
                        _wSGroup.GetLogger().Debug("Send Register message\n" + currentModel);
                        ws.SendAsync(new Register(nodeName, currentModel).Serialize(), a => { _wSGroup.GetLogger().Debug("Register completed"); });
                        _wSGroup.GetLogger().Debug("Register message send");
                    };

                    ws.OnMessage += (sender, e) =>
                    {
                        // Handle push messages only  
                        _wSGroup.GetLogger().Debug("OnMessage : " + sender + " " + e.Data);
                        try
                        {
                            var message = new ClientProtocolParser().Parse(e.Data);
                            if (message is Push)
                            {
                                var pushMessage = (Push)message;
                                ModelUtil.UpdateModelLocaly(pushMessage.GetModel());
                            }
                            else
                            {
                                _wSGroup.GetLogger().Debug("Message not implemented " + message);
                            }
                        }
                        catch (ClientProtocolParsingError)
                        {
                            _wSGroup.GetLogger().Error("Unknow message " + e.Data);
                        }
                    };

                    ws.OnClose += (sender, args) =>
                    {
                        _wSGroup.GetLogger().Debug("Disconnected from master");
                    };

                    ws.Connect();

                    _wSGroup.GetLogger().Debug("Connected to " + host + ":" + port);
                    break;
                }
                catch (Exception)
                {

                }
            }

        }



        private string ForgeUrl(string host, string port)
        {
            if (host.Contains(":"))
            {
                return "ws://[" + host + "]:" + port;
            }
            else
            {
                return "ws://" + host + ":" + port;
            }
        }

        private string ResolveMasterPort()
        {
            var lastModel = WSGroupServices.GetModelService().getPendingModel();
            var group = lastModel.findByPath(WSGroupServices.GetContext().getPath()).CastToGroup();
            var masterDico = group.findFragmentDictionaryByID(_wSGroup.GetMaster());
            string port;
            if (masterDico != null)
            {
                var val = masterDico.findValuesByID("port");
                port = val.getValue();
            }
            else
            {
                port = "9000";
            }
            return port;
        }

        /**
         * Return a list of potential IP of the master.
         */
        private IEnumerable<string> ResolveMasterIps()
        {
            var lastModel = WSGroupServices.GetModelService().getPendingModel();
            var masterNode = lastModel.findNodesByID(_wSGroup.GetMaster());
            var networkInformations = masterNode.getNetworkInformation();

            return (from network in networkInformations
                    from networkValue in network.getValues()
                    where networkValue.getName().equalsIgnoreCase("ip") || network.getName().equalsIgnoreCase("ip")
                    select networkValue.getValue()).ToList();
        }
    }
}
