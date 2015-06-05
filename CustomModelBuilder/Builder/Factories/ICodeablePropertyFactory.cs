using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.BuilderInterfaces;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Factories
{
    public interface ICodeablePropertyFactory
    {
        AbstractCodeableProperty CreateProperty(Type propertyType, string propertyName);
    }
}
