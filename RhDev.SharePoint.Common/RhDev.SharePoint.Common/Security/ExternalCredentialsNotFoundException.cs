using System;
using System.Runtime.Serialization;

namespace RhDev.SharePoint.Common.Security
{
    [Serializable]
    public class ExternalCredentialsNotFoundException : Exception
    {
        public ExternalCredentialsNotFoundException()
        {
        }

        public ExternalCredentialsNotFoundException(string message)
            : base(message)
        {
        }

        public ExternalCredentialsNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ExternalCredentialsNotFoundException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
