using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RhDev.SharePoint.Common
{
    [Serializable]
    public class FolderNotFoundException : Exception
    {
        public FolderNotFoundException()
        {
        }

        public FolderNotFoundException(string message)
            : base(message)
        {
        }

        public FolderNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected FolderNotFoundException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
