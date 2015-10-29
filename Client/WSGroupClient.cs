using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ikvm.extensions;
using Org.Kevoree.Core.Marshalled;
using Org.Kevoree.Library.Protocol;
using Org.Kevoree.Library.Util;
using WebSocketSharp;
using org.kevoree;
using Org.Kevoree.Core.Api.IMarshalled;
using System.IO;

namespace Org.Kevoree.Library.Client
{
    class WSGroupClient
    {
        private WSGroup wSGroup;

        public WSGroupClient(WSGroup wSGroup)
        {
            this.wSGroup = wSGroup;
        }

        internal void Start()
        {
            StartGroupWebsocket();

            while (!wSGroup.GetStop())
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
                this.wSGroup.GetLogger().Debug("Try connection to " + host + ":" + port);
                try
                {
                    WebSocket ws = new WebSocket(ForgeUrl(host, port));
                    ws.OnOpen += (sender, args) =>
                    {
                        var nodeName = WSGroupServices.GetContext().getNodeName();
                        var currentModel = WSGroupServices.GetModelService().getPendingModel().serialize();
                        this.wSGroup.GetLogger().Debug("Send Register message\n" + currentModel);
                        ws.SendAsync(new Register(nodeName, currentModel).Serialize(), (a) => { this.wSGroup.GetLogger().Debug("Register completed"); });
                        this.wSGroup.GetLogger().Debug("Register message send");
                    };

                    ws.OnMessage += (sender, e) =>
                    {
                        // Handle push messages only  
                        this.wSGroup.GetLogger().Debug("OnMessage : " + sender + " " + e.Data);
                        try
                        {
                            var message = new ClientProtocolParser().parse(e.Data);
                            if (message is Push)
                            {
                                var pushMessage = (Push) message;
                                ModelUtil.UpdateModelLocaly(pushMessage.getModel());
                            }
                            else
                            {
                                this.wSGroup.GetLogger().Debug("Message not implemented " + message);
                            }
                        }
                        catch (ClientProtocolParsingError)
                        {
                            this.wSGroup.GetLogger().Error("Unknow message " + e.Data);
                        }
                    };

                    ws.OnClose += (sender, args) =>
                    {
                        this.wSGroup.GetLogger().Debug("Disconnected from master");
                    };

                    ws.Connect();

                    this.wSGroup.GetLogger().Debug("Connected to " + host + ":" + port);
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
            IFragmentDictionaryMarshalled masterDico = group.findFragmentDictionaryByID(wSGroup.GetMaster());
            string port;
            if (masterDico != null)
            {
                IValueMarshalled val = masterDico.findValuesByID("port");
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
            var masterNode = lastModel.findNodesByID(wSGroup.GetMaster());
            var networkInformations = masterNode.getNetworkInformation();
            return networkInformations.Select(e => e.getValues().Find(f => f.getName().equalsIgnoreCase("ip")).getValue());
        }
    }
}
