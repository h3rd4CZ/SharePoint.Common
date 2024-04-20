namespace RhDev.SharePoint.Common.Composition.Factory
{
    public interface IApplicationContainerSetup
    {
        IApplicationContainer Frontend { get; }
        IApplicationContainer Backend { get; }
    }
}
