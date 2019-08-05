using System.Linq;

namespace WebFlsQuiz.Models
{
    public class QuestionResult
    {
        public string QuestionText { get; set; }

        public AnswerResult[] AnswerResults { get; set; }

        public bool IsUserAnswerCorrect { get => !AnswerResults.Any(x => x.IsCorrect ^ x.IsUserChosen); }
    }
}
