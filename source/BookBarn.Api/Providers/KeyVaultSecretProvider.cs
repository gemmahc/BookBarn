using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Security.KeyVault.Secrets;

namespace BookBarn.Api.Providers
{
    public class KeyVaultSecretProvider : KeyVaultSecretManager
    {
        private string _environment;

        public KeyVaultSecretProvider(string environment)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(environment);

            _environment = $"{environment}-";
        }

        public override bool Load(SecretProperties secret)
        {
            // Filter out expired.
            if (secret.ExpiresOn.HasValue && secret.ExpiresOn.Value < DateTimeOffset.UtcNow)
            {
                return false;
            }

            // Return only prefixed with this environment.
            return secret.Name.StartsWith(_environment);
        }

        public override string GetKey(KeyVaultSecret secret)
        {
            // Replace KeyVault double-dash delimiter with Config provider colon delimiter
            return secret.Name[_environment.Length..].Replace("--", ConfigurationPath.KeyDelimiter);
        }
    }
}
