using System;
using System.Runtime.Serialization;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Security
{
    [Serializable]
    public class PrincipalsFromADGroupQueryMaximumReachedException : Exception
    {
        public PrincipalsFromADGroupQueryMaximumReachedException()
        {
        }

        public PrincipalsFromADGroupQueryMaximumReachedException(string message) : base(message)
        {
        }

        public PrincipalsFromADGroupQueryMaximumReachedException(string message, Exception inner) : base(message, inner)
        {
        }

        protected PrincipalsFromADGroupQueryMaximumReachedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
