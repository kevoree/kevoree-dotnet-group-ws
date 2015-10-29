using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Assert.Throws(typeof(ServerProtocolParsingError), new TestDelegate(() => cpp.parse("  ")));
        }

        [Test]
        public void TestWrongMessageMessage()
        {
            Assert.Throws(typeof(ServerProtocolParsingError), new TestDelegate(() => cpp.parse("push")));
        }

        [Test]
        public void TestGoodMessagePushMessage()
        {
            var res = cpp.parse("push/{}");
            Assert.IsTrue(res is Push);
            Assert.AreEqual("{}", ((Push)res).getModel());
        }

        [Test]
        public void TestGoodMessagePullMessage()
        {
            var res = cpp.parse("pull");
            Assert.IsTrue(res is Pull);
        }

        [Test]
        public void TestGoodMessageRegisterMessage()
        {
            var res = cpp.parse("register/a/b");
            Assert.IsTrue(res is Register);
            Assert.AreEqual("a", ((Register)res).GetNodeName());
            Assert.AreEqual("b", ((Register)res).GetModel());
        }
    }
}
