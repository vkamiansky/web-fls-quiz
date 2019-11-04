using WebFlsQuiz.Models;
using System.Threading.Tasks;

namespace WebFlsQuiz.Interfaces
{
    public interface IMailService
    {
        IOperationResult SendResults(string email, string name, string comment, UserResult result, string quizName);
        IOperationResult SendConfirmCode(ConfirmationRequestData data);
    }
}
