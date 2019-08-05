using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebFlsQuiz.Interfaces
{
    public interface IConfigurationService
    {
        Task<string> GetMailingAccountLogin();
        Task<string> GetMailingAccountPassword();
        Task<string> GetMailingSmtpHost();
        Task<int> GetMailingSmtpPort();
        Task<string> GetDbConnectionString();
        Task<string> GetDbName();
        Task<string> GetIsConfigured();
    }
}
