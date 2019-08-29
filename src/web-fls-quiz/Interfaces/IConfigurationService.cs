using System.Threading.Tasks;
using WebFlsQuiz.Models;

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
        Task<string> GetAdminEmail();
        Task<string> GetAdminEmail(Configuration configuration);
        Task<string> GetAdminEmailUsingNotConfirmedConfiguration();
        Task<string> GetIsConfigured();
        Task<bool> CheckConfiguration(Configuration configuration);
        string SetConfiguration(Configuration configuration);
        bool ConfirmConfiguration(string confirmCode);
        Task<MailSettings> GetMailSettingsUsingNotConfirmedConfiguration();
        Task<MailSettings> GetMailSettings();
    }
}
