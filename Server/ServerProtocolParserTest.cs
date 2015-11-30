using NUnit.Framework;
using Org.Kevoree.Library.Protocol;

namespace Org.Kevoree.Library.Server
{
    [TestFixture]
    class ServerProtocolParserTest
    {

        private ServerProtocolParser cpp = new ServerProtocolParser();

        [Test]
        public void TestEmptyMessage()
        {
            Assert.Throws(typeof(ServerProtocolParsingError), () => cpp.Parse("  "));
        }

        [Test]
        public void TestWrongMessageMessage()
        {
            Assert.Throws(typeof(ServerProtocolParsingError), () => cpp.Parse("push"));
        }

        [Test]
        public void TestGoodMessagePushMessage()
        {
            var res = cpp.Parse("push/{}");
            Assert.IsTrue(res is Push);
            Assert.AreEqual("{}", ((Push)res).GetModel());
        }

        [Test]
        public void TestGoodMessagePullMessage()
        {
            var res = cpp.Parse("pull");
            Assert.IsTrue(res is Pull);
        }

        [Test]
        public void TestGoodMessageRegisterMessage()
        {
            var res = cpp.Parse("register/a/b");
            Assert.IsTrue(res is Register);
            Assert.AreEqual("a", ((Register)res).GetNodeName());
            Assert.AreEqual("b", ((Register)res).GetModel());
        }
    }
}
