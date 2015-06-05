using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace ScyllaPlugins.ArchetypeModelBuilder.PropertyValueConverters.Helpers
{
    public static class PropertyValueConverterHelper
    {
        public static T GetValueThroughConverters<T>(object value, IDataTypeDefinition dataTypeDef)
        {
            PublishedPropertyType properyType = new PublishedPropertyType(null, new PropertyType(dataTypeDef));//new PublishedPropertyType(null, new PropertyType(new DataTypeDefinition(-1, prop.PropertyEditorAlias) { Id = prop.DataTypeId }));
            // In umbraco, there are default value converters that try to convert the 
            // value if all else fails. The problem is, they are also in the list of
            // converters, and the means for filtering these out is internal, so
            // we currently have to try ALL converters to see if they can convert
            // rather than just finding the most appropreate. If the ability to filter
            // out default value converters becomes public, the following logic could
            // and probably should be changed.
            foreach (var converter in PropertyValueConvertersResolver.Current.Converters.Where(x => x.IsConverter(properyType)))
            {
                // Convert the type using a found value converter
                var value2 = converter.ConvertDataToSource(properyType, value, false);

                T result;  
                Attempt<T> attempt;
                // If the value is of type T, just return it
                if (value2 is T)
                {
                    attempt = Attempt<T>.Succeed((T)value2);
                    if (attempt.Success)
                    {
                        result = attempt.Result;
                    }
                    else
                    {
                        result = default(T);
                    }
                }


                // If ConvertDataToSource failed try ConvertSourceToObject.
                var value3 = converter.ConvertSourceToObject(properyType, value2, false);

                // If the value is of type T, just return it
                if (value3 is T)
                {
                    attempt = Attempt<T>.Succeed((T)value3);
                    if (attempt.Success)
                    {
                        return attempt.Result;
                    }
                    else
                    {
                        result = default(T);
                    }
                }

                // Value is not final value type, so try a regular type conversion aswell
                var convertAttempt = value2.TryConvertTo<T>();
                if (convertAttempt.Success)
                {
                    attempt = Attempt<T>.Succeed(convertAttempt.Result);
                    if (attempt.Success)
                    {
                        return attempt.Result;
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }

            return default(T);//nullAttempt<T>.Fail();
        }
    }
}