using Microsoft.AspNetCore.DataProtection;

namespace WebFlsQuiz.Models
{
    public class ProtectedConfiguration
    {
        private string _IP;

        private string _port;

        private string _token;

        private IDataProtector _dataProtector;

        public ProtectedConfiguration(IDataProtectionProvider provider, string ip, string port, string token)
        {
            _dataProtector = provider.CreateProtector("Protected configuration");
            _IP = _dataProtector.Protect(ip);
            _port = _dataProtector.Protect(port);
            _token = _dataProtector.Protect(token);
        }

        public Configuration Unprotect()
        {
            return new Configuration
            {
                IP = _dataProtector.Unprotect(_IP),
                Port = _dataProtector.Unprotect(_port),
                Token = _dataProtector.Unprotect(_token),
            };
        }
    }
}
