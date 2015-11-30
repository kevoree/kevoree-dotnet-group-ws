namespace Org.Kevoree.Library.Protocol
{
    public class Register : Message
    {
        private readonly string _nodeName;
        private readonly string _model;

        public Register(string nodeName, string model)
        {
            _nodeName = nodeName;
            _model = model;
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
