using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScyllaPlugins.ArchetypeModelBuilder.Constants
{
    //NOTE: Lifted from https://github.com/imulus/Archetype/blob/fbf686f559c2838b0aa319498496e09866093c5d/app/Umbraco/Umbraco.Archetype/Constants.cs
    //Because we are overhauling the property value converters which use this information.
    public static class ArchetypeConstants
    {
        public const string CacheKey_PreValueFromDataTypeId = "Archetype_GetArchetypePreValueFromDataTypeId_";

        //Just for accessibility, lets add the property editor alias here too since this class is named ArchetypeConstants
        public const string Archetype_PropertyEditorAlias = PropertyEditorAlias.Imulus_Archetype;

        public const string PreValueAlias = "archetypeConfig";

        public const string CacheKey_DataTypeByGuid = "";
    }
}