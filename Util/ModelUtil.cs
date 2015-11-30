using org.kevoree;
using org.kevoree.factory;
using org.kevoree.pmodeling.api.json;
using Org.Kevoree.Core.Marshalled;
using System;

namespace Org.Kevoree.Library.Util
{
    class ModelUtil
    {

        private static readonly JSONModelLoader Loader = new JSONModelLoader(new DefaultKevoreeFactory());

        public static void UpdateModelLocaly(string model)
        {
            if (!String.IsNullOrEmpty(model))
            {
                var models = Loader.loadModelFromString(model);
                if (models != null && model.Length > 0)
                {
                    WSGroupServices.GetModelService().update(new ContainerRootMarshalled((ContainerRoot)models.get(0)), null);
                }
                else
                {
                    WSGroupServices.GetLogger().Warn(string.Format("\"{0}\" received model is empty, push aborted",
                        WSGroupServices.GetContext().getInstanceName()));
                }
            }
            else
            {
                WSGroupServices.GetLogger().Warn(string.Format("\"{0}\" push message does not contain a model, push aborted", WSGroupServices.GetContext().getInstanceName()));
            }
        }
    }
}
