using Microsoft.SqlServer.Server;
using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.Logging;

namespace RhDev.SharePoint.Common.DataAccess.Mail
{

    public interface IMailingFactory : IAutoRegisteredService
    {
               

        /// <summary>
        /// Je nutné obalit do usingu.
        /// </summary>
        /// <returns></returns>
        IMailSender CreateMailSender(string appSiteUrl);

        IMailSender CreateSPMailSender(SectionDesignation sectionDesignation);

        //IMailConfigurationProvider CreateMailConfigurationProvider();
    }

}
