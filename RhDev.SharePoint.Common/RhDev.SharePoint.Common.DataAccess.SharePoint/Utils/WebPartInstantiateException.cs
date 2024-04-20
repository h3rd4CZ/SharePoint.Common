using System;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Utils
{
    public class WebPartInstantiateException : Exception
    {
        public string TypeName { get; set; }

        public WebPartInstantiateException(string message, string typeName)
            : base(message)
        {
            this.TypeName = typeName;
        }

        public WebPartInstantiateException(string message, string typeName, Exception innerException)
            : base(message, innerException)
        {
            this.TypeName = typeName;
        }
    }
}
