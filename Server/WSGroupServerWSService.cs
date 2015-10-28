using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.Kevoree.Library.Server;
using Org.Kevoree.Library.Util;
using WebSocketSharp;
using WebSocketSharp.Server;
using Org.Kevoree.Library.Protocol;
using Org.Kevoree.Core.Marshalled;
using org.kevoree.pmodeling.api.json;
using org.kevoree.factory;
using org.kevoree;

namespace Org.Kevoree.Library.Server
{
    class WSGroupServerWSService : WebSocketBehavior
    {

        private ServerProtocolParser spp = new ServerProtocolParser();
        private readonly JSONModelLoader _loader = new JSONModelLoader(new DefaultKevoreeFactory());

        protected override void OnMessage(MessageEventArgs e)
        {
            WSGroupServices.GetLogger().Info("Message received");
            try
            {
                var message = spp.parse(e.Data);
                if (message is Pull)
                {
                    PullHandler();
                }
                else if (message is Push)
                {
                    PushHandler((Push) message);
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
            throw new NotImplementedException();
        }

        private void PushHandler(Push pushMessage)
        {
            var model = pushMessage.getModel();
            if (!String.IsNullOrEmpty(model))
            {
                var models = _loader.loadModelFromString(model);
                if (models != null && model.Length > 0)
                {
                    WSGroupServices.GetModelService().update(new ContainerRootMarshalled((ContainerRoot)models.get(0)), null);
                }
                else
                {
                    WSGroupServices.GetLogger().Warn(string.Format("\"{0}\" received model is empty, push aborted",
                        WSGroupServices.GetContext().getInstanceName()));
                }
            }
            else
            {
                WSGroupServices.GetLogger().Warn(string.Format("\"{0}\" push message does not contain a model, push aborted", WSGroupServices.GetContext().getInstanceName()));
            }
        }

        private void PullHandler()
        {
            Send(WSGroupServices.GetModelService().getCurrentModel().getModel().serialize()); 
        }

        protected override void OnClose(CloseEventArgs e)
        {
            WSGroupServices.GetLogger().Info("Socket closed");
        }

        protected override void OnOpen()
        {
            WSGroupServices.GetLogger().Info("Socket opened");
        }
    }
}
