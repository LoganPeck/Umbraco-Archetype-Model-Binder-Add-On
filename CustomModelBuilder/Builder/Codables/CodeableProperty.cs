using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.BuilderInterfaces;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Helpers
{
    public class CodeableProperty : AbstractCodeableProperty
    {
        public Type propertyType;
        public string propertyName;

        private StringBuilder _sb;

        public CodeableProperty(Type propertyTypeIn, string propertyNameIn)
        {
            if (propertyTypeIn == null) throw new ArgumentNullException("propertyTypeIn");
            if (String.IsNullOrEmpty(propertyNameIn)) throw new ArgumentNullException("propertyNameIn");

            this.propertyType = propertyTypeIn;
            this.propertyName = propertyNameIn;

            this._sb = new StringBuilder();
        }

        public override string ToCode()
        {
            string propertyTypeString = this.propertyType.FullName;
            if(this.propertyType.Name.Equals(typeof(IEnumerable<>).Name))
            {
                Type typeParam = this.propertyType.GenericTypeArguments[0];
                propertyTypeString = "System.Collections.Generic.IEnumerable<" + typeParam.FullName + ">";
            }
            this._sb.AppendLine(String.Format("public {0} {1} {{get; set;}}", new object[2]{propertyTypeString, this.propertyName}));

            return this._sb.ToString();
        }
    }
}