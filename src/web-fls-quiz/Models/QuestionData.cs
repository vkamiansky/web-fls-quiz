namespace WebFlsQuiz.Models
{
    public class QuestionData
    {
        public int Id { get; set; }

        public int QuizId { get; set; }

        public string ImageBase64 { get; set; }

        public int ImageId { get; set; }

        public string Text { get; set; }

        public AnswerData[] Answers { get; set; }
    }
}
