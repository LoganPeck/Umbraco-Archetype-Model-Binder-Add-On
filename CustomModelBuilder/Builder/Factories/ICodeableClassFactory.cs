using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.BuilderInterfaces;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Factories
{
    public interface ICodeableClassFactory
    {
        AbstractCodeableClass CreateClass(string className, List<string> usingAssemblies = null, dynamic options = null);
    }
}