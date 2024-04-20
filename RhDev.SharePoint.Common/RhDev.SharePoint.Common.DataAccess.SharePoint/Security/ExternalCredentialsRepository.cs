using System;
using System.Collections.Generic;
using RhDev.SharePoint.Common.DataAccess.Security;
using Microsoft.SharePoint;
using System.Net;
using System.Security;
using System.Runtime.InteropServices;
using RhDev.SharePoint.Common.Security;
using RhDev.SharePoint.Common.Caching.Composition;
using Microsoft.Office.SecureStoreService.Server;
using Microsoft.BusinessData.Infrastructure.SecureStore;
using RhDev.SharePoint.Common.Logging;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Security
{
    public class ExternalCredentialsRepository : IExternalCredentialsRepository
    {
        private readonly string webUrl;

        public ExternalCredentialsRepository(string webUrl)
        {
            this.webUrl = webUrl;
        }

        public string IssueTicket()
        {
            using (var site = new SPSite(webUrl))
            {
                var serviceContext = SPServiceContext.GetContext(site);
                var secureStoreProvider = new SecureStoreProvider { Context = serviceContext };

                return secureStoreProvider.IssueTicket();
            }
        }

        public NetworkCredential GetCredentials(string applicationId)
        {
            return GetCredentials(applicationId, null);            
        }

        public NetworkCredential GetCredentials(string applicationId, string ticket)
        {
            var credentialMap = new Dictionary<string, string>();

            using (var site = new SPSite(webUrl))
            {
                var serviceContext = SPServiceContext.GetContext(site);
                var secureStoreProvider = new SecureStoreProvider { Context = serviceContext };

                try
                {
                    using (var credentials = GetCredentialsCollection(secureStoreProvider, applicationId, ticket))
                        PopulateCredentialsMap(secureStoreProvider, credentials, applicationId, credentialMap);
                }
                catch (SecureStoreTargetApplicationNotFoundException e)
                {
                    throw new ExternalCredentialsNotFoundException(
                        String.Format("External credentials for application ID {0} not found.", applicationId), e);
                }
            }

            string userName = credentialMap["Windows User Name"];
            string domain = credentialMap["Windows Domain"];
            string password = credentialMap["Windows Password"];

            return new NetworkCredential(userName, password, domain);
        }

        private static SecureStoreCredentialCollection GetCredentialsCollection(SecureStoreProvider secureStoreProvider, string applicationId, string ticket)
        {
            return String.IsNullOrEmpty(ticket)
                       ? secureStoreProvider.GetCredentials(applicationId)
                       : secureStoreProvider.GetCredentialsUsingTicket(ticket, applicationId);
        }

        private static void PopulateCredentialsMap(SecureStoreProvider secureStoreProvider, SecureStoreCredentialCollection credentials, string applicationId, Dictionary<string, string> credentialMap)
        {
            var fields = secureStoreProvider.GetTargetApplicationFields(applicationId);

            for (var i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                var credential = credentials[i];
                var decryptedCredential = ExtractString(credential.Credential);

                credentialMap.Add(field.Name, decryptedCredential);
            }
        }

        private static string ExtractString(SecureString secureString)
        {
            var ptr = Marshal.SecureStringToBSTR(secureString);

            try
            {
                return Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                Marshal.FreeBSTR(ptr);
            }
        }
    }
}
