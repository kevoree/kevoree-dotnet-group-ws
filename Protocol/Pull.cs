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
