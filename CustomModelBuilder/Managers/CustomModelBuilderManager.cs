using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder
{
    public class CustomModelBuilderManager : ICustomModelBuilderManager
    {

        public bool BuildCustomModel(string dataTypeName, string dataSource, string propertyEditorAlias)
        {
            //Create the converter to take the data base source and put it into a c# object.
            IConvertableToCustomModel convertableModel = ModelSourceConverterFactory.CreateModelSourceConverter(dataTypeName, dataSource, propertyEditorAlias);

            if (convertableModel == null) return false;

            //This will dynamically create a c# class based off of the convertable.
            return convertableModel.CreateStronglyTypedModel(); 
        }
    }
}