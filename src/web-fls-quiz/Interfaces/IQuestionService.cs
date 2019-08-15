using WebFlsQuiz.Models;

namespace WebFlsQuiz.Interfaces
{
    public interface IQuestionService
    {
        Question GetRandom(int[] excludedQuestionIds, string quizName);

        UserResult GetUserResult(UserAnswer[] userAnswers, string quizName);
    }
}
