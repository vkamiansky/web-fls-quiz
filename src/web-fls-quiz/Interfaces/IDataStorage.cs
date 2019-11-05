using WebFlsQuiz.Models;

namespace WebFlsQuiz.Interfaces
{
    public interface IDataStorage
    {
        IOperationResult<QuestionData> GetQuestion(string quizName, int id);
        IOperationResult<int> GetQuestionsNumber(string quizName);
        IOperationResult<QuizInfo> GetQuiz(string quizName);
        IOperationResult InsertQuizResult(QuizResult quizResult);
        IOperationResult<StandardImage> GetStandardImage(int id);
        IOperationResult<int[]> GetStandardImagesIds();
        IOperationResult<int[]> GetStandardImagesIds(ImageType imageType);
    }
}
