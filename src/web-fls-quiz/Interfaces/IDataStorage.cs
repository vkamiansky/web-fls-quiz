using WebFlsQuiz.Models;

namespace WebFlsQuiz.Interfaces
{
    public interface IDataStorage
    {
        QuestionData GetQuestion(int id);
        long GetQuestionsNumber();
    }
}
