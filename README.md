# Umbraco-Archetype-Model-Binder-Add-On
This will extend the Model Binder package for Umbraco 7 to support the Imulus.Archetype data type. It hooks into the IDataTypeService.Saved event and creates a strongly typed model for the saved archetype data type.

This is my first time using GitHub and sharing any kind of code, so any comments/advice is welcome! 

I plan on making a nuget package out of this so it is easy to install. 

# How it works

The first thing to take note of is how we are registering the model binder so it runs everytime an Imulus.Archetype data type is saved in the back office. This is in the App_Start/ModelBinderEvents.cs file.
From there, we will take the prevalues for the Archetype and determine if the archetype represents a class or if it needs to be an interface (this would be the case if Multiple Fieldsets were enabled.). It is also at this point that we determine if we need to return an IEnumerable of the class or interface; or if a single one will suffice. We can determine this by looking at the max fieldsets prevalue. IMPORTANT: It will generate the models in the /Models/Generated folder, so make sure you create that folder!
The other main piece to note is that I overrode the ArchetypeValueConverter. Note: I had to remove the old one in the ApplicationStarting method. Since the model binder gets its type from the property value converters, we needed to make archetype smart enough to tell the model binder what strongly typed model it represents. I did this by implementing the IPropertyValueConverterMeta interface. The other function that overriding the property value converter served was to return the strongly typed model instead of just a plain old ArchetypeModel.

#setup
You will need two settings in the appSettings section of your web.config.
CustomModelBuilderNamespace - This is the namespace that the generated model with have.
CustomModelBuilderAssembly - This is the name of the assembly that the strongly typed models will be generated in. (So whatever assembly you have your umbraco code running in.)

# Questions & Comments
Like I said, this is the first real piece of software I have shared. So if you have any suggestions/comments/advice on how I could improve my coding, style, anything, please feel free to comment. Also, please feel free to contribute!
