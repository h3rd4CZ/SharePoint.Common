using System;

namespace RhDev.SharePoint.Common
{
    public class DuplicateEntityException : Exception
    {
        public string Name { get; private set; }

        public DuplicateEntityException(string name)
        {
            this.Name = name;
        }

        public DuplicateEntityException(string name, string message)
            : base(message)
        {
            this.Name = name;
        }
    }
}
