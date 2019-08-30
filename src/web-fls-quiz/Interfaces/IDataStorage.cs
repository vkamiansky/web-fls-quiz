using System.Threading.Tasks;
using WebFlsQuiz.Models;

namespace WebFlsQuiz.Interfaces
{
    public interface IDataStorage
    {
        Task<QuestionData> GetQuestion(string quizName, int id);
        Task<QuizInfo> GetQuiz(string quizName);
        Task<int?> GetQuestionsNumber(string quizName);
        Task<bool> InsertQuizResult(QuizResult quizResult);
        Task<StandardImage> GetStandardImage(int id);
        Task<int[]> GetStandardImagesIds();
        Task<int[]> GetStandardImagesIds(ImageType imageType);
    }
}
