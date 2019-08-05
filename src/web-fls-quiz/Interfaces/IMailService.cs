using WebFlsQuiz.Models;
using System.Threading.Tasks;

namespace WebFlsQuiz.Interfaces
{
    public interface IMailService
    {
        Task SendResults(string email, string name, string comment, UserResult result);
    }
}
