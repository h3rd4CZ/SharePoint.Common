using System;
using RhDev.SharePoint.Common.DataAccess.Repository;
using Microsoft.SharePoint;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Repository
{
    public class WebFileRepository : IWebFileRepository
    {

        public DataAccess.Repository.Entities.SolutionFileInfo GetWebFile(string webFileAbsoluteUrl)
        {
            if (string.IsNullOrEmpty(webFileAbsoluteUrl)) throw new ArgumentNullException("webFileAbsoluteUrl");

            using (SPSite site = new SPSite(webFileAbsoluteUrl))
            using (SPWeb web = site.OpenWeb())
            {
                var file = web.GetFile(webFileAbsoluteUrl);
                if (!Equals(null, file) && file.Exists)
                {
                    return new DataAccess.Repository.Entities.SolutionFileInfo()
                    {
                        FileName = file.Name,
                        Blob = file.OpenBinary()
                    };
                }
            }

            throw new InvalidOperationException(string.Format("File at {0} doesnt exist", webFileAbsoluteUrl));
        }
    }
}
