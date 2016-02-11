using System;
using org.kevoree.factory;
using org.kevoree.kevscript;
using org.kevoree.pmodeling.api;
using org.kevoree.pmodeling.api.compare;
using org.kevoree.pmodeling.api.json;
using Org.Kevoree.Core.Api;
using Org.Kevoree.Log.Api;

namespace Org.Kevoree.Library.Util
{
    class WSGroupServices
    {
        private static Func<ILogger> _logger;
        private static Func<ModelService> _modelService;
        private static Func<Context> _context;

        private static readonly KevoreeFactory Factory = new DefaultKevoreeFactory();

        private static readonly ModelCloner Cloner = Factory.createModelCloner();

        private static readonly JSONModelLoader JsonModelLoader = new JSONModelLoader(new DefaultKevoreeFactory());

        private static readonly ModelCompare Compare = new ModelCompare(new DefaultKevoreeFactory());
        private static Func<KevScriptEngine> _kevScriptEngine;
        private static Func<string> _onConnect;
        private static Func<string> _onDisconnect;
        private static Func<string> _filter;
        private static readonly TemplateEngine TemplateEngine = new TemplateEngine();

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

        internal static void RegisterContext(Func<Context> getContext)
        {
            _context = getContext;
        }

        internal static ModelCloner GetCloner()
        {
            return Cloner;
        }

        internal static JSONModelLoader GetJsonModelLoader()
        {
            return JsonModelLoader;
        }

        internal static ModelCompare GetModelCompare()
        {
            return Compare;
        }

        internal static void RegisterKevScriptEngine(Func<KevScriptEngine> getKevScriptEngine)
        {
            _kevScriptEngine = getKevScriptEngine;
        }

        internal static KevScriptEngine GetKevScriptEngine()
        {
            return _kevScriptEngine();
        }

        internal static string GetOnConnect()
        {
            return _onConnect();
        }

        internal static string GetFilter()
        {
            return _filter();
        }


        internal static string GetOnDisconnect()
        {
            return _onDisconnect();
        }

        internal static void RegisterOnConnect(Func<string> getOnConnect)
        {
            _onConnect = getOnConnect;
        }

        internal static void RegisterFilter(Func<string> getFilter)
        {
            _filter = getFilter;
        }

        internal static void RegisterOnDisconnect(Func<string> getOnDisconnect)
        {
            _onDisconnect = getOnDisconnect;
        }

        internal static TemplateEngine GetTemplateEngine()
        {
            return TemplateEngine;
        }
    }
}
