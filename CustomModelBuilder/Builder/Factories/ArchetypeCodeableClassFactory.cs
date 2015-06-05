using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.BuilderInterfaces;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Helpers;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Factories
{
    public class ArchetypeCodeableClassFactory : ICodeableClassFactory
    {
        public AbstractCodeableClass CreateClass(string className, List<string> usingAssemblies, dynamic options = null)
        {
            return new CodeableClass(className, new ArchetypeCodeablePropertyFactory(), usingAssemblies, options);
        }
    }
}