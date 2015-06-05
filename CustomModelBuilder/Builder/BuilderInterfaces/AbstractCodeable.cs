using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Helpers;
using ScyllaPlugins.ArchetypeModelBuilder.PropertyValueConverters.Helpers;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.BuilderInterfaces
{
    public abstract class AbstractCodeable : ICodeable
    {

        protected StringBuilder _sb;
        protected List<CodeableUsing> _usings;
        protected CodeableNamespace _namespace;

        public abstract string GetCodeableName();
        public abstract void AppendCodeBody();

        public AbstractCodeable(List<string> usingAssemblies = null)
        {
            string namespaceForModels = ArchetypeValueProvider.GetModelsNamespace();

            this._namespace = new CodeableNamespace(namespaceForModels);
            this._usings = new List<CodeableUsing>();

            this._sb = new StringBuilder();

            if (usingAssemblies != null && usingAssemblies.Any())
            {
                foreach (var ass in usingAssemblies)
                {
                    this._usings.Add(new CodeableUsing(ass));
                }
            }
            else
            {
                this._usings.Add(new CodeableUsing("System"));
            }
        }

        

        public List<string> GetReferencedAssemblies()
        {
            List<string> assemblies = new List<string>();

            foreach (var a in this._usings)
            {
                assemblies.Add(a.GetAssemblyName() + ".dll");
            }

            return assemblies;
        }

        public virtual bool IsReadyToCode()
        {
            return HasUsing() && HasNamespace();
        }

        public void AddUsing(string assemblyName)
        {
            if (string.IsNullOrEmpty(assemblyName)) throw new ArgumentNullException("assemblyName");

            this._usings.Add(new CodeableUsing(assemblyName));
        }

        public void SetNamespace(string namespaceName)
        {
            if (String.IsNullOrEmpty(namespaceName)) throw new ArgumentNullException("namespaceName");
            this._namespace = new CodeableNamespace(namespaceName);
        }

        
        public virtual string ToCode()
        {
            ///******NOTE: Everything is appended to the string builder.
            ///
            if (!IsReadyToCode()) throw new Exception("You must make sure you have included using statements, namespace, and properties on this class before generating its code.");

            AppendHeader();

            AppendUsings(); //using namespace;

            //Add a space
            this._sb.AppendLine();

            AppendNamespace(); //namespace MyNamespace.Configured.In.Web.Config {

            AppendCodeBody(); //Class or interface or stuct or enum

            AppendClosingClassAndNamespaceBraces(); //} }

            return this._sb.ToString();
        }


        protected virtual bool HasUsing()
        {
            return this._usings != null && this._usings.Any();
        }

        protected virtual bool HasNamespace()
        {
            return this._namespace != null;
        }


        protected virtual void AppendHeader()
        {
            this._sb.AppendLine("/*");
            this._sb.AppendLine("This is auto generated code");
            this._sb.AppendLine("*/");
        }

        protected virtual void AppendUsings()
        {
            //USING
            foreach (CodeableUsing bu in this._usings)
            {
                this._sb.AppendLine(bu.ToCode());
            }
            //END USING
        }

        protected virtual void AppendNamespace()
        {
            //Namespace
            this._sb.AppendLine(this._namespace.ToCode());
            this._sb.AppendLine("{"); //Start the namespace.
            this._sb.AppendLine(); //insert another space.
            //End Namespace
        }

        protected virtual void AppendClosingClassAndNamespaceBraces()
        {
            this._sb.AppendLine("\t}"); //close the class
            this._sb.AppendLine("}"); //close the namespace
        }
    }
}