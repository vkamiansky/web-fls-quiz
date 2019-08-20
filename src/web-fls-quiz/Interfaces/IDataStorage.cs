using WebFlsQuiz.Models;

namespace WebFlsQuiz.Interfaces
{
    public interface IDataStorage
    {
        QuestionData GetQuestion(string quizName, int id);
        QuizInfo GetQuiz(string quizName);
        int? GetQuestionsNumber(string quizName);
        bool InsertQuizResult(QuizResult quizResult);
    }
}
