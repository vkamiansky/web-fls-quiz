using System.Linq;

namespace HolyJsQuiz2019.Models
{
    public class UserResult
    {
        public int PercentUserAnswersCorrect { get => 100 * QuestionResults.Count(x => x.IsUserAnswerCorrect) / QuestionResults.Length; }

        public QuestionResult[] QuestionResults { get; set; }
    }
}
