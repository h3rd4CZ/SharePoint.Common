using RhDev.SharePoint.Common.Caching.Composition;

namespace RhDev.SharePoint.Common.Notifications
{
    public interface INotificationTemplate : IService
    {
        bool OutputsHtmlBody { get; }
        string Subject { get; }
        string FormatBody(Notification notification);
    }
}
