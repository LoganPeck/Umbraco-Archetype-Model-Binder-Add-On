using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.BuilderInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Codables
{
    public class CodeableParameter : ICodeable
    {
        private Type _paramType;
        private string _paramName;

        public CodeableParameter(Type paramType, string paramName)
        {
            this._paramName = paramName;
            this._paramType = paramType;
        }


        public string ToCode()
        {
            return _paramType.FullName.ToString() + " " + _paramName;
        }
    }
}
