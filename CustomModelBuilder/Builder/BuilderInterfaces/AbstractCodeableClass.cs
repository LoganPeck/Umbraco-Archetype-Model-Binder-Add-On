using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Helpers;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.BuilderInterfaces
{
    public abstract class AbstractCodeableClass : AbstractCodeable
    {
        protected List<CodeableProperty> _properties;
        public abstract void AddProperty(Type type, string propertyName);
        protected string _className;

        public AbstractCodeableClass(string className, List<string> usingAssemblies = null)
            :base(usingAssemblies)
        {
            this._properties = new List<CodeableProperty>();
            this._className = className;
        }

        public string GetClassName()
        {
            return this._className;
        }

        public override string GetCodeableName()
        {
            return GetClassName();
        }
    }
}