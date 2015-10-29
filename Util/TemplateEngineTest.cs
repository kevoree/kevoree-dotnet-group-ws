using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Org.Kevoree.Library.Util
{
     [TestFixture]
    class TemplateEngineTest
    {

         [Test]
         public void SimpleTest()
         {
             string template = "{{key1}}=key1, {test42}} = pasreplace";

             Dictionary<string, string> env = new Dictionary<string, string>();
             env.Add("key1", "val1");
             env.Add("key2", "val2");

             var res = new TemplateEngine().Process(template, env);

             Assert.AreEqual("val1=key1, {test42}} = pasreplace", res);
         }
    }
}
