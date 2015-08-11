using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Helpers;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Codables;
using System.Text;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.BuilderInterfaces
{
    public abstract class AbstractCodeableClass : AbstractCodeable
    {
        protected List<CodeableProperty> _properties;
        protected List<CodeableConstructor> _constructors;

        public abstract void AddProperty(Type type, string propertyName, Accessibility accessibility, bool auto = false, StringBuilder propertyBody = null);
        public abstract void AddConstructor(Accessibility accessibility, List<CodeableParameter> parameters, StringBuilder constructorBody);

        protected string _className;

        public AbstractCodeableClass(string className, List<string> usingAssemblies = null)
            :base(usingAssemblies)
        {
            this._constructors = new List<CodeableConstructor>();
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