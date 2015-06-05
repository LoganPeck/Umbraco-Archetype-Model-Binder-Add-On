using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Codables;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Factories
{
    public class ArchetypeCodeableInterfaceFactory : ICodeableInterfaceFactory
    {
        public BuilderInterfaces.AbstractCodeableInterface CreateInterface(string interfaceName, List<string> usingAssemblies = null)
        {
            return new CodeableInterface(interfaceName, usingAssemblies);
        }
    }
}
