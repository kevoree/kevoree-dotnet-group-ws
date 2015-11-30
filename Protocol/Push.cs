namespace Org.Kevoree.Library.Protocol
{
    public class Push: Message
    {
        private readonly string _model;

        public Push(string model)
        {
            _model = model;
        }

        public string GetModel()
        {
            return _model;
        }

        public override string Serialize()
        {
            return "push/" + _model;
        }
    }
}
