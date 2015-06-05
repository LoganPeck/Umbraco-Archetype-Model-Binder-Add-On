using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.BuilderInterfaces
{
    public abstract class AbstractCodeableProperty : ICodeable
    {

        public abstract string ToCode();

    }
}