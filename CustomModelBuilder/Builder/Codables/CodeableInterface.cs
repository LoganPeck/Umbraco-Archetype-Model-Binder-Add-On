using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.BuilderInterfaces;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Codables
{
    public class CodeableInterface: AbstractCodeableInterface
    {
        
        public CodeableInterface(string interfaceName, List<string> usingAssemblies = null)
            :base(interfaceName, usingAssemblies)
        {}

    }
}