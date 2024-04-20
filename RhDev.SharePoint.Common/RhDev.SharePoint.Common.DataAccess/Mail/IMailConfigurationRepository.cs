using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.Mail;

namespace RhDev.SharePoint.Common.DataAccess.Mail
{
    public interface IMailConfigurationRepository : IService
    {
        MailConfiguration GetMailConfiguration();
    }
}
