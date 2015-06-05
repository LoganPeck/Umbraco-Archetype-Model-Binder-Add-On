using Archetype.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using ScyllaPlugins.ArchetypeModelBuilder.Constants;
using ScyllaPlugins.ArchetypeModelBuilder.Helpers;
using ScyllaPlugins.ArchetypeModelBuilder.PropertyValueConverters.Helpers;

namespace ScyllaPlugins.ArchetypeModelBuilder.PropertyValueConverters
{
    //Could use these attributes if it always returned the same type. But we need to do some investigating to determine
    //what type needs returned.
    //[PropertyValueType(typeof(ArchetypeModel))]
    //[PropertyValueCache(PropertyCacheValue.All, PropertyCacheLevel.Request)]
    public class ArchetypePropertyValueConverter : PropertyValueConverterBase, IPropertyValueConverterMeta
    {

        public override bool IsConverter(Umbraco.Core.Models.PublishedContent.PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals(ArchetypeConstants.Archetype_PropertyEditorAlias, StringComparison.InvariantCultureIgnoreCase);
        }


        /// <summary>
        /// Convert the JSON to an ArchetypeModel
        /// </summary>
        /// <param name="propertyType"></param>
        /// <param name="source"></param>
        /// <param name="preview"></param>
        /// <returns></returns>
        public override object ConvertDataToSource(Umbraco.Core.Models.PublishedContent.PublishedPropertyType propertyType, object source, bool preview)
        {
            var defaultValue = new ArchetypeModel();

            if (source == null)
                return defaultValue;

            var sourceString = source.ToString();

            if (!sourceString.DetectIsJson())
                return defaultValue;


            var archetype = ArchetypeHelper.Instance.DeserializeJsonToArchetype(sourceString,
                (propertyType != null ? propertyType.DataTypeId : -1),
                (propertyType != null ? propertyType.ContentType : null));

            return archetype;
            
        }




        /// <summary>
        /// Convert the ArchetypeModel to the appriate strongly typed model
        /// </summary>
        /// <param name="propertyType"></param>
        /// <param name="source"></param>
        /// <param name="preview"></param>
        /// <returns></returns>
        public override object ConvertSourceToObject(Umbraco.Core.Models.PublishedContent.PublishedPropertyType propertyType, object source, bool preview)
        {
            ArchetypeModel model = source as ArchetypeModel;

            IDataTypeService dts = ApplicationContext.Current.Services.DataTypeService;

            ArchetypePreValue archPrevalue = GetArchetypePrevalues(propertyType.DataTypeId, dts);

            ArchetypeToTypedModelConverter converter = new ArchetypeToTypedModelConverter(archPrevalue, dts);

            object convertedModel = converter.ConvertArchetypeToTypedModel(model, propertyType);

            return convertedModel;
        }
        


        public PropertyCacheLevel GetPropertyCacheLevel(Umbraco.Core.Models.PublishedContent.PublishedPropertyType propertyType, PropertyCacheValue cacheValue)
        {
            return PropertyCacheLevel.Content;
        }


        /// <summary>
        /// This is a very important method. This will determine what type we can expect the archetype to give back in a strongly typed form.
        /// </summary>
        /// <param name="propertyType">The property type that we are inquiring about the Type..</param>
        /// <returns>The type that you can expect this archetype to resolve to when strongly typed.</returns>
        public Type GetPropertyValueType(Umbraco.Core.Models.PublishedContent.PublishedPropertyType propertyType)
        {
            //Get Our Custom namespace
            string customModelNamespace = ArchetypeValueProvider.GetModelsNamespace();

            IDataTypeService dts = ApplicationContext.Current.Services.DataTypeService;

            ArchetypePreValue archPrevalue = GetArchetypePrevalues(propertyType.DataTypeId, dts);

            IDataTypeDefinition dataTypeDef = dts.GetDataTypeDefinitionById(propertyType.DataTypeId);

            if (archPrevalue.Fieldsets == null || !archPrevalue.Fieldsets.Any()) throw new Exception("Archetype prevalues must have fieldsets defined.");

            return ArchetypeValueProvider.ResolveStronglyTypedType(archPrevalue, dataTypeDef.Name);
        }


        /// <summary>
        /// Get the prevalues for the archetype.
        /// </summary>
        /// <param name="dataTypeId">The data type of the archetype we are asking about. NOTE: will not work with any data type, it must be an archetype.</param>
        /// <param name="dts">The data type service.</param>
        /// <returns></returns>
        private ArchetypePreValue GetArchetypePrevalues(int dataTypeId, IDataTypeService dts)
        {
            
            IEnumerable<string> preValues = dts.GetPreValuesByDataTypeId(dataTypeId);

            if (preValues == null || !preValues.Any()) throw new Exception("Prevalues for archetype cannot be null.");

            return JsonConvert.DeserializeObject<ArchetypePreValue>(preValues.First());
        }
    }
     
}