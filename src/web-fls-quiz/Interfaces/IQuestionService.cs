using WebFlsQuiz.Models;

namespace WebFlsQuiz.Interfaces
{
    public interface IQuestionService
    {
        IOperationResult<Question> GetRandom(int[] excludedQuestionIds, string quizName);
        IOperationResult<UserResult> GetUserResult(UserAnswer[] userAnswers, string quizName);
        IOperationResult<QuizInfo> GetQuizInfo(string quizName);
    }
}
