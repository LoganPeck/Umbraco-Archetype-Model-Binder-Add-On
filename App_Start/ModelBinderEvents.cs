using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using umbraco.interfaces;

namespace ScyllaPlugins.ArchetypeModelBuilder.App_Start
{
    public class ModelBinderEvents : ApplicationEventHandler
    {
        public ModelBinderEvents()
            : base()
        {
            DataTypeService.Saved += CreateStronglyTypedModel; //On data type saved.
        }

        protected void CreateStronglyTypedModel(IDataTypeService dts, SaveEventArgs<IDataTypeDefinition> e)
        {
            //Get the data type definition from the saved arguments.
            IDataTypeDefinition definition = e.SavedEntities.FirstOrDefault();

            if (definition == null) return;

            IEnumerable<string> prevalues = ApplicationContext.Current.DatabaseContext.Database.Fetch<string>("SELECT value FROM dbo.cmsDataTypePreValues WHERE datatypeNodeId = @0", definition.Id);
            //Get the prevalues.
            //IEnumerable<string> prevalues = dts.GetPreValuesByDataTypeId(definition.Id);

            if (prevalues == null || !prevalues.Any()) return;

            string preValue = prevalues.First();

            if (String.IsNullOrEmpty(preValue)) return;

            //Instantiate the model builder manager.
            ICustomModelBuilderManager modelBuilderManager = new CustomModelBuilderManager();

            //Build the Strongly typed model associated with this property editor, currently it will only build an archtype model.
            bool status = modelBuilderManager.BuildCustomModel(definition.Name, preValue, definition.PropertyEditorAlias);

            if (!status)
            {

            }
        }
    }
}