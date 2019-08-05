using WebFlsQuiz.Models;

namespace WebFlsQuiz.Interfaces
{
    public interface IQuestionService
    {
        Question GetRandom(int[] excludedQuestionsIds);

        UserResult GetUserResult(UserAnswer[] userAnswers);
    }
}
