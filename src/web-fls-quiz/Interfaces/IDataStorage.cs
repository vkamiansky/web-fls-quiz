using System.Linq;
using WebFlsQuiz.Models;

namespace WebFlsQuiz.Interfaces
{
    public interface IDataStorage
    {
        QuestionData GetQuestion(int id);
        Quiz GetQuiz(string quizName);
        IQueryable<int> GetQuestionIds(int quizId);
        void InsertQuizResult(QuizResult quizResult);
    }
}
