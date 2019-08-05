using HolyJsQuiz2019.Models;

namespace HolyJsQuiz2019.Interfaces
{
    public interface IDataStorage
    {
        QuestionData GetQuestion(int id);
        long GetQuestionsNumber();
    }
}
