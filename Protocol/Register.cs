using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Org.Kevoree.Library.Protocol
{
    public class Register : Message
    {
        private readonly string _nodeName;
        private readonly string _model;

        public Register(string nodeName, string model)
        {
            this._nodeName = nodeName;
            this._model = model;
        }

        public string GetNodeName()
        {
            return _nodeName;
        }

        public string GetModel()
        {
            return _model;
        }

        public override string Serialize()
        {
            return "register/" + _nodeName + "/" + _model;
        }
    }
}
