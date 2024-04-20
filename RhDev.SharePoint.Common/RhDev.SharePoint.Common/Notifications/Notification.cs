using RhDev.SharePoint.Common.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Xml.Serialization;

namespace RhDev.SharePoint.Common.Notifications
{
    public class Notification
    {
        [XmlIgnore]
        public UserInfo User { get; set; }
        
        public CentralClock CompilationDate { get; set; }
        public string AdditionalRecipients { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string ReplyTo { get; set; }
        public string MessageHeaders { get; set; }
        public string MailContent { get; set; }
        public IList<MailQueueItemAttachment> Attachments { get; }
        public string Subject
        {
            get;
        }

        public string AdditionalHeader { get; set; }
        public bool IsValid => Validate();

        public Notification()
        {
            
        }

        public Notification(UserInfo user,
            CentralClock compilationDate, string mailContent, string additionalRecipients, string cc, string bcc,
            string replyTo, string messageHeaders, IList<MailQueueItemAttachment> attachments = null, string subject = null)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            User = user;
            CompilationDate = compilationDate;
            Subject = subject;
            AdditionalRecipients = additionalRecipients;
            CC = cc;
            BCC = bcc;
            ReplyTo = replyTo;
            MessageHeaders = messageHeaders;
            MailContent = mailContent;
            Attachments = attachments;
        }

        public virtual INotificationTemplate GetTemplate(INotificationTemplateFactory notificationTemplateFactory)
        {
            throw new NotImplementedException();

            //return notificationTemplateFactory.GetTemplate(this);
        }       
        
        protected virtual bool Validate() => true;
    }
}
