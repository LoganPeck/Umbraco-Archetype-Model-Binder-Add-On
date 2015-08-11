using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.BuilderInterfaces;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Codables
{
    public class CodeableConstructor
    {
        public StringBuilder constructorBody;
        public Accessibility accessibility;
        public List<CodeableParameter> parameters;

        public CodeableConstructor(Accessibility accessibilityIn, List<CodeableParameter> parametersIn, StringBuilder constructorBodyIn)
        {
            this.accessibility = accessibilityIn;
            this.parameters = parametersIn;
            this.constructorBody = constructorBodyIn;
        }
    }
}
