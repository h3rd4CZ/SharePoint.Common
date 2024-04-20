using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace RhDev.SharePoint.Common.Notifications
{
    [Serializable]
    public class NotificationSendingFailedException : Exception
    {
        public NotificationSendingFailedException()
        {
        }

        public NotificationSendingFailedException(string message)
            : base(message)
        {
        }

        public NotificationSendingFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected NotificationSendingFailedException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
