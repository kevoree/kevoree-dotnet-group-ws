using System.Threading;
using WebSocketSharp.Server;

namespace Org.Kevoree.Library.Server
{
    class WSGroupServer
    {
        
        private readonly WSGroup _wSGroup;

        public WSGroupServer(WSGroup _wSGroup)
        {
            this._wSGroup = _wSGroup;
        }

        internal void Start()
        {
            WebSocketServer masterServer = new WebSocketServer("ws://127.0.0.1:" + _wSGroup.GetPort());
            masterServer.AddWebSocketService<WSGroupServerWSService>("/");
            masterServer.Start();
            _wSGroup.GetLogger().Debug("Server started");

            while (!_wSGroup.GetStop())
            {
                Thread.Sleep(1000);
            }

            masterServer.Stop();
        }
    }
}
