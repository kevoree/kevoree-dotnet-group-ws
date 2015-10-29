using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Org.Kevoree.Library.Protocol
{
    public class Push: Message
    {
        private readonly string _model;

        public Push(string model)
        {
            this._model = model;
        }

        public string getModel()
        {
            return this._model;
        }

        public override string Serialize()
        {
            return "push/" + _model;
        }
    }
}
