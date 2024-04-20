using System;
using System.Collections.Generic;
using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.Mail;

namespace RhDev.SharePoint.Common.Notifications
{
   public interface INotificationSender : IAutoRegisteredService
    {
        IList<Notification> SendNotifications(SectionDesignation sectionDesignation, IList<Notification> notifications, Action<Notification, MailQueueItem> notifySuccessLogger, Action<Notification, Exception> notifyFailedLogger);
        IList<Notification> SendNotifications(SectionDesignation sectionDesignation, IList<Notification> notifications);
    }
}
