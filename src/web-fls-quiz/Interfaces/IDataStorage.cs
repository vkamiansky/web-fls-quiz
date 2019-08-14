using System.Linq;
using WebFlsQuiz.Models;

namespace WebFlsQuiz.Interfaces
{
    public interface IDataStorage
    {
        QuestionData GetQuestion(int id);
        QuizInfo GetQuiz(string quizName);
        IQueryable<int> GetQuestionIds(int quizId);
    }
}
