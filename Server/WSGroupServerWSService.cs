using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using org.kevoree;
using org.kevoree.factory;
using org.kevoree.kevscript;
using org.kevoree.pmodeling.api.trace;
using Org.Kevoree.Core.Api;
using Org.Kevoree.Core.Marshalled;
using Org.Kevoree.Library.Protocol;
using Org.Kevoree.Library.Util;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Org.Kevoree.Library.Server
{
    class WSGroupServerWSService : WebSocketBehavior
    {

        private ServerProtocolParser spp = new ServerProtocolParser();
        private readonly Dictionary<string, WebSocket> _clients = new Dictionary<string, WebSocket>();


        protected override void OnMessage(MessageEventArgs e)
        {
            WSGroupServices.GetLogger().Info("Message received");
            try
            {
                var message = spp.Parse(e.Data);
                if (message is Pull)
                {
                    PullHandler();
                }
                else if (message is Push)
                {
                    PushHandler((Push)message);
                }
                else if (message is Register)
                {
                    RegisterHandler((Register)message);
                }
                else { WSGroupServices.GetLogger().Error("Unhandled message type"); }
            }
            catch (ServerProtocolParsingError)
            {
                WSGroupServices.GetLogger().Error("Message parsing error:\n" + e.Data.Substring(0, 200));
            }
        }

        private void RegisterHandler(Register register)
        {
            var client = Context.WebSocket;
            _clients.Add(register.GetNodeName(), client);
            var currentModel = WSGroupServices.GetModelService().getCurrentModel().getModel();


            var kf = new DefaultKevoreeFactory();
            // we clone the marshalled instance into a proper one
            var modelToApply = (ContainerRoot)kf.createJSONLoader().loadModelFromString(currentModel.serialize()).get(0);

            // We merge received model with current one
            ContainerRoot recModel = (ContainerRoot)WSGroupServices.GetJsonModelLoader()
                        .loadModelFromString(register.GetModel()).get(0);

            TraceSequence tseq = WSGroupServices.GetModelCompare().merge(modelToApply, recModel);
            tseq.applyOn(modelToApply);

            Dictionary<string, string> context = new Dictionary<string, string>();
            context.Add("nodeName", register.GetNodeName());
            context.Add("groupName", WSGroupServices.GetContext().getInstanceName());
            if (DoesFilterMatchIncomingNodeName(register.GetNodeName(), WSGroupServices.GetFilter()))
            {
                ApplyOnConnect(context, modelToApply);
            }
            else
            {
                var push = new Push(new ContainerRootMarshalled(modelToApply).serialize());
                _clients[register.GetNodeName()].Send(push.Serialize());
            }
        }

        private void ApplyOnConnect(Dictionary<string, string> context, ContainerRoot modelToApply)
        {
            var onConnectKevScript = WSGroupServices.GetOnConnect();
            if (!onConnectKevScript.IsNullOrEmpty())
            {
                try
                {
                    var kevScriptEngine = WSGroupServices.GetKevScriptEngine();
                    var templateEngine = WSGroupServices.GetTemplateEngine();
                    var process = templateEngine.Process(onConnectKevScript, context);
                    kevScriptEngine.execute(process, modelToApply);
                }
                catch (Exception e)
                {
                    WSGroupServices.GetLogger()
                        .Error("["+e.Message+"] - Unable to parse onConnect KevScript. Broadcasting model without onConnect process.\n" + onConnectKevScript );
                    WSGroupServices.GetLogger().Error(e.StackTrace);
                }
                finally
                {
                    var marshalled = new ContainerRootMarshalled(modelToApply);

                    // update locally
                    WSGroupServices.GetModelService().update(marshalled, null);

                    // broadcast changes
                    BroadcastToTheGroup(new Push(marshalled.serialize()));
                }
            }
        }

        /**
         *  Check if the filter match the node name. If the filter is empty or is not a valid regex expression the method return true (legacy compatibility).
         */
        private bool DoesFilterMatchIncomingNodeName(string nodeName, string filter)
        {
            bool ret;
            if (filter.IsNullOrEmpty())
            {
                ret = true;
            }
            else
            {
                try
                {
                    var regex = new Regex(filter);
                    var match = regex.Match(nodeName);
                    ret = match.Success;
                }
                catch (ArgumentException e)
                {
                    WSGroupServices.GetLogger().Error(e.Message);
                    ret = true;
                }
            }
            return ret;
        }

        private void PushHandler(Push pushMessage)
        {
            ModelUtil.UpdateModelLocaly(pushMessage.GetModel());
            BroadcastToTheGroup(pushMessage);
        }

        private void BroadcastToTheGroup(Message pushMessage)
        {
            foreach (var client in _clients)
            {
                client.Value.Send(pushMessage.Serialize());
            }
        }

        private void PullHandler()
        {
            Send(WSGroupServices.GetModelService().getCurrentModel().getModel().serialize());
        }

        protected override void OnClose(CloseEventArgs e)
        {
            WSGroupServices.GetLogger().Info("Socket closed");
            foreach (var client in _clients)
            {
                if (!client.Value.IsAlive && DoesFilterMatchIncomingNodeName(client.Key, WSGroupServices.GetFilter()))
                {
                    ApplyOnDisconnect(client);
                }
            }

        }

        private static void ApplyOnDisconnect(KeyValuePair<string, WebSocket> client)
        {
            Dictionary<string, string> context = new Dictionary<string, string>();
            context.Add("nodeName", client.Key);
            var onDisconnectKevScript = WSGroupServices.GetOnDisconnect();
            if (!onDisconnectKevScript.IsNullOrEmpty())
            {
                var kevscript = WSGroupServices.GetTemplateEngine()
                    .Process(onDisconnectKevScript, context);
                UpdateCallback cb =
                    applied =>
                        WSGroupServices.GetLogger()
                            .Info("onDisconnect result from " + client.Key + " : " + applied);
                try
                {
                    WSGroupServices.GetModelService().submitScript(kevscript, cb);
                }
                catch (Exception)
                {
                    WSGroupServices.GetLogger()
                        .Error(
                            "Unable to parse onDisconnect KevScript. No changes made after the disconnection of " +
                            client.Key);
                }
            }
        }

        protected override void OnOpen()
        {
            WSGroupServices.GetLogger().Info("Socket opened");
        }
    }
}
