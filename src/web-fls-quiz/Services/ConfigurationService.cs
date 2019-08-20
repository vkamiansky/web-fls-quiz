using WebFlsQuiz.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;
using Microsoft.AspNetCore.DataProtection;
using WebFlsQuiz.Models;

namespace WebFlsQuiz.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IDataProtectionProvider _protectionProvider;

        private ProtectedConfiguration _notConfirmedConfiguration;

        private ProtectedConfiguration _configuration;

        private string _confirmCode;

        private Exception _VaultClientError;

        public ConfigurationService(IDataProtectionProvider provider)
        {
            _protectionProvider = provider;
        }

        private async Task<string> ReadSecret(string path)
        {
            try
            {
                Secret<SecretData> secret = await CreateVaultClient().V1.Secrets.KeyValue.V2.ReadSecretAsync(path);
                return secret.Data.Data["CURRENT"] as string;
            }
            catch
            {
                return null;
            }
        }

        public async Task<MailSettings> GetMailSettings()
        {
            try
            {
                var client = CreateVaultClient();
                return await GetMailSettings(client);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<MailSettings> GetMailSettingsUsingNotConfirmedConfiguration()
        {
            try
            {
                var client = CreateVaultClientUsingNotConfirmedConfiguration();
                return await GetMailSettings(client);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<MailSettings> GetMailSettings(VaultClient client)
        {
            try
            {
                Secret<SecretData> login = await client.V1.Secrets.KeyValue.V2.ReadSecretAsync("email_login");
                Secret<SecretData> password = await client.V1.Secrets.KeyValue.V2.ReadSecretAsync("email_password");
                Secret<SecretData> address = await client.V1.Secrets.KeyValue.V2.ReadSecretAsync("smtp_address");
                Secret<SecretData> port = await client.V1.Secrets.KeyValue.V2.ReadSecretAsync("smtp_port");

                return new MailSettings
                {
                    Address = address.Data.Data["CURRENT"] as string,
                    Login = login.Data.Data["CURRENT"] as string,
                    Password = password.Data.Data["CURRENT"] as string,
                    Port = int.Parse(port.Data.Data["CURRENT"] as string)
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> GetMailingAccountLogin()
        {
            return await ReadSecret("email_login");
        }

        public async Task<string> GetMailingAccountPassword()
        {
            return await ReadSecret("email_password");
        }

        public async Task<string> GetMailingSmtpHost()
        {
            return await ReadSecret("smtp_address");
        }

        public async Task<int> GetMailingSmtpPort()
        {
            return int.Parse(await ReadSecret("smtp_port"));
        }

        public async Task<string> GetDbConnectionString()
        {
            return await ReadSecret("mongo_connection_string");
        }

        public async Task<string> GetDbName()
        {
            return await ReadSecret("mongo_db_name");
        }

        public async Task<string> GetIsConfigured()
        {
            try
            {
                var vault = CreateVaultClient();

                Secret<SecretData> smtpAddress = await vault.V1.Secrets.KeyValue.V2.ReadSecretAsync("smtp_address");
                Secret<SecretData> smtpPort = await vault.V1.Secrets.KeyValue.V2.ReadSecretAsync("smtp_port");
                Secret<SecretData> emailLogin = await vault.V1.Secrets.KeyValue.V2.ReadSecretAsync("email_login");
                Secret<SecretData> emailPassword = await vault.V1.Secrets.KeyValue.V2.ReadSecretAsync("email_password");
                Secret<SecretData> dbConnectionString = await vault.V1.Secrets.KeyValue.V2.ReadSecretAsync("mongo_connection_string");
                Secret<SecretData> dbName = await vault.V1.Secrets.KeyValue.V2.ReadSecretAsync("mongo_db_name");
                Secret<SecretData> tracingHost = await vault.V1.Secrets.KeyValue.V2.ReadSecretAsync("jaeger_address");
                Secret<SecretData> tracingPort = await vault.V1.Secrets.KeyValue.V2.ReadSecretAsync("jaeger_port");

                var configValues = new[] { smtpAddress, smtpPort, emailLogin, emailPassword, dbConnectionString, dbName, tracingHost, tracingPort };
                var result = configValues.Any(x => !x.Data.Data.ContainsKey("CURRENT"));
                return result ? "Vault value empty" : string.Empty;
            }
            catch (Exception e)
            {
                return new String(e.StackTrace + "\n" + _VaultClientError?.StackTrace ?? string.Empty);
            }
        }

        private VaultClient CreateVaultClient(string secretIp, string secretPort, string secretToken)
        {
            try
            {
                IAuthMethodInfo authMethod = new TokenAuthMethodInfo(secretToken);
                var vaultClientSettings = new VaultClientSettings($"http://{secretIp}:{secretPort}", authMethod);
                return new VaultClient(vaultClientSettings);
            }
            catch (Exception e)
            {
                _VaultClientError = e;
                return null;
            }
        }

        private VaultClient CreateVaultClient(Configuration configuration)
        {
            return CreateVaultClient(configuration.IP, configuration.Port, configuration.Token);
        }

        private VaultClient CreateVaultClient()
        {
            if (_configuration == null)
                return null;

            var configuration = _configuration.Unprotect();
            return CreateVaultClient(configuration);
        }

        private VaultClient CreateVaultClientUsingNotConfirmedConfiguration()
        {
            if (_notConfirmedConfiguration == null)
                return null;

            var configuration = _notConfirmedConfiguration.Unprotect();
            return CreateVaultClient(configuration);
        }

        public bool CheckConfiguration(Configuration configuration)
        {
            try
            {
                var client = CreateVaultClient(configuration.IP, configuration.Port, configuration.Token);
                var secret = client.V1.Secrets.KeyValue.V2.ReadSecretAsync("check_vault").Result;
                return (secret.Data.Data["CURRENT"] as string) == "Online";
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string SetConfiguration(Configuration configuration)
        {
            _notConfirmedConfiguration = configuration.Protect(_protectionProvider);
            _confirmCode = Guid.NewGuid().ToString();
            return _confirmCode;
        }

        public bool ConfirmConfiguration(string confirmCode)
        {
            if (string.Equals(confirmCode, _confirmCode))
            {
                _configuration = _notConfirmedConfiguration;
                return true;
            }
            return false;
        }
    }
}
