using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.BuilderInterfaces;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Helpers
{
    public class CodeableNamespace : ICodeable
    {
        public string myNamespace; 

        public CodeableNamespace(string namespaceIn)
        {
            this.myNamespace = namespaceIn;
        }

        public string ToCode()
        {
            return String.Format("namespace {0}", this.myNamespace);
        }
    }
}