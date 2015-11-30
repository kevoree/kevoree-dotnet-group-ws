using System;
using System.ComponentModel.Composition;
using System.Threading;
using org.kevoree.kevscript;
using Org.Kevoree.Annotation;
using Org.Kevoree.Core.Api;
using Org.Kevoree.Library.Client;
using Org.Kevoree.Library.Server;
using Org.Kevoree.Library.Util;
using Org.Kevoree.Log.Api;

namespace Org.Kevoree.Library
{
    [GroupType]
    [Serializable]
    [Export(typeof(DeployUnit))]
    public class WSGroup : MarshalByRefObject, DeployUnit
    {
        [KevoreeInject]
        private Context _context;

        [Param(Optional = true)]
        private string master;

        [KevoreeInject]
        public ModelService _modelService;

        [Param(DefaultValue = "")]
        private string onConnect = "";

        [Param(DefaultValue = "")]
        private string onDisconnect = "";

        [Param(Optional = true, FragmentDependent = true, DefaultValue = "9000")]
        private int port = 9000;

        [KevoreeInject]
        private ILogger _logger;

        private bool _stop;


        [KevoreeInject] private KevScriptEngine _kevScriptEngine;

        [Start]
        public void Start()
        {
            WSGroupServices.RegisterLogger(GetLogger);
            WSGroupServices.RegisterModelService(GetModelService);
            WSGroupServices.RegisterContext(GetContext);
            WSGroupServices.RegisterKevScriptEngine(GetKevScriptEngine);
            WSGroupServices.RegisterOnConnect(GetOnConnect);
            WSGroupServices.RegisterOnDisconnect(GetOnDisconnect);
            if (HasMaster())
            {
                if (IsMaster())
                {
                    StartServer();
                }
                else
                {
                    StartClient();
                }
            }
            else
            {
                _logger.Debug("No master designated");

                StartServer();
            }
            _logger.Debug("WSGroup stopped");
        }


        private Context GetContext()
        {
            return _context;
        }

        private ModelService GetModelService()
        {
            return _modelService;
        }


        [Stop]
        public void Stop()
        {
            _stop = true;
        }

        private void StartClient()
        {
            _logger.Debug("Start WSChan as client ");
            new Thread(new WSGroupClient(this).Start).Start();
        }

        private void StartServer()
        {
            new Thread(new WSGroupServer(this).Start).Start();
        }

        private bool HasMaster()
        {
            return !String.IsNullOrEmpty(master);
        }

        private bool IsMaster()
        {
            return HasMaster() && String.Equals(_context.getNodeName(), master);
        }

        internal ILogger GetLogger()
        {
            return _logger;
        }

        internal KevScriptEngine GetKevScriptEngine()
        {
            return _kevScriptEngine;
        }

        internal int GetPort()
        {
            return port;
        }

        internal string GetMaster()
        {
            return master;
        }

        internal bool GetStop()
        {
            return _stop;
        }

        internal string GetOnConnect()
        {
            return onConnect;
        }

        internal string GetOnDisconnect()
        {
            return onDisconnect;
        }
    }
}
