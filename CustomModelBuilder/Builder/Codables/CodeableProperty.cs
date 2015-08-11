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
        public Accessibility accessibility;
        public bool auto;
        public StringBuilder propertyBody;
        private StringBuilder _sb;


        public CodeableProperty(Type propertyTypeIn, string propertyNameIn, Accessibility accessibilityIn, bool autoIn = false, StringBuilder propertyBodyIn = null)
        {
            if (propertyTypeIn == null) throw new ArgumentNullException("propertyTypeIn");
            if (String.IsNullOrEmpty(propertyNameIn)) throw new ArgumentNullException("propertyNameIn");

            this.propertyType = propertyTypeIn;
            this.propertyName = propertyNameIn;
            this.auto = autoIn;

            if(!this.auto && propertyBody != null)
            {
                throw new Exception("If this is not an auto property, you cannot provide a body for it.");
            }
            else
            {
                this.propertyBody = propertyBodyIn;
            }

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

            if(this.accessibility == null)
            {
                this.accessibility = Accessibility.PUBLIC;
            }

            StringBuilder propertyLine = new StringBuilder();
            propertyLine.Append(String.Format("{0} {1} {2}", new object[3] { this.accessibility.ToString(), propertyTypeString, this.propertyName }));
            
            if(this.auto)
            {
                if(this.propertyBody != null)
                {
                    propertyLine.AppendLine("");
                    propertyLine.AppendLine("\t\t{");
                    propertyLine.AppendLine(this.propertyBody.ToString());
                    propertyLine.AppendLine("\t\t}");   
                }
                else
                {
                    propertyLine.Append(" {get; set;}");
                }
            }
            else
            {
                propertyLine.Append(";");
            }
            
            this._sb.AppendLine(propertyLine.ToString());

            return this._sb.ToString();
        }

        
    }

    public sealed class Accessibility
    {
        private readonly String _name;
        private readonly int _value;

        public static readonly Accessibility PRIVATE = new Accessibility(1, "private");
        public static readonly Accessibility PUBLIC = new Accessibility(2, "public");
        public static readonly Accessibility PROTECTED = new Accessibility(3, "protected");

        private Accessibility(int value, String name)
        {
            this._name = name;
            this._value = value;
        }

        public override string ToString()
        {
            return this._name;
        }
    }
}