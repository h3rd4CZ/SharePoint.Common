using RhDev.SharePoint.Common.Caching.Composition;

namespace RhDev.SharePoint.Common.Mail
{
    public interface IMailFormatter : IAutoRegisteredService
    {
        string Format(string template, object input);
    }
}
