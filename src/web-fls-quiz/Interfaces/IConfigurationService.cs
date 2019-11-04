using System.Threading.Tasks;
using WebFlsQuiz.Models;

namespace WebFlsQuiz.Interfaces
{
    public interface IConfigurationService
    {
        Task<IOperationResult<ConfirmationRequestData>> ProcessConfigurationChangeRequest(string vaultIp, string vaultPort, string vaultToken);
        IOperationResult ConfirmConfigurationChange(string code);
        IOperationResult<string> GetString(ConfigurationKey key);
    }
}
