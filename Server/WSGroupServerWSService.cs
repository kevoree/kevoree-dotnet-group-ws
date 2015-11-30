using System;
using System.Collections.Generic;
using org.kevoree;
using org.kevoree.factory;
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
                WSGroupServices.GetLogger().Error("Message parsing error");
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
            try
            {
                WSGroupServices.GetKevScriptEngine()
                    .execute(WSGroupServices.GetTemplateEngine().Process(WSGroupServices.GetOnConnect(), context), modelToApply);
            }
            catch (Exception)
            {
                WSGroupServices.GetLogger()
                    .Error("Unable to parse onConnect KevScript. Broadcasting model without onConnect process.");
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
                if (!client.Value.IsAlive)
                {
                    Dictionary<string, string> context = new Dictionary<string, string>();
                    context.Add("nodeName", client.Key);
                    var kevscript = WSGroupServices.GetTemplateEngine()
                        .Process(WSGroupServices.GetOnDisconnect(), context);
                    UpdateCallback cb = applied => WSGroupServices.GetLogger().Info("onDisconnect result from " + client.Key + " : " + applied);
                    try
                    {
                        WSGroupServices.GetModelService().submitScript(kevscript, cb);
                    }
                    catch (Exception)
                    {
                        WSGroupServices.GetLogger().Error("Unable to parse onDisconnect KevScript. No changes made after the disconnection of " + client.Key);

                    }
                }
            }

        }

        protected override void OnOpen()
        {
            WSGroupServices.GetLogger().Info("Socket opened");
        }
    }
}
