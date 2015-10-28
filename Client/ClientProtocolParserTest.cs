using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.Kevoree.Library.Protocol;

namespace Org.Kevoree.Library.Client
{
    [TestFixture]
    class ClientProtocolParserTest
    {

        private ClientProtocolParser cpp = new ClientProtocolParser();

        [Test]
        public void TestEmptyMessage()
        {
            Assert.Throws(typeof(ClientProtocolParsingError), new TestDelegate(() => cpp.parse("  ")));
        }

        [Test]
        public void TestWrongMessageMessage()
        {
            Assert.Throws(typeof(ClientProtocolParsingError), new TestDelegate(() => cpp.parse("push")));
        }

        [Test]
        public void TestGoodMessageMessage()
        {
            var res = cpp.parse("push/{}");
            Assert.IsTrue(res is Push);
            Assert.AreEqual("{}", ((Push)res).getModel());
        }
    }
}
