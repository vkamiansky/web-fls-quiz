using HolyJsQuiz2019.Models;

namespace HolyJsQuiz2019.Interfaces
{
    public interface IQuestionService
    {
        Question GetRandom(int[] excludedQuestionsIds);

        UserResult GetUserResult(UserAnswer[] userAnswers);
    }
}
