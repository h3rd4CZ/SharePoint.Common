using System;
using System.Collections.Generic;
using System.Linq;
using RhDev.SharePoint.Common.DataAccess.Mail;
using RhDev.SharePoint.Common.Mail;
using RhDev.SharePoint.Common.Logging;
using System.Net.Mail;
using System.IO;
using RhDev.SharePoint.Common.Caching.Composition;
using System.Web;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Mail
{
    public class SmtpMailSender : ServiceBase, IMailSender
    {
        public const string MESSAGEHEADERS_DELIMITER = ";";
        public const string MESSAGEHEADERS_EQUAL_DELIMITER = "=";
        public const string RECIPIENTS_DELIMITER = ";";

        private readonly IMailConfigurationProvider mailConfigurationProvider;
        private MailConfiguration mailConfiguration;

        protected override TraceCategory TraceCategory => TraceCategories.Common;
                        
        public SmtpMailSender(IMailConfigurationProvider mailConfigurationProvider, ITraceLogger traceLogger) : base(traceLogger)
        {
            this.mailConfigurationProvider = mailConfigurationProvider;

            InitializeMailSettings();
        }

        private void InitializeMailSettings()
        {
            if (mailConfiguration != null)
                return;

            mailConfiguration = mailConfigurationProvider.GetMailConfiguration();
        }

        public void Send(MailQueueItem mailItem)
        {
            if (mailItem == null)
                throw new ArgumentNullException(nameof(mailItem));

            if (String.IsNullOrEmpty(mailItem.To))
                throw new ArgumentException("Mail recipient not set.");

            WriteTrace("Sending e-mail to {0} with subject \"{1}\"", mailItem.To, mailItem.Subject);

            try
            {

                var sender = mailConfiguration.SenderEmailAddress;

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.IsBodyHtml = mailItem.IsHtml;
                    
                    mailMessage.From = new MailAddress(sender);
                    mailMessage.Sender = new MailAddress(sender);
                    mailMessage.Subject = mailItem.Subject;
                    mailMessage.Body = mailItem.Body;

                    foreach (var recipient in GetRecipients(mailItem.To, mailItem.AdditionalRecipientsTo).Distinct())
                        mailMessage.To.Add(recipient);

                    if (!string.IsNullOrWhiteSpace(mailItem.CcTo))
                        PopulateListWithRecipients(m => m.CcTo, m => m.CC, mailMessage, mailItem);
                    
                    if (!string.IsNullOrWhiteSpace(mailItem.BccTo))
                        PopulateListWithRecipients(m => m.BccTo, m => m.Bcc, mailMessage, mailItem);

                    if (!string.IsNullOrWhiteSpace(mailItem.ReplyTo))
                        PopulateListWithRecipients(m => m.ReplyTo, m => m.ReplyToList, mailMessage, mailItem);

                    if (!string.IsNullOrWhiteSpace(mailItem.MessageHeaders))
                    {
                        var headerItems =
                            mailItem.MessageHeaders.Split(new[] { MESSAGEHEADERS_DELIMITER },
                                StringSplitOptions.RemoveEmptyEntries);

                        var col = headerItems.Select(h =>
                        {
                            var keyVal = h.Split(new[] {MESSAGEHEADERS_EQUAL_DELIMITER},
                                StringSplitOptions.RemoveEmptyEntries);

                            return new KeyValuePair<string, string>(keyVal[0], keyVal[1]);
                        });

                        foreach (var pair in col) mailMessage.Headers.Add(pair.Key, pair.Value);
                    }

                    foreach (var mailQueueItemAttachment in mailItem.Attachments)
                    {
                        Attachment attachment = new Attachment(
                                new MemoryStream(mailQueueItemAttachment.Data),
                                MimeMapping.GetMimeMapping(mailQueueItemAttachment.Name))
                            {Name = mailQueueItemAttachment.Name};

                        mailMessage.Attachments.Add(attachment);
                    }

                    var smtpClient = new SmtpClient {Host = mailConfiguration.ServerAddress};
                    smtpClient.Send(mailMessage);
                }

                WriteTrace("E-mail sent successfully");
            }
            catch (Exception e)
            {
                throw new MailSendingFailedException("Mail sending failed.", e);
            }
        }

        public void Dispose()
        {
        }

        private IEnumerable<string> GetRecipients(string to, string additional)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(to));

            yield return to;

            if(string.IsNullOrWhiteSpace(additional)) yield break;

            var additionalRecipients = additional.Split(new[] {RECIPIENTS_DELIMITER},
                StringSplitOptions.RemoveEmptyEntries);

            foreach (string additionalRecipient in additionalRecipients)
                yield return additionalRecipient;
        }

        private void PopulateListWithRecipients(Func<MailQueueItem, string> propGetter, Func<MailMessage, MailAddressCollection> recipientsGetter, MailMessage message, MailQueueItem mailItem)
        {
            if (propGetter == null) throw new ArgumentNullException(nameof(propGetter));
            if (recipientsGetter == null) throw new ArgumentNullException(nameof(recipientsGetter));
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (mailItem == null) throw new ArgumentNullException(nameof(mailItem));

            var prop = propGetter(mailItem);

            var recipients = prop.Split(new[] { RECIPIENTS_DELIMITER },
                StringSplitOptions.RemoveEmptyEntries).Distinct();

            var propToSet = recipientsGetter(message);

            foreach (string recipient in recipients)
                propToSet.Add(recipient);
        }
    }
}
