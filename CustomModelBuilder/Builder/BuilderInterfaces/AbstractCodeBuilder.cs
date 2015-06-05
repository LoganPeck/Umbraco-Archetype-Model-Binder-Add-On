using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScyllaPlugins.ArchetypeModelBuilder.CustomModelBuilder.Builder.BuilderInterfaces
{
    public abstract class AbstractCodeBuilder
    {
        protected AbstractCodeable _currentEntity { get; set; }
        protected string _sourceCode;

        public List<AbstractCodeable> builtEntites { get; protected set; }
        

        public AbstractCodeBuilder()
        {
            this.builtEntites = new List<AbstractCodeable>();
        }

        public abstract AbstractCodeable CreateCodeEntity(string entityName, List<string> usingAssemblies = null, dynamic options = null);
        

        public virtual string ToSourceCode()
        {
            foreach (AbstractCodeable c in this.builtEntites)
            {
                this._sourceCode += c.ToCode();
            }

            return this._sourceCode;
        }


        public virtual string[] GetReferencedAssemblies()
        {
            List<string> referencedAssemblies = new List<string>();

            foreach (AbstractCodeableClass c in this.builtEntites)
            {
                referencedAssemblies.AddRange(c.GetReferencedAssemblies());
            }

            return referencedAssemblies.ToArray();
        }


        public virtual void CommitCodeEntity()
        {
            this.builtEntites.Add(this._currentEntity);

            this._currentEntity = null;
        }

        public AbstractCodeable GetCurrentEntity()
        {
            return this._currentEntity;
        }
    }
}
