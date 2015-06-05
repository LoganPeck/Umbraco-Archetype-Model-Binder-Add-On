using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Helpers;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Factories
{
    public class ArchetypeCodeablePropertyFactory : ICodeablePropertyFactory
    {
        public BuilderInterfaces.AbstractCodeableProperty CreateProperty(Type propertyType, string propertyName)
        {
            return new CodeableProperty(propertyType, propertyName);
        }
    }
}