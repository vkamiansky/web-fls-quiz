using Microsoft.AspNetCore.DataProtection;

namespace WebFlsQuiz.Models
{
    public class Configuration
    {
        public string IP { get; set; }

        public string Port { get; set; }

        public string Token { get; set; }

        public ProtectedConfiguration Protect(IDataProtectionProvider provider)
        {
            return new ProtectedConfiguration(provider, IP, Port, Token);
        }
    }
}
