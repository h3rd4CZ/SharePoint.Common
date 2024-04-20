using System;

namespace RhDev.SharePoint.Common.DataAccess.Repository.Entities
{
    public class ApplicationConfiguration : EntityBase, ICloneable
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Module { get; set; }
        public ApplicationConfiguration()
        {

        }

        public ApplicationConfiguration(string key, string value, string module)
        {
            Key = key;
            Value = value;
            Module = module;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public ApplicationConfiguration Clone()
        {
            return (ApplicationConfiguration)MemberwiseClone();
        }
    }
}
