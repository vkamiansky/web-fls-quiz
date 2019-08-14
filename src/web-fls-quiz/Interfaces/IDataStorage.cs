using WebFlsQuiz.Models;

namespace WebFlsQuiz.Interfaces
{
    public interface IDataStorage
    {
        QuestionData GetQuestion(int id);
        long GetQuestionsNumber();
        void InsertConfirmCode(string confirmCode);
        bool DoesConfirmCodeExist(string confirmCode);
    }
}
