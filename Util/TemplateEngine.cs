﻿using System.Collections.Generic;

namespace Org.Kevoree.Library.Util
{
    class TemplateEngine
    {
        public string Process(string template, Dictionary<string, string> env)
        {
            string tmpl = template;
            foreach(var it in env)
            {
                tmpl = tmpl.Replace("{{" + it.Key + "}}", it.Value);
            }
            return tmpl;
        }
    }
}
