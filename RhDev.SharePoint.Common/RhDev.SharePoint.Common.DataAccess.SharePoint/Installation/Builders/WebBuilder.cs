using Microsoft.SharePoint;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Repository;
using RhDev.SharePoint.Common.Utils;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Installation.Builders
{
    public class WebBuilder : ModificationBuilder<SPWeb>
    {
        public WebBuilder(string webUrl) : base(webUrl)
        {
            
        }

        public WebBuilder AddRoleDefinition(string roleName, SPBasePermissions basePermissions)
        {
            Guard.StringNotNullOrWhiteSpace(roleName, nameof(roleName));

            AddModification((w1, w2) =>
            {
                SPRoleDefinition rd = null;
                try
                {
                    rd = w1.RoleDefinitions[roleName];
                }
                catch{}

                if (!Equals(null, rd)) return false;

                SPRoleDefinition roleDefinition = new SPRoleDefinition();
                roleDefinition.BasePermissions = basePermissions;
                roleDefinition.Name = roleName;

                w1.RoleDefinitions.Add(roleDefinition);

                return true;
            });

            return this;
        }

        public WebBuilder AddReadPermToAllAuthenticated()
        {
            AddModification((w1, w2) =>
            {
                var aaUsrs = w1.EnsureUser("c:0(.s|true");
                if (!w1.HasUniqueRoleAssignments)
                    w1.BreakRoleInheritance(true);

                var ra = new SPRoleAssignment(aaUsrs);
                ra.RoleDefinitionBindings.Add(w1.RoleDefinitions.GetByType(SPRoleType.Reader));

                w1.RoleAssignments.Add(ra);

                return true;
            });

            return this;
        }

        protected override SPWeb PrepareObject(SPWeb web)
        {
            return web;
        }

        protected override void SaveObject(SPWeb obj)
        {
            obj.Update();
        }

        public WebBuilder ModifyQL()
        {
            AddModification((web, w) =>
            {
                return true;
            });

            return this;
        }
    }
}
