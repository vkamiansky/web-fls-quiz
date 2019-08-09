using WebFlsQuiz.Models;

namespace WebFlsQuiz.Interfaces
{
    public interface IQuestionService
    {
        Question GetRandom(int[] excludedQuestionIds, int quizId);

        UserResult GetUserResult(UserAnswer[] userAnswers);
    }
}
