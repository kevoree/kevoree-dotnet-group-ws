using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Org.Kevoree.Library.Protocol
{
    public class Register : Message
    {
        private readonly string _group;
        private readonly string _model;

        public Register(string group, string model)
        {
            this._group = group;
            this._model = model;
        }

        public string GetGroup()
        {
            return _group;
        }

        public string GetModel()
        {
            return _model;
        }
    }
}
