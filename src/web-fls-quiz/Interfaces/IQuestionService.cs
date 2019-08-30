using System.Threading.Tasks;
using WebFlsQuiz.Models;

namespace WebFlsQuiz.Interfaces
{
    public interface IQuestionService
    {
        Task<Question> GetRandom(int[] excludedQuestionIds, string quizName);

        Task<UserResult> GetUserResult(UserAnswer[] userAnswers, string quizName);
    }
}
