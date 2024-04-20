using System;
using System.Collections.Generic;

namespace RhDev.SharePoint.Common.Mail
{
    /// <summary>
    /// Item in mail queue
    /// </summary>
    public class MailQueueItem
    {
        public MailQueueItem(string to, string subject)
        {
            To = to;
            Subject = subject;
            Attachments = new List<MailQueueItemAttachment>();
        }

        public void IncludeAttachments(IList<MailQueueItemAttachment> attachments)
        {
            if (attachments == null) throw new ArgumentNullException(nameof(attachments));

            Attachments = attachments;
        }

        /// <summary>
        /// Name of service that generated mail
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Mail from e-mail address
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Mail to e-mail address
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Mail carbon copies e-mail addresses
        /// </summary>
        public string CcTo { get; set; }

        /// <summary>
        /// Mail from e-mail address
        /// </summary>        
        public string BccTo { get; set; }

        /// <summary>
        /// Reply to list for specific message
        /// </summary>        
        public string ReplyTo { get; set; }


        /// <summary>
        /// Custom message headers for email
        /// </summary>        
        public string MessageHeaders { get; set; }

        /// <summary>
        /// Additional recipients addresses
        /// </summary>        
        public string AdditionalRecipientsTo { get; set; }

        /// <summary>
        /// Mail title
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Mail body
        /// </summary>
        public string PlainTextBody { get; set; }

        /// <summary>
        /// Mail body in HTML format
        /// </summary>
        public string HtmlBody { get; set; }

        /// <summary>
        /// Is mail in HTML format
        /// </summary>
        public bool IsHtml { get; set; }

        public string Body => IsHtml ? HtmlBody : PlainTextBody;

        public IList<MailQueueItemAttachment> Attachments { get; private set; }
    }
}
