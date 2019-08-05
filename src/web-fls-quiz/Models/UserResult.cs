using System.Linq;

namespace WebFlsQuiz.Models
{
    public class UserResult
    {
        public int PercentUserAnswersCorrect { get => 100 * QuestionResults.Count(x => x.IsUserAnswerCorrect) / QuestionResults.Length; }

        public QuestionResult[] QuestionResults { get; set; }
    }
}
