using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhDev.SharePoint.Common.Configuration
{
    public class ConfigurationKey : ICloneable
    {
        private readonly string module;

        protected readonly string code;

        public string Web { get; set; }

        public string Module => module;

        public ConfigurationKey(string module, string code)
        {
            this.module = module;
            this.code = code;
        }

        public override string ToString()
        {
            return String.Format("{0}__{1}{2}", Module, code,
                string.IsNullOrEmpty(Web) ? string.Empty : string.Format("_{0}", Web));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != GetType()) return false;

            var other = (ConfigurationKey)obj;
            return String.Equals(Module, other.Module) && String.Equals(code, other.code);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Module.GetHashCode() * 397) ^ code.GetHashCode();
            }
        }

        public object Clone()
        {
            var clone = new ConfigurationKey(Module, code);

            return clone;
        }
    }
}
