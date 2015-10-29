using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.Kevoree.Core.Api;
using Org.Kevoree.Log.Api;
using org.kevoree.factory;
using org.kevoree.kevscript;
using org.kevoree.pmodeling.api;
using org.kevoree.pmodeling.api.compare;
using org.kevoree.pmodeling.api.json;

namespace Org.Kevoree.Library.Util
{
    class WSGroupServices
    {
        private static Func<ILogger> _logger;
        private static Func<ModelService> _modelService;
        private static Func<Context> _context;

        private static readonly KevoreeFactory factory = new DefaultKevoreeFactory();

        private static readonly ModelCloner cloner = factory.createModelCloner();

        private static readonly JSONModelLoader jsonModelLoader = new JSONModelLoader(new DefaultKevoreeFactory());

        private static readonly ModelCompare compare = new ModelCompare(new DefaultKevoreeFactory());
        private static Func<org.kevoree.kevscript.KevScriptEngine> _kevScriptEngine;
        private static Func<string> _onConnect;
        private static Func<string> _onDisconnect;
        private static TemplateEngine TemplateEngine = new TemplateEngine();

        public static void RegisterLogger(Func<ILogger> logger)
        {
            _logger = logger;
        }

        public static void RegisterModelService(Func<ModelService> modelService)
        {
            _modelService = modelService;
        }

        public static ILogger GetLogger()
        {
            return _logger();
        }

        internal static ModelService GetModelService()
        {
            return _modelService();
        }

        internal static Context GetContext()
        {
            return _context();
        }

        internal static void RegisterContext(Func<Context> GetContext)
        {
            _context = GetContext;
        }

        internal static ModelCloner GetCloner()
        {
            return cloner;
        }

        internal static JSONModelLoader GetJsonModelLoader()
        {
            return jsonModelLoader;
        }

        internal static ModelCompare GetModelCompare()
        {
            return compare;
        }

        internal static void RegisterKevScriptEngine(Func<org.kevoree.kevscript.KevScriptEngine> GetKevScriptEngine)
        {
            _kevScriptEngine = GetKevScriptEngine;
        }

        internal static KevScriptEngine GetKevScriptEngine()
        {
            return _kevScriptEngine();
        }

        internal static string GetOnConnect()
        {
            return _onConnect();
        }


        internal static string GetOnDisconnect()
        {
            return _onDisconnect();
        }

        internal static void RegisterOnConnect(Func<string> GetOnConnect)
        {
            _onConnect = GetOnConnect;
        }

        internal static void RegisterOnDisconnect(Func<string> GetOnDisconnect)
        {
            _onDisconnect = GetOnDisconnect;
        }

        internal static TemplateEngine GetTemplateEngine()
        {
            return TemplateEngine;
        }
    }
}
