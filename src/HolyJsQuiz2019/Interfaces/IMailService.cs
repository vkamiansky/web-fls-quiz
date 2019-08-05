using HolyJsQuiz2019.Models;
using System.Threading.Tasks;

namespace HolyJsQuiz2019.Interfaces
{
    public interface IMailService
    {
        Task SendResults(string email, string name, string comment, UserResult result);
    }
}
