using Archetype.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;

namespace ScyllaPlugins.ArchetypeModelBuilder.PropertyValueConverters.Helpers
{
    public class ArchetypeToTypedModelConverter
    {
        private ArchetypePreValue _prevalues;
        private ArchetypePrevalueChecker _rulesChecker;
        private IDataTypeService _dataTypeService;

        private string _modelsNamespace;

        public ArchetypeToTypedModelConverter(ArchetypePreValue prevalues, IDataTypeService dts)
        {
            if (prevalues == null) throw new ArgumentNullException("prevalues");
            if (dts == null) throw new ArgumentNullException("dts");

            this._prevalues = prevalues;

            this._rulesChecker = new ArchetypePrevalueChecker(this._prevalues);

            this._dataTypeService = dts;

            this._modelsNamespace = ArchetypeValueProvider.GetModelsNamespace();
        }

        /// <summary>
        /// Given an archetype model and a published property type, this method will create the strongly typed version
        /// of the archetype.
        /// </summary>
        /// <param name="archModel">The archetype to create the strongly typed model for.</param>
        /// <param name="propType">The property type that this archetype came from.</param>
        /// <returns>An strongly typed model that represents the archetype. This will be the same type that is returned from the property value converter.</returns>
        public object ConvertArchetypeToTypedModel(ArchetypeModel archModel, PublishedPropertyType propType)
        {
            if(archModel == null) throw new ArgumentNullException("archModel");

            //if there are no fieldsets then just return null
            if(archModel.Fieldsets == null || !archModel.Fieldsets.Any()) return null;

            //Get the data type definition.
            IDataTypeDefinition dataTypeDef = this._dataTypeService.GetDataTypeDefinitionById(propType.DataTypeId);

            //Get the Strongly Typed Model type that this archetype represents.
            Type objectType = ArchetypeValueProvider.ResolveStronglyTypedType(this._prevalues, dataTypeDef.Name);

            //Check to see if the objectType is an enumerable.
            if(objectType.Name.Equals(typeof(IEnumerable<>).Name, StringComparison.InvariantCultureIgnoreCase))
            {
                //We are dealing with an IEnumerable, so multiple fieldsets.

                //Get the type inside the ienumerable
                if (objectType.GenericTypeArguments == null || !objectType.GenericTypeArguments.Any()) throw new Exception("An IEnumerable should have generic type parameters.");
                Type enumerableType = objectType.GenericTypeArguments[0];

                //Create the generic list, must be a list because we will be adding to it. 
                Type listType = typeof(List<>); //Get the list type
                Type listOfTypeType = listType.MakeGenericType(enumerableType); //Make a list type of the enumerable type
                object listInstance = Activator.CreateInstance(listOfTypeType);

                //Get the Add method on List
                MethodInfo addMethod = listOfTypeType.GetMethods().Where(x => x.Name.Equals("Add", StringComparison.InvariantCultureIgnoreCase)).First();

                if(addMethod == null) throw new Exception("Could not find the method Add() on the List<> type");

                //Loop through each fieldset and create its stronly typed model with the property values mapped to the strongly typed model.
                foreach(ArchetypeFieldsetModel fieldset in archModel.Fieldsets)
                {
                    object typedModel = CreateTypedModelFromFieldset(fieldset, enumerableType);
                    
                    //Could be null if the fieldset is not active (that little power button on the fieldset)
                    if (typedModel != null)
                    {
                        //Now add the typed model to the list
                        addMethod.Invoke(listInstance, new object[] { typedModel });
                    }
                }

                return listInstance;
            }
            else
            {
                //Its just one fieldset.
                ArchetypeFieldsetModel fieldset = archModel.Fieldsets.First();

                object typedModel = CreateTypedModelFromFieldset(fieldset, objectType);

                if(typedModel != null)
                {
                    return typedModel;
                }
                else
                {
                    return null;
                }
            }
        }


        /// <summary>
        /// Given a ArchetypeFieldsetModel and a createType, this method will create an instance of the corresponding strongly
        /// typed model that we generated on Archetype Data Type save. It will also map the fieldset's property values to the strongly 
        /// typed instances properties.
        /// </summary>
        /// <param name="fieldset">The fieldset that represents the strongly typed model.</param>
        /// <param name="createType"></param>
        /// <returns></returns>
        private object CreateTypedModelFromFieldset(ArchetypeFieldsetModel fieldset, Type createType)
        {
            if (fieldset == null) throw new ArgumentNullException("fieldset");
            if (createType == null) throw new ArgumentNullException("createType");

