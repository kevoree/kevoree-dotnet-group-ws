using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using WebSocketSharp.Server;

namespace Org.Kevoree.Library.Server
{
    class WSGroupServer
    {
        
        private readonly WSGroup wSGroup;

        public WSGroupServer(WSGroup wSGroup)
        {
            this.wSGroup = wSGroup;
        }

        internal void Start()
        {
            WebSocketServer masterServer = new WebSocketServer("ws://127.0.0.1:" + wSGroup.GetPort());
            masterServer.AddWebSocketService<WSGroupServerWSService>("/");
            masterServer.Start();
            wSGroup.GetLogger().Debug("Server started");

            while (!wSGroup.GetStop())
            {
                Thread.Sleep(1000);
            }
        }
    }
}
