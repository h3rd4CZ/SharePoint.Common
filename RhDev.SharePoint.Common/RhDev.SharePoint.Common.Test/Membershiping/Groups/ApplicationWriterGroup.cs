using System;

namespace RhDev.SharePoint.Common.Test.Membershiping.Groups
{
    public class ApplicationWriterGroup : ApplicationSolutionGroup
    {
        public static string GroupNameCustom = "Application writer custom";
        public override string Name => "Writer";

        public override string Description => "Writer can write";

        public override Func<string, string> CustomNameProvider => webTitle => GroupNameCustom;
    }
}
