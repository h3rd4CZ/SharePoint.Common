using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.DataAccess.Mail;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Extensions;
using RhDev.SharePoint.Common.Logging;
using RhDev.SharePoint.Common.Mail;
using RhDev.SharePoint.Common.Notifications;
using RhDev.SharePoint.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace RhDev.SharePoint.Common.Impl.Notifications
{
    public class NotificationSender : ServiceBase, INotificationSender
    {
        private readonly IMailingFactory mailingFactory;
        private readonly FarmConfiguration farmConfiguration;

        protected override TraceCategory TraceCategory => TraceCategories.Common;

        public NotificationSender(
            IMailingFactory mailingFactory,
            ITraceLogger traceLogger,
            FarmConfiguration farmConfiguration
            ) : base(traceLogger)
        {
            this.mailingFactory = mailingFactory;
            this.farmConfiguration = farmConfiguration;
        }
                
        public IList<Notification> SendNotifications(SectionDesignation sectionDesignation, IList<Notification> notifications)
        {
            return SendNotifications(sectionDesignation, notifications, null, null);
        }
        public IList<Notification> SendNotifications(SectionDesignation sectionDesignation, IList<Notification> notifications, Action<Notification, MailQueueItem> notifySuccessAction, Action<Notification, Exception> notifyFailedAction)
        {
            WriteTrace($"Sending notifications started...");

            var failedNotifications = new Dictionary<Notification, Exception>();

            var isRedirected = !string.IsNullOrWhiteSpace( farmConfiguration.NotificationRedirect);

            using (IMailSender mailSender =  mailingFactory.CreateMailSender(sectionDesignation.Address))
            {
                foreach (Notification notification in notifications)
                {
                    WriteTrace("Processing notification for user {0}", notification.User);

                    SendNotification(notification, failedNotifications, mailSender, notifySuccessAction,
                        notifyFailedAction, isRedirected);

                    WriteTrace("Notification processed");
                }
            }

            HandleFailedNotifications(failedNotifications);

            WriteTrace($"Completed sending notifications");

            return failedNotifications.Select(n => n.Key).ToList();
        }


        private void SendNotification(Notification notification, IDictionary<Notification, Exception> failedNotifications, IMailSender mailSender, Action<Notification, MailQueueItem> notifySuccessLogger, Action<Notification, Exception> notifyFailedLogger, bool isRedirected)
        {
            try
            {
                Guard.NotNull(notification.User, nameof(notification.User));

                if (!isRedirected && !notification.User.HasEmail)
                {
                    WriteTrace("User {0} does not have e-mail filled, notification will not be sent", notification.User);

                    throw new NotificationSendingFailedException(
                        $"User {notification.User} does not have e-mail filled, notification will not be sent");
                }

                MailQueueItem mail = PrepareEmail(notification);

                WriteTrace("Sending notification");

                mailSender.Send(mail);

                WriteTrace("Notification sent successfully");

                notifySuccessLogger?.Invoke(notification, mail);


            }
            catch (Exception e)
            {
                WriteUnexpectedTrace(e, "Failed to send notification to user {0}", notification.User);

                notifyFailedLogger?.Invoke(notification, e);

                failedNotifications.Add(notification, e);
            }
        }

        private MailQueueItem PrepareEmail(Notification notification)
        {
            
            string subject = notification.Subject;
            string body = notification.MailContent;

            var redirectedRecipient = farmConfiguration.NotificationRedirect;
            string redicretedRecipientValue = default;
            if (!string.IsNullOrWhiteSpace(redirectedRecipient))
            {
                var isValidMail = redirectedRecipient.IsValidEmailAddress();
                if (!isValidMail) throw new InvalidOperationException($"Redirect email address is not valid email : {redirectedRecipient}");
                redicretedRecipientValue = redirectedRecipient;
            }

            subject = subject ?? string.Empty;
            
            var mail = new MailQueueItem(
                redicretedRecipientValue ?? notification.User.Email,
                !Equals(null, redirectedRecipient) 
                    ? $"{subject} [redirected mail, original user : {(string.IsNullOrWhiteSpace(notification.User.Email) ? notification.User.Name : notification.User.Email)}]" 
                    : subject)
            { 
                IsHtml = true 
            };
                        

            mail.HtmlBody = body;
            mail.AdditionalRecipientsTo = notification.AdditionalRecipients;
            mail.CcTo = notification.CC;
            mail.BccTo = notification.BCC;
            mail.ReplyTo = notification.ReplyTo;
            mail.MessageHeaders = notification.MessageHeaders;            

            FillAttachments(notification, mail);

            WriteTrace("Prepared notification e-mail for user {0} with address {1} and subject {2}.", notification.User, mail.To, mail.Subject);
            if(!string.IsNullOrWhiteSpace(redicretedRecipientValue))
                WriteTrace("E-mail for user {0} will be redirected to address {1}.", notification.User, redicretedRecipientValue);

            return mail;
        }


        private void HandleFailedNotifications(IDictionary<Notification, Exception> failedNotifications)
        {
            if (failedNotifications.Count != 0) return;

            WriteTrace("All notifications sent succesfully");
        }

        private void FillAttachments(Notification notification, MailQueueItem mail)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));
            if (mail == null) throw new ArgumentNullException(nameof(mail));

            if (!Equals(null, notification.Attachments) && notification.Attachments.Count > 0) mail.IncludeAttachments(notification.Attachments);
        }
    }
}
