using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.BuilderInterfaces;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Factories;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Builders
{
    public class InterfaceBuilder : AbstractCodeBuilder
    {
        private ICodeableInterfaceFactory _interfaceFactory;

        public InterfaceBuilder(ICodeableInterfaceFactory interfaceFactory)
            :base()
        {
            if (interfaceFactory == null) throw new ArgumentNullException("interfaceFactory");

            this._interfaceFactory = interfaceFactory;
        }

        public override AbstractCodeable CreateCodeEntity(string interfacename, List<string> usingAssemblies = null, dynamic options = null)
        {
            if (this._currentEntity != null) throw new Exception("You must commit your current class before creating another.");

            this._currentEntity = this._interfaceFactory.CreateInterface(interfacename, usingAssemblies);

            return this._currentEntity;
        }
    }
}