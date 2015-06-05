using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ScyllaPlugins.ArchetypeModelBuilder.Constants;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.BuilderInterfaces;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Builders;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Factories;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.Managers;
using ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.ConvertableObjects;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder
{
    public static class ModelSourceConverterFactory
    {

        public static IConvertableToCustomModel CreateModelSourceConverter(string dataTypeName, string dataSource, string propertyEditorAlias)
        {

            switch(propertyEditorAlias)
            {
                case PropertyEditorAlias.Imulus_Archetype:

                    //FACTORIES

                    //The factory responsible for creating the CodeableClass (which contains the logic on how to build the class)
                    ICodeableClassFactory classFactory = new ArchetypeCodeableClassFactory();

                    //The factory responsible for createing the InterfaceClass( which contains the logic on how to build the interface)
                    ICodeableInterfaceFactory interfaceFactory = new ArchetypeCodeableInterfaceFactory();


                    //BUILDERS

                    //Build the entity responsible for building the class, and give it where it should get its Codeable Class from (the factory)
                    ClassBuilder classBuilder = new ClassBuilder(classFactory);

                    //Build the entity responsible for building the interface, and give it where it should get its codeble interfacef rom (the factory)
                    InterfaceBuilder interfaceBuilder = new InterfaceBuilder(interfaceFactory);


                    List<AbstractCodeBuilder> codeBuilders = new List<AbstractCodeBuilder>();
                    codeBuilders.Add(classBuilder); //Add the class builder
                    codeBuilders.Add(interfaceBuilder); //add the interface builder


                    //MANAGER
                    //Create the manager that will manage the code builders.
                    CodeBuildManager codeBuilderManager = new CodeBuildManager(codeBuilders);

                    return new ArchetypeConvertableModel(dataTypeName, dataSource, codeBuilderManager);//new ArchetypeModelSourceConverter(dataSource);
                default:
                    return null;
                    
            }
                
        }
    }
}