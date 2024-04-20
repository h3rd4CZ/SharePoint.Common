namespace RhDev.SharePoint.Common
{
    public interface IPrincipalInfo
    {
        int Id { get; }
        string Name { get; }
        string DisplayName { get; }
        bool IsValid { get; }
        bool IsDomainGroup { get; }
        bool Isgroup { get; }
        bool IsExternal { get; }
    }
}
