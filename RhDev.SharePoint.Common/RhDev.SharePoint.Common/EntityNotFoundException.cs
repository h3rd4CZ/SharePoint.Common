using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhDev.SharePoint.Common
{
    public class EntityNotFoundException : Exception
    {
        public string Name { get; private set; }

        public EntityNotFoundException(string name)
            : base()
        {
            this.Name = name;
        }

        public EntityNotFoundException(string name, string message)
            : base(message)
        {
            this.Name = name;
        }

        public EntityNotFoundException(string name, Exception inner)
            : base(string.Empty, inner)
        {
            this.Name = name;
        }

        public EntityNotFoundException(string name, string message, Exception inner)
            : base(message, inner)
        {
            this.Name = name;
        }
    }
}
