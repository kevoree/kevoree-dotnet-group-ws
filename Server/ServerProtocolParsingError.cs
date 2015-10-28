using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Kevoree.Library.Server
{
    class ServerProtocolParsingError : Exception
    {

        public ServerProtocolParsingError(string p)
            : base(p)
        {
        }
    }
}
