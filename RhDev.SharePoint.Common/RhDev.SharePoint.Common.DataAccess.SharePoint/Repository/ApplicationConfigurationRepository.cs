using RhDev.SharePoint.Common.DataAccess.Repository;
using RhDev.SharePoint.Common.DataAccess.Repository.Entities;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Repository.CAML;
using Microsoft.SharePoint;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Repository
{
    public class ApplicationConfigurationRepository : EntityRepositoryBase<ApplicationConfiguration>, IApplicationConfigurationRepository
    {
        protected override bool RequiresElevation => true;

        public ApplicationConfigurationRepository(string webUrl)
            : base(webUrl, ListFetcher.ForRelativeUrl(Config.Lists.APPCONFIGURL))
        {
        }

        protected override void LoadData(SPListItem listItem, ApplicationConfiguration entity)
        {
            base.LoadData(listItem, entity);

            entity.Key = (string)listItem[Config.Fields.APPCONFIGKEY];
            entity.Value = (string)listItem[Config.Fields.APPCONFIGVAL];
            entity.Module = (string)listItem[Config.Fields.APPCONFIGMODULE];
        }

        protected override void CreateData(SPListItem listItem, ApplicationConfiguration entity)
        {
            base.CreateData(listItem, entity);
            CreateOrUpdateData(listItem, entity);
        }

        protected override void UpdateData(SPListItem listItem, ApplicationConfiguration entity)
        {
            base.UpdateData(listItem, entity);
            CreateOrUpdateData(listItem, entity);
        }

        private void CreateOrUpdateData(SPListItem listItem, ApplicationConfiguration entity)
        {
            listItem[Config.Fields.APPCONFIGKEY] = entity.Key;
            listItem[Config.Fields.APPCONFIGVAL] = entity.Value;
            listItem[Config.Fields.APPCONFIGMODULE] = entity.Module;
        }

        public void CreateConfiguration(ApplicationConfiguration configuration)
        {
            Create(configuration);
        }

        public void UpdateConfiguration(ApplicationConfiguration configuration)
        {
            Update(configuration);
        }

        public ApplicationConfiguration GetByKey(string key)
        {
            CAMLFilter filter = CAMLFilters.Equal(Config.Fields.APPCONFIGKEY, key, CAMLType.Text);
            SPQuery query = CAMLQueryBuilder.BuildQuery(filter);

            return QueryEntity(query);
        }
    }
}
