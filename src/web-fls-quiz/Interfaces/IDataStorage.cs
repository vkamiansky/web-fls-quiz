using WebFlsQuiz.Models;

namespace WebFlsQuiz.Interfaces
{
    public interface IDataStorage
    {
        QuestionData GetQuestion(string quizName, int id);
        QuizInfo GetQuiz(string quizName);
        int GetQuestionsNumber(string quizName);
        void InsertQuizResult(QuizResult quizResult);
        void InsertConfirmCode(string confirmCode);
        bool DoesConfirmCodeExist(string confirmCode);
    }
}
