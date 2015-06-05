using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.BuilderInterfaces
{
    public class AbstractCodeableInterface : AbstractCodeable
    {
        protected string _interfaceName;

        public AbstractCodeableInterface(string interfaceName, List<string> usingAssemblies = null)
            :base(usingAssemblies)
        {
            this._interfaceName = interfaceName;
        }

        public string GetInterfaceName()
        {
            return this._interfaceName;
        }

        public override string GetCodeableName()
        {
            return GetInterfaceName();
        }

        public override void AppendCodeBody()
        {
            this._sb.AppendLine(String.Format("\tpublic partial interface {0}", this._interfaceName));
            this._sb.AppendLine("\t{"); //start the interface
        }
    }
}