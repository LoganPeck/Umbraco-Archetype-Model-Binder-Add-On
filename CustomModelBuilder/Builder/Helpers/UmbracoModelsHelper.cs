using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Helpers
{
    public static class UmbracoModelsHelper
    {
        public static Type GetDataTypeType(IDataTypeDefinition dataTypeDef)
        {
            PublishedPropertyType ppt = new PublishedPropertyType(null, new PropertyType(dataTypeDef));

            //Type returnType = Type.GetType(ppt.ClrType.FullName);

            return ppt.ClrType;
        }
    }
}