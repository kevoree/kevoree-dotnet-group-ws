using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.Kevoree.Core.Api;
using Org.Kevoree.Log.Api;

namespace Org.Kevoree.Library.Util
{
    class WSGroupServices
    {
        private static Func<ILogger> _logger;
        private static Func<ModelService> _modelService;
        private static Func<Context> _context;

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
    }
}
