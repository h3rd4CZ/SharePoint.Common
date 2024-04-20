using RhDev.SharePoint.Common.Caching.Composition;

namespace RhDev.SharePoint.Common.Mail
{
    public interface IMailConfigurationProvider : IService
    {
        MailConfiguration GetMailConfiguration();
    }
}
