using Archetype.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.BuilderInterfaces;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Builders;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Helpers;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Managers;
using ScyllaPlugins.ArchetypeModelBuilder.Helpers;
using ScyllaPlugins.ArchetypeModelBuilder.PropertyValueConverters.Helpers;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.ConvertableObjects
{
    public class ArchetypeConvertableModel : IConvertableToCustomModel
    {
        private ArchetypePreValue _data;
      
        private IDataTypeService _dts;

        private CodeBuildManager _codeBuildManager;

        private string _dataSource;

        private string _dataTypeName; 

        public ArchetypeConvertableModel(string dataTypeName, string jsonDataSource, CodeBuildManager cbm)
        {
            if(!String.IsNullOrEmpty(dataTypeName))
            {
                this._dataTypeName = dataTypeName;
            }

            if(String.IsNullOrEmpty(jsonDataSource)) throw new ArgumentNullException("jsonDataSource");

            this._dataSource = jsonDataSource;

            if (cbm == null) throw new ArgumentNullException("cbm");

            this._codeBuildManager = cbm;

            _dts = ApplicationContext.Current.Services.DataTypeService;
        }

        public ArchetypeConvertableModel(ArchetypePreValue arch, CodeBuildManager cbm)
        {
            if(arch != null)
            {
                this._data = arch;
            }

            if(cbm != null)
            {
                this._codeBuildManager = cbm;
            }

            _dts = ApplicationContext.Current.Services.DataTypeService;
        }

        private ArchetypePreValue DataSourceToConvertableObject()
        {
            try
            {
                ArchetypePreValue pv = JsonConvert.DeserializeObject<ArchetypePreValue>(this._dataSource);

                return pv;
            }
            catch (Exception e)
            {
                throw new Exception("Could not convert the json data to an ArchetypePreValue.", e);
            }
        }

        public bool CreateStronglyTypedModel()
        {
            if (this._codeBuildManager == null) return false;

            if(this._data == null)
            {
                this._data = DataSourceToConvertableObject(); //Convert the json to the prevalue
            }

            string interfaceName = "";
            if(this._data.EnableMultipleFieldsets)
            {
                interfaceName = ArchetypeValueProvider.StringToInterfaceName(this._dataTypeName);
                
                //Mulitple fieldsets are enabled so we want to create an interface and have each class inherit from it.
                bool status = CreateInterfaceFromArchetype(interfaceName);
            }

            foreach(var fieldset in this._data.Fieldsets)
            {
                //Create a model fro each fieldset
                CreateModelsFromFieldset(fieldset, interfaceName);
            }

            try
            {
                this._codeBuildManager.CreateSourceFilesForCodeableUnits();
                return true;
            }
            catch(Exception e)
            {
                throw new Exception("Could not create strongly teyped model for archetype",e);
            }
        }

        private bool CreateInterfaceFromArchetype(string interfaceName)
        {
            try
            {
                //Get the interface builder
                AbstractCodeBuilder ib = this._codeBuildManager.GetCodeBuilder(Enum.CodeableUnitType.Interface);

                //Create the interface.
                ib.CreateCodeEntity(interfaceName);

                //Commit it because we are done building it.
                ib.CommitCodeEntity();

                return true;
            }
            catch(Exception e)
            {
                throw new Exception("Could not create interface " + interfaceName, e);
            }
            
        }

        private bool CreateModelsFromFieldset(ArchetypePreValueFieldset fieldset, string inheritsInterface)
        {
            ClassBuilder classBuilder = (ClassBuilder)this._codeBuildManager.GetCodeBuilder(Enum.CodeableUnitType.Class);

            //changes "myFieldset" to "MyFieldset"
            string className = fieldset.Alias.ToFirstUpperInvariant();

            dynamic classOptions = new System.Dynamic.ExpandoObject();

            if(!String.IsNullOrEmpty(inheritsInterface))
            {
                classOptions.InheritsInterface = inheritsInterface;
            }

            CodeableClass currentClass = (CodeableClass)classBuilder.CreateCodeEntity(className, new List<string>(){"System", "Archetype"}, classOptions);
            
            //Create the model before we put properties on it, we have to do this because if this 

            foreach(var property in fieldset.Properties)
            {
                IDataTypeDefinition dataTypeDef = this._dts.GetDataTypeDefinitionById(property.DataTypeGuid);
                
                Type stronglyTypedType = UmbracoModelsHelper.GetDataTypeType(dataTypeDef);

                currentClass.AddProperty(stronglyTypedType, property.Alias.ToFirstUpperInvariant());
            }

            classBuilder.CommitCodeEntity();

            return true;
        }
    }
}