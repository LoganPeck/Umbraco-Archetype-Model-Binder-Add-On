using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.BuilderInterfaces;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Helpers
{
    public class CodeableUsing : ICodeable
    {
        public string assemblyName;

        public CodeableUsing(string assemblyNameIn)
        {
            if (String.IsNullOrEmpty(assemblyNameIn)) throw new ArgumentNullException("assemblyNameIn");
            this.assemblyName = assemblyNameIn;
        }

        public string GetAssemblyName()
        {
            return assemblyName;
        }

        public string ToCode()
        {
            return String.Format("using {0};", this.assemblyName);
        }
    }
}