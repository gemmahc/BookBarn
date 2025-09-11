using Azure.Core;
using Azure.Identity;
using Microsoft.Identity.Client;
using System.Security.Cryptography.X509Certificates;

namespace BookBarn.Api
{
    public class Configuration
    {
        private ConfigurationManager _configManager;
        private TokenCredential? _credential;

        public Configuration(ConfigurationManager configManager)
        {
            _configManager = configManager;
        }

        public string DataSourceConnectionString
        {
            get
            {
                return GetRequired("ConnectionStrings:DataSource");
            }
        }

        public string DataSourceDatabase
        {
            get
            {
                return GetRequired("DataSource:Database");
            }
        }

        public string DataSourceBookCollection
        {
            get
            {
                return GetRequired("DataSource:BookCollection");
            }
        }

        public string DataSourceGenreCollection
        {
            get
            {
                return GetRequired("DataSource:GenreCollection");
            }
        }

        public string MediaStoreConnectionString
        {
            get
            {
                return GetRequired("ConnectionStrings:MediaSource");
            }
        }

        public string MediaStoreContainer
        {
            get
            {
                return GetRequired("MediaSource:Container");
            }
        }

        public string KeyVaultName
        {
            get
            {
                return GetRequired("KeyVaultName");
            }
        }

        public TokenCredential ServiceCredential
        {
            get
            {
                if (_credential != null)
                {
                    return _credential;
                }

                string? aadThumbprint = _configManager["AzureADCertThumbprint"];
                string? aadDirectory = _configManager["AzureADDirectoryId"];
                string? aadAppId = _configManager["AzureADApplicationId"];

                if (string.IsNullOrEmpty(aadThumbprint) || string.IsNullOrEmpty(aadDirectory) || string.IsNullOrEmpty(aadAppId))
                {
                    throw new InvalidOperationException("Unable to generate credential without valid Azure identity configuration.");
                }

                using (var x509Store = new X509Store(StoreLocation.CurrentUser))
                {
                    x509Store.Open(OpenFlags.ReadOnly);

                    var x509Certificate = x509Store.Certificates
                        .Find(
                            X509FindType.FindByThumbprint,
                            aadThumbprint,
                            validOnly: false)
                        .OfType<X509Certificate2>()
                        .Single();

                    _credential = new ClientCertificateCredential(
                                                aadDirectory,
                                                aadAppId,
                                                x509Certificate);
                }

                return _credential;
            }
        }

        private string GetRequired(string key)
        {
            string? value = _configManager[key];

            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException($"Empty config value for required key [{key}]");
            }

            return value;
        }
    }
}
