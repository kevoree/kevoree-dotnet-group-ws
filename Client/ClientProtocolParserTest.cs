using NUnit.Framework;
using Org.Kevoree.Library.Protocol;

namespace Org.Kevoree.Library.Client
{
    [TestFixture]
    class ClientProtocolParserTest
    {

        private readonly ClientProtocolParser _cpp = new ClientProtocolParser();

        [Test]
        public void TestEmptyMessage()
        {
            Assert.Throws(typeof(ClientProtocolParsingError), () => _cpp.Parse("  "));
        }

        [Test]
        public void TestWrongMessageMessage()
        {
            Assert.Throws(typeof(ClientProtocolParsingError), () => _cpp.Parse("push"));
        }

        [Test]
        public void TestGoodMessageMessage()
        {
            var res = _cpp.Parse("push/{}");
            Assert.IsTrue(res is Push);
            Assert.AreEqual("{}", ((Push)res).GetModel());
        }
    }
}
