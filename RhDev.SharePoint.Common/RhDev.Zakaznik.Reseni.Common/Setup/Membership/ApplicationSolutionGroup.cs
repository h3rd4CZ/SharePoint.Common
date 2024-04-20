using RhDev.SharePoint.Common.Security;

namespace $ext_safeprojectname$.Common.Setup.Membership
{
    public abstract class ApplicationSolutionGroup : ApplicationGroup 
    {
        public override string ApplicationName => Const.SOLUTION_DISPLAY;
    }
}
