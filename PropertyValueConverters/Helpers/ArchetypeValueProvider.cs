using Archetype.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Umbraco.Core;

namespace ScyllaPlugins.ArchetypeModelBuilder.PropertyValueConverters.Helpers
{
    public static class ArchetypeValueProvider
    {
        public static string StringToInterfaceName(string nameToConvert)
        {
            if (String.IsNullOrEmpty(nameToConvert)) throw new ArgumentNullException("nameToConvert");

            string interfaceName = String.Format("I{0}", nameToConvert.ToFirstUpperInvariant());
            interfaceName = Regex.Replace(interfaceName, @"\s+", ""); //Replace all the whitespace

            return interfaceName;
        }

        /// <summary>
        /// Determines if a class or interface is needed as the type, and then determines if we need a list of those items or just a single one.
        /// </summary>
        /// <param name="archPrevalue"></param>
        /// <param name="dataTypeName"></param>
        /// <returns></returns>
        public static Type ResolveStronglyTypedType(ArchetypePreValue archPrevalue, string dataTypeName)
        {
            //A helper class that will help us check the pre-values of the archetype.
            ArchetypePrevalueChecker ruleChecker = new ArchetypePrevalueChecker(archPrevalue);

            //Do we need an interface to represent this archetype or will a class suffice?
            Type archetypeType = GetArchetypeType(ruleChecker, archPrevalue, dataTypeName);

            //Determines if we need a list of the interface/class or if a single instance is fine.
            return ResolveListOrSingleModel(ruleChecker, archetypeType);
        }
        

        /// <summary>
        /// Determines if this archetype needs an interface or just a class
        /// </summary>
        /// <param name="ruleChecker"></param>
        /// <param name="archPrevalue"></param>
        /// <param name="dataTypeName"></param>
        /// <returns></returns>
        public static Type GetArchetypeType(ArchetypePrevalueChecker ruleChecker, ArchetypePreValue archPrevalue, string dataTypeName)
        {
            Type archetypeType;

            if (ruleChecker.HasMultipleFieldsets()) //Multiple fieldsets are enabled so we need to set the type to an interface
            {
                string interfaceName = ArchetypeValueProvider.StringToInterfaceName(dataTypeName);

                archetypeType = Type.GetType(GetModelsNamespace() + "." + interfaceName + ", " + GetAssemblyName());
            }
            else //No multiple fieldsets so just a class as the type
            {
                ArchetypePreValueFieldset fieldset = archPrevalue.Fieldsets.First();

                string className = fieldset.Alias.ToFirstUpperInvariant();

                //string className = propertyType.PropertyTypeAlias.UppercaseFirstLetter();
                archetypeType = Type.GetType(GetModelsNamespace() + "." + className);
            }

            return archetypeType;
        }


        /// <summary>
        /// Determines whether this archetypes type needs to be an IEnumerable or a single type
        /// </summary>
        /// <param name="ruleChecker"></param>
        /// <param name="archetypeType"></param>
        /// <returns></returns>
        public static Type ResolveListOrSingleModel(ArchetypePrevalueChecker ruleChecker, Type archetypeType)
        {
            //See if we need to return a list or just the archetype type
            if (ruleChecker.IsMaxFieldsetsGreaterThan1())
            {
                //We need to return a list of the type
                Type listType = typeof(IEnumerable<>);
                Type archetypeTypeList = listType.MakeGenericType(archetypeType);

                return archetypeTypeList;
            }
            else
            {
                //Just return a single
                return archetypeType;
            }
        }

        //Gets the namespace that we are generating the strongly typed archetype models into
        public static string GetModelsNamespace()
        {
            //From the web.config under <appSetting>
            return ConfigurationManager.AppSettings["CustomModelBuilderNamespace"];
        }

        public static string GetAssemblyName()
        {
            //Im sure there is a better way to do this. This is so we know what assembly we are looking for the strongly typed model class (cs) file in.
            return ConfigurationManager.AppSettings["CustomModelBuilderAssembly"];
        }
    }
}