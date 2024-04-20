using System;
using RhDev.SharePoint.Common.DataAccess.Repository;
using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.DataAccess.Repository.Entities;
using System.Linq;
using RhDev.SharePoint.Common.Utils;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Repository
{
    public class CommonRepositoryFactory : ICommonRepositoryFactory
    {
        private readonly FarmConfiguration _farmConfig;

        public CommonRepositoryFactory(FarmConfiguration farmConfig)
        {
            _farmConfig = farmConfig;
        }

        public IApplicationConfigurationRepository GetApplicationConfigurationRepository(string webUrl = null)
        {
            var rootSiteUrl = string.IsNullOrEmpty(webUrl) ? GetAppSiteUrl() : webUrl;

            return new ApplicationConfigurationRepository(rootSiteUrl);
        }

        public IApplicationLogRepository<LogItem> GetApplicationLogRepository(string webUrl = null)
        {
            var rootSiteUrl = string.IsNullOrEmpty(webUrl) ? GetAppSiteUrl() : webUrl;

            if (string.IsNullOrEmpty(rootSiteUrl)) throw new InvalidOperationException("App site url is not set");

            return new LogListRepository(rootSiteUrl);
        }

        public IDayOffRepository GetDayOffRepository(string webUrl = null)
        {
            var rootSiteUrl = string.IsNullOrEmpty(webUrl) ? GetAppSiteUrl() : webUrl;

            if (string.IsNullOrEmpty(rootSiteUrl)) throw new InvalidOperationException("App site url is not set");

            return new DayOffConfigurationRepository(rootSiteUrl);
        }

        public T GetRepository<T, TEntity>(string webUrl = null) where TEntity : EntityBase
        {
            CheckRepositoryType<TEntity>(typeof(T));

            return (T)Activator.CreateInstance(typeof(T), webUrl ?? GetAppSiteUrl());
        }

        public TRepository GetRepository<TRepository>(string webUrl) where TRepository : IRepository
        {
            Guard.StringNotNullOrWhiteSpace(webUrl, nameof(webUrl));

            Type repositoryType = GetRepositoryImplementationOf<TRepository>();

            Guard.NotNull(repositoryType, nameof(repositoryType), $"Repository implementation for type : {typeof(TRepository).Name} was not found");

            return (TRepository)Activator.CreateInstance(repositoryType, webUrl ?? GetAppSiteUrl());
        }

        public static Type GetRepositoryImplementationOf<T>() => GetRepositoryImplementationOf(typeof(T));

        public static Type GetRepositoryImplementationOf(Type typeRepository)
        {
            foreach (var ass in AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name.StartsWith(Constants.RhDevSOLUTIONS_PREFIX)))
            {
                var type = ass.GetTypes().FirstOrDefault(t => typeRepository.IsAssignableFrom(t) && !t.IsAbstract && !Equals(null, t.BaseType));

                if (!Equals(null, type))
                {
                    if (typeof(RepositoryBase).IsAssignableFrom(type)) return type;
                }
            }

            return null;
        }

        private void CheckRepositoryType<TEntity>(Type type) where TEntity : EntityBase
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if(!type.IsClass) throw  new InvalidOperationException("Type must be a class");

            if(!typeof(IEntityRepository<TEntity>).IsAssignableFrom(type)) throw new InvalidOperationException("Type is not IEntityRepository");
        }

        private string GetAppSiteUrl()
        {
            return _farmConfig.AppSiteUrl;
        }
    }
}
