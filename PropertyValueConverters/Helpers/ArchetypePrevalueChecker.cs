using Archetype.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScyllaPlugins.ArchetypeModelBuilder.PropertyValueConverters.Helpers
{
    public class ArchetypePrevalueChecker
    {
        private ArchetypePreValue _prevalue;

        public ArchetypePrevalueChecker(ArchetypePreValue prevalue)
        {
            if (prevalue == null) throw new ArgumentNullException("prevalue");

            this._prevalue = prevalue;
        }

        public bool HasMultipleFieldsets()
        {
            return this._prevalue.EnableMultipleFieldsets;
        }

        public bool IsMaxFieldsetsGreaterThan1()
        {
            return this._prevalue.MaxFieldsets != 1;
        }
    }
}