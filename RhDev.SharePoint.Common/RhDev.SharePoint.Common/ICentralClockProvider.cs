using RhDev.SharePoint.Common.Caching.Composition;

namespace RhDev.SharePoint.Common
{
    public interface ICentralClockProvider : IAutoRegisteredService
    {
        CentralClock Now();
    }
}
