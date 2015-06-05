﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder
{
    interface ICustomModelBuilderManager
    {
        bool BuildCustomModel(string dataTypeName, string dataSource, string propertyEditorAlias);
    }
}
