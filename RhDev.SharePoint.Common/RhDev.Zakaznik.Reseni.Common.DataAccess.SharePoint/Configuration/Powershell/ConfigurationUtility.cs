using RhDev.SharePoint.Common.Configuration;
using $ext_safeprojectname$.Common.Setup;

namespace $ext_safeprojectname$.Common.DataAccess.SharePoint.Configuration.Powershell
{
    public class ConfigurationUtility
    {
        public Objects.GlobalConfiguration GlobalConfiguration { get; set; }

        public FarmConfiguration FarmConfiguration { get; set; }

        public ConfigurationUtility()
        {
            FarmConfiguration = IoC.Get.Backend.GetInstance<FarmConfiguration>();

            GlobalConfiguration = IoC.Get.Backend.GetInstance<Objects.GlobalConfiguration>();

        }
    }
}
