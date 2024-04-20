namespace RhDev.SharePoint.Common.Security
{
    public class GroupMapping
    {
        public GroupDefinitionBase Definition { get; private set; }

        public string[] MemberLoginNames { get; private set; }

        public GroupMapping(GroupDefinitionBase definition, params string[] memberLoginNames)
        {
            Definition = definition;
            MemberLoginNames = memberLoginNames;
        }
    }
}
