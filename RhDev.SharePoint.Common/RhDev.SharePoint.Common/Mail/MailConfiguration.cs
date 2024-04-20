using System;

namespace RhDev.SharePoint.Common.Mail
{
    public class MailConfiguration
    {
        public string SenderEmailAddress { get; private set; }
        public string ReplyToEmailAddress { get; private set; }
        public string ServerAddress { get; private set; }

        public MailConfiguration(string senderEmailAddress, string replyToEmailAddress, string serverAddress)
        {
            if (String.IsNullOrEmpty(senderEmailAddress))
                throw new InvalidOperationException(
                    "The sender address is not set in outbound e-mail settings in Central Administration.");

            if (String.IsNullOrEmpty(replyToEmailAddress))
                throw new InvalidOperationException(
                    "The reply-to address is not set in outbound e-mail settings in Central Administration.");

            if (String.IsNullOrEmpty(serverAddress))
                throw new InvalidOperationException(
                    "The outbound SMTP server address is not set in outbound e-mail settings in Central Administration.");

            SenderEmailAddress = senderEmailAddress;
            ReplyToEmailAddress = replyToEmailAddress;
            ServerAddress = serverAddress;
        }
    }
}
