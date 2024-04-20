using System;

namespace RhDev.SharePoint.Common.Security
{
    class ApplicationAdministratorGroup : ApplicationGroup
    {
        public override string Name => "Admin";

        public override string Description => "Administrátor aplikace";
    }
}
