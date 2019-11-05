using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;
using WebFlsQuiz.Interfaces;
using WebFlsQuiz.Models;

namespace WebFlsQuiz.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private const int MINUTES_CONFIRMATION_CODE_VALID = 5;
        private const int MINUTES_BETWEEN_CONFIGURATION_CHANGE_REQUESTS = 2;
        private Dictionary<ConfigurationKey, string> _currentProtectedConfiguration = new Dictionary<ConfigurationKey, string>();
        private Dictionary<ConfigurationKey, string> _candidateProtectedConfiguration = new Dictionary<ConfigurationKey, string>();
        private IDataProtector _dataProtector;
        private DateTime? _confirmationCodeIssuedAt;
        private DateTime? _lastConfigurationChangeRequestAt;
        private string _confirmationCodeProtected;
        public ConfigurationService(IDataProtectionProvider provider)
        {
            _dataProtector = provider.CreateProtector("Protected Configuration");
        }
        public async Task<IOperationResult<ConfirmationRequestData>> ProcessConfigurationChangeRequest(
            string vaultIp,
            string vaultPort,
            string vaultToken)
        {
            if (_lastConfigurationChangeRequestAt.HasValue
                    && _lastConfigurationChangeRequestAt.Value.AddMinutes(MINUTES_BETWEEN_CONFIGURATION_CHANGE_REQUESTS) > DateTime.Now)
                return OperationResult.UserError<ConfirmationRequestData>($"Please, wait. The interval between configuration requests is: {MINUTES_BETWEEN_CONFIGURATION_CHANGE_REQUESTS}");
            else
                _lastConfigurationChangeRequestAt = DateTime.Now;

            return await
                OperationResult
                    .Try(() => CreateVaultClient(vaultIp, vaultPort, vaultToken))
                    .BindAsync(async (x) => await ReadConfiguration(x))
                    .Bind(candidateProtectedConfiguration =>
                    {
                        // A new configuration has been read. We set it as a candidate.
                        _candidateProtectedConfiguration = candidateProtectedConfiguration;

                        // The email is stored in protected form. If it's been set already, we use the current one, otherwise the candidate one
                        var requestFields = new[]
                        {
                            ConfigurationKey.AdminEmail,
                            ConfigurationKey.SmtpHost,
                            ConfigurationKey.SmtpPort,
                            ConfigurationKey.MailingAccountLogin,
                            ConfigurationKey.MailingAccountPassword
                        };

                        var actingProtectedConfiguration = _currentProtectedConfiguration.Keys.Intersect(requestFields).Count() == requestFields.Length
                        ? _currentProtectedConfiguration
                        : _candidateProtectedConfiguration;

                        // Generate and set the confirmation code parameters
                        var confirmationCode = Guid.NewGuid().ToString();
                        _confirmationCodeProtected = _dataProtector.Protect(confirmationCode);
                        _confirmationCodeIssuedAt = DateTime.Now;

                        var requestData = new ConfirmationRequestData
                        {
                            ConfirmationCode = confirmationCode,
                            AdminEmail = _dataProtector.Unprotect(actingProtectedConfiguration[ConfigurationKey.AdminEmail]),
                            SmtpHost = _dataProtector.Unprotect(actingProtectedConfiguration[ConfigurationKey.SmtpHost]),
                            SmtpPort = _dataProtector.Unprotect(actingProtectedConfiguration[ConfigurationKey.SmtpPort]),
                            MailingAccountLogin = _dataProtector.Unprotect(actingProtectedConfiguration[ConfigurationKey.MailingAccountLogin]),
                            MailingAccountPassword = _dataProtector.Unprotect(actingProtectedConfiguration[ConfigurationKey.MailingAccountPassword]),
                        };
                        return OperationResult.Success(requestData);
                    });
        }
        public IOperationResult ConfirmConfigurationChange(string code)
        {
            var correctCode = _dataProtector.Unprotect(_confirmationCodeProtected);
            // Someone is trying to break in
            if (!_confirmationCodeIssuedAt.HasValue || correctCode != code)
                return OperationResult.UserError("Wrong code!");
            // The right person is just too late
            if (correctCode == code && _confirmationCodeIssuedAt.Value.AddMinutes(MINUTES_CONFIRMATION_CODE_VALID) < DateTime.Now)
                return OperationResult.UserError("Confirmation code has expired. Please, request another one.");
            // Let's clear the current configuration
            _currentProtectedConfiguration = new Dictionary<ConfigurationKey, string>();
            // Fill it with new protected values
            foreach (var key in _candidateProtectedConfiguration.Keys)
                _currentProtectedConfiguration[key] = _candidateProtectedConfiguration[key];
            _confirmationCodeIssuedAt = null;
            _candidateProtectedConfiguration = new Dictionary<ConfigurationKey, string>();
            return OperationResult.Success();
        }
        public IOperationResult<string> GetString(ConfigurationKey key)
        {
            if (_currentProtectedConfiguration.TryGetValue(key, out var protectedResult))
            {
                var result = _dataProtector.Unprotect(protectedResult);
                // We disallow blank configuration keys. They breech security.
                if (string.IsNullOrWhiteSpace(result))
                    return OperationResult.Failure<string>(new Exception($"Configuration value is blank for key: {key}."));
                return OperationResult.Success(result);
            }
            return OperationResult.Failure<string>(new Exception($"Configuration missing for key: {key}."));
        }
        private async Task<IOperationResult<Dictionary<ConfigurationKey, string>>> ReadConfiguration(VaultClient client)
        {
            var vaultToLocalKeys = new[]
                {("admin_email", ConfigurationKey.AdminEmail),
                ("email_login", ConfigurationKey.MailingAccountLogin),
                ("email_password", ConfigurationKey.MailingAccountPassword),
                ("smtp_address", ConfigurationKey.SmtpHost),
                ("smtp_port", ConfigurationKey.SmtpPort),
                ("mongo_connection_string", ConfigurationKey.DbConnectionString),
                ("mongo_db_name", ConfigurationKey.DbName)};

            var res = OperationResult.Success(new Dictionary<ConfigurationKey, string>());
            foreach (var key in vaultToLocalKeys)
            {
                res = await res.Merge(() => ReadSecret(client, key.Item1), (a, x) =>
                {
                    a[key.Item2] = _dataProtector.Protect(x);
                    return OperationResult.Success(a);
                });
            }
            return res;
        }
        private IOperationResult<VaultClient> CreateVaultClient(string ip, string port, string token)
        {
            IAuthMethodInfo authMethod = new TokenAuthMethodInfo(token);
            var vaultClientSettings = new VaultClientSettings($"http://{ip}:{port}", authMethod);
            return OperationResult.Success(new VaultClient(vaultClientSettings));
        }
        private async Task<IOperationResult<string>> ReadSecret(VaultClient client, string path)
        {
            Secret<SecretData> secret = await client.V1.Secrets.KeyValue.V2.ReadSecretAsync(path);
            return OperationResult.Success(secret.Data.Data["CURRENT"] as string);
        }
    }
}