using System.Linq;

namespace HolyJsQuiz2019.Models
{
    public class QuestionResult
    {
        public string QuestionText { get; set; }

        public AnswerResult[] AnswerResults { get; set; }

        public bool IsUserAnswerCorrect { get => !AnswerResults.Any(x => x.IsCorrect ^ x.IsUserChosen); }
    }
}
