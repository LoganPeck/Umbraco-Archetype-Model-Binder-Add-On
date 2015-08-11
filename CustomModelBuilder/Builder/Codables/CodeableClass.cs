using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.BuilderInterfaces;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Factories;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Codables;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Helpers
{
    public class CodeableClass : AbstractCodeableClass
    {
        private ICodeablePropertyFactory _propertyFactory;
        private dynamic _options;

        public CodeableClass(string classNameIn, ICodeablePropertyFactory pf, List<string> usingAssemblies = null, dynamic options = null)
            :base(classNameIn, usingAssemblies)
        {
            if (pf == null) throw new ArgumentNullException("pf");
            
            if (options != null)
            {
                this._options = options;
            }

            this._propertyFactory = pf;
        }


        public override void AppendCodeBody()
        {
            //CLASS
            try
            {
                if(!String.IsNullOrEmpty(this._options.InheritsInterface as string))
                {
                    this._sb.AppendLine(String.Format("\tpublic partial class {0} : {1}", this._className, this._options.InheritsInterface as string));
                }
                else
                {
                    this._sb.AppendLine(String.Format("\tpublic partial class {0}", this._className));
                }
            }
            catch
            {
                this._sb.AppendLine(String.Format("\tpublic partial class {0}", this._className));
            }
            
            
            this._sb.AppendLine("\t{"); //start the class

            //Constructors
            foreach(CodeableConstructor constructor in this._constructors)
            {
                this._sb.AppendLine("");
                string constructorLine = String.Format("\t\t{0} {1} (", constructor.accessibility, this._className);
                foreach(CodeableParameter param in constructor.parameters)
                {
                    constructorLine += param.ToCode() + ",";
                }
                constructorLine = constructorLine.TrimEnd(',');
                constructorLine += ")";
                this._sb.AppendLine(constructorLine);
                this._sb.AppendLine("\t\t{");
                this._sb.AppendLine("\t\t\t"+constructor.constructorBody.ToString());
                this._sb.AppendLine("\t\t}");
            }

            this._sb.AppendLine("");

            //PROPERTIES
            foreach (CodeableProperty cp in this._properties)
            {
                this._sb.AppendLine("\t\t" + cp.ToCode());
            }
            //End Properties.
        }

        public override void AddConstructor(Accessibility accessibility, List<CodeableParameter> parameters, StringBuilder constructorBody)
        {
            this._constructors.Add(new CodeableConstructor(accessibility, parameters, constructorBody));
        }

       
        public override void AddProperty(Type type, string propertyName, Accessibility accessibility, bool auto = false, StringBuilder propertyBody = null)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");

            this._properties.Add(new CodeableProperty(type, propertyName, accessibility, auto, propertyBody));
        }


        public override bool IsReadyToCode()
        {
            return HasUsing() && HasNamespace() && HasProperties();
        }


        private bool HasProperties()
        {
            return this._properties != null && this._properties.Any();
        }
    }
}