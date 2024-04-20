using RhDev.SharePoint.Common.Caching.Composition;

namespace RhDev.SharePoint.Common.Notifications
{
    public interface INotificationTemplateFactory : IAutoRegisteredService
    {
        INotificationTemplate GetTemplate(Notification notification);
    }
}
