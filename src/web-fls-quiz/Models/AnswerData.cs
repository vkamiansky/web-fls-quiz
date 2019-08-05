namespace WebFlsQuiz.Models
{
    public class AnswerData
    {
        public int AnswerId { get; set; }

        public int QuestionId { get; set; }

        public string Text { get; set; }

        public bool IsCorrect { get; set; }
    }
}
