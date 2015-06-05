using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.BuilderInterfaces;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Factories;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Helpers;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder
{
    public class ClassBuilder : AbstractCodeBuilder
    {

        private ICodeableClassFactory _classFactory;

        public ClassBuilder(ICodeableClassFactory classFactory)
            :base()
        {
            if (classFactory == null) throw new ArgumentNullException("classFactory");

            this._classFactory = classFactory;
        }

        public override AbstractCodeable CreateCodeEntity(string className, List<string> usingAssemblies = null, dynamic options = null)
        {
            if (this._currentEntity != null) throw new Exception("You must commit your current class before creating another.");

            this._currentEntity = this._classFactory.CreateClass(className, usingAssemblies, options);

            return this._currentEntity;
        }
    }
}