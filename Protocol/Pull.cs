using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Org.Kevoree.Library.Protocol
{
    public class Pull : Message
    {
        public override string Serialize()
        {
            return "pull";
        }
    }
}
