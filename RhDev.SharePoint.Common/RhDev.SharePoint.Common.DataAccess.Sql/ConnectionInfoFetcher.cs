using RhDev.SharePoint.Common.DataAccess.SharePoint.Configuration.Objects;
using RhDev.SharePoint.Common.DataAccess.SQL;
using System;

namespace RhDev.SharePoint.Common.DataAccess.Sql
{
    public class ConnectionInfoFetcher : IConnectionInfoFetcher
    {        
        private readonly string connectionString;

        private readonly GlobalConfiguration _globalConfiguration;

        public ConnectionInfoFetcher(GlobalConfiguration globalConfiguration)
        {
            _globalConfiguration = globalConfiguration;

            var connString = globalConfiguration.ConnectionString;

            if (string.IsNullOrEmpty(connString))
                throw new InvalidOperationException(
                    "Connection string has not been set, please set connection string first");

            connectionString = connString;
        }

        public string GetConnectionInfo(string parameter = default)
        {
            return connectionString;
        }

    }
}
