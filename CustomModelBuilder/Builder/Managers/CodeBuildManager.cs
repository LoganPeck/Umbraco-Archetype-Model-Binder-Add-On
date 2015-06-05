using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.BuilderInterfaces;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Builders;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Enum;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Managers
{
    public class CodeBuildManager
    {

        protected List<AbstractCodeBuilder> _builders;

        public CodeBuildManager(List<AbstractCodeBuilder> builders = null)
        {
            if(builders != null && builders.Any())
            {
                this._builders = builders;
            }
        }

        public virtual AbstractCodeBuilder GetCodeBuilder(CodeableUnitType type)
        {
            switch(type)
            {
                case CodeableUnitType.Class:
                    ClassBuilder cb = (ClassBuilder)this._builders.Where(x => x is ClassBuilder).FirstOrDefault();
                    if (cb == null) throw new Exception("There is no code builder for a class in the list of code builders.");
                    return cb;
                case CodeableUnitType.Interface:
                    InterfaceBuilder ib = (InterfaceBuilder)this._builders.Where(x => x is InterfaceBuilder).FirstOrDefault();
                    if (ib == null) throw new Exception("There is no code builder for a interface in the list of code builders.");
                    return ib;
                default:
                    throw new Exception("Could not find the code builder you were looking for in the builders list");
            }
        }

        public virtual bool CreateSourceFilesForCodeableUnits()
        {

            string currentPathToModelsFolder = AppDomain.CurrentDomain.BaseDirectory + "Models\\Generated\\";

            CSharpCodeProvider codeProvider = new CSharpCodeProvider();

            List<AbstractCodeable> codeables = this.GetCodeableEntities();

            foreach (AbstractCodeable c in codeables)
            {
                CodeSnippetCompileUnit cu = new CodeSnippetCompileUnit(c.ToCode());
                string sourceFile;
                if (codeProvider.FileExtension.Any() && codeProvider.FileExtension[0] == '.')
                {
                    sourceFile = currentPathToModelsFolder + c.GetCodeableName() + codeProvider.FileExtension;
                }
                else
                {
                    sourceFile = currentPathToModelsFolder + c.GetCodeableName() + "." + codeProvider.FileExtension;
                }


                try
                {
                    using (StreamWriter sw = new StreamWriter(sourceFile, false))
                    {
                        IndentedTextWriter tw = new IndentedTextWriter(sw, "     ");

                        codeProvider.GenerateCodeFromCompileUnit(cu, tw, new CodeGeneratorOptions());
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(String.Format("Could not create class {0} from generated source code.", c.GetCodeableName()), e);
                }
            }


            return true;

        }



        protected virtual List<AbstractCodeable> GetCodeableEntities()
        {
            List<AbstractCodeable> codeables = new List<AbstractCodeable>();

            foreach (AbstractCodeBuilder builder in this._builders)
            {
                codeables.AddRange(builder.builtEntites);
            }

            return codeables;
        }
    }
}
