using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RhDev.SharePoint.Common.Configuration
{
    [Serializable]
    public class MissingConfigurationValueException : Exception
    {
        public MissingConfigurationValueException()
        {
        }

        public MissingConfigurationValueException(string message)
            : base(message)
        {
        }

        public MissingConfigurationValueException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected MissingConfigurationValueException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
