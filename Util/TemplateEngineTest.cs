using NUnit.Framework;
using System.Collections.Generic;

namespace Org.Kevoree.Library.Util
{
     [TestFixture]
    class TemplateEngineTest
    {

         [Test]
         public void SimpleTest()
         {
             var template = "{{key1}}=key1, {test42}} = pasreplace";

             var env = new Dictionary<string, string>();
             env.Add("key1", "val1");
             env.Add("key2", "val2");

             var res = new TemplateEngine().Process(template, env);

             Assert.AreEqual("val1=key1, {test42}} = pasreplace", res);
         }
    }
}
