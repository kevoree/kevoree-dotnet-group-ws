using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.Kevoree.Annotation;
using Org.Kevoree.Core.Api;
using Org.Kevoree.Library.Util;
using Org.Kevoree.Log.Api;
using WebSocketSharp.Server;
using System.Threading;
using Org.Kevoree.Library.Server;

namespace Org.Kevoree.Library
{
    [GroupType]
    [Serializable]
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

        private bool _stop = false;

        [Start]
        public void Start()
        {
            WSGroupServices.RegisterLogger(GetLogger);
            WSGroupServices.RegisterModelService(GetModelService);
            WSGroupServices.RegisterContext(GetContext);
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
            this._stop = true;
        }

        private void StartClient()
        {
            throw new NotImplementedException();
        }

        private void StartServer()
        {
            new Thread(new ThreadStart(new WSGroupServer(this).Start)).Start();
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
            return this._logger;
        }

        internal int GetPort()
        {
            return this.port;
        }

        internal bool GetStop()
        {
            return this._stop;
        }
    }
}