            if(fieldset.Disabled) //This is the little power button found on the fieldsets.
            {
                return null; //its disabled so it shouldn't return anything.
            }

            //createType will be an interface when there are multiple fieldsets enabled. Making it an interface gives us a common
            //way to reference the objects within the archetype.
            if(createType.IsInterface)
            {
                //Its an interface so we want to set the createType to the base class that will implement the interface,
                //this can be determined by getting the fieldsets alias and converting it to a class. This is how the interface
                //is created during the strongly typed model creation phase.
                string implementingClassName = fieldset.Alias.ToFirstUpperInvariant();

                Type implementingClassType = Type.GetType(ArchetypeValueProvider.GetModelsNamespace() + "." + implementingClassName);

                if (implementingClassType == null) throw new Exception(String.Format("Could not find implementing base type for interface {0}", createType));

                //Set the create type to the class type that is implementing the interface.
                createType = implementingClassType;
            }

            //Create an instance of the strongly typed class that represents this archetype's fieldset.
            object classInstance = Activator.CreateInstance(createType);

            //Get the properties from the strongly typed class that we generated for the archetype.
            PropertyInfo[] classProperties = createType.GetProperties();

            foreach(PropertyInfo classProperty in classProperties)
            {
                string propertyName = classProperty.Name.ToFirstLowerInvariant(); //Get the property name from the strongly typed class
                  
                //Get the corresponding archetype property
                ArchetypePropertyModel archProp = fieldset.Properties.FirstOrDefault(x => x.Alias.Equals(propertyName));
                
                //We need to get the data type definition for this property because we need to run it through a property value converter,
                //which requires a PublishedPropertyType, which requires a PropertyType, which requires a data type definition
                IDataTypeDefinition dataTypeDef = this.GetDataTypeDefinitionForArchetypeProperty(fieldset.Alias, archProp.Alias);

                //Get the method from the PropertyValueConverterHelper that will get us the archetype properties value.
                MethodInfo getValueMethod = typeof(PropertyValueConverterHelper).GetMethods().Where(x => x.Name.Equals("GetValueThroughConverters")).First();
                MethodInfo genericGetValueMethod = getValueMethod.MakeGenericMethod(classProperty.PropertyType);

                //Get the property value
                var propertyValue = genericGetValueMethod.Invoke(null, new object[] { archProp.Value, dataTypeDef });

                if(archProp != null)
                {
                    //Set the value on the strongly typed class.
                    classProperty.SetValue(classInstance, propertyValue);
                }
            }

            return classInstance;
        }



        /// <summary>
        /// Gets the data type definition for the Property Editor of the given "currentPropertyAlias"
        /// </summary>
        /// <param name="currentFieldsetAlias">The current fieldset we are searching.</param>
        /// <param name="currentPropertyAlias">The property alias that we are searching the archetype fieldset for.</param>
        /// <returns>A data type definition representing the archetype's property, property editor.</returns>
        private IDataTypeDefinition GetDataTypeDefinitionForArchetypeProperty(string currentFieldsetAlias, string currentPropertyAlias)
        {
            if (String.IsNullOrEmpty(currentFieldsetAlias)) throw new ArgumentNullException("currentFieldsetAlias");
            if (String.IsNullOrEmpty(currentPropertyAlias)) throw new ArgumentNullException("currentPropertyAlias");

            //Get the prevalue fieldset from the fieldset(currentFieldsetAlias) that we are currently dealing with.
            ArchetypePreValueFieldset preValueFieldset = this._prevalues.Fieldsets.Where(x => x.Alias.Equals(currentFieldsetAlias)).FirstOrDefault();

            if (preValueFieldset == null) throw new Exception(String.Format("The fieldset prevalue for alias {0} should not be null", currentFieldsetAlias));


            //Get the prevalue property from the pre value fieldset that we are in.
            ArchetypePreValueProperty preValueProperty = preValueFieldset.Properties.Where(x => x.Alias.Equals(currentPropertyAlias)).FirstOrDefault();

            if (preValueProperty == null) throw new Exception(String.Format("The property prevalue for property alias {0} should not be null", currentPropertyAlias));

            //Get the data type definition for the current property editr.
            IDataTypeDefinition dataTypeDef = this._dataTypeService.GetDataTypeDefinitionById(preValueProperty.DataTypeGuid);

            return dataTypeDef;
        }
    }
}