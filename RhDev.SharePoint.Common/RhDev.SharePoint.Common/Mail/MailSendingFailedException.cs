using System;
using System.Runtime.Serialization;

namespace RhDev.SharePoint.Common.Mail
{
    [Serializable]
    public class MailSendingFailedException : Exception
    {
        public MailSendingFailedException()
        {
        }

        public MailSendingFailedException(string message)
            : base(message)
        {
        }

        public MailSendingFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected MailSendingFailedException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
