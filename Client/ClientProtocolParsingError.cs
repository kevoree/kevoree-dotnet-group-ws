using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Kevoree.Library.Client
{
    class ClientProtocolParsingError : Exception
    {
        public ClientProtocolParsingError(string p)
            : base(p)
        {
        }
    }
}
