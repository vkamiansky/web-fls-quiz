namespace WebFlsQuiz.Models
{
    public class QuizResult
    {
        public int QuizId { get; set; }

        public UserAnswer[] UserAnswers { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Comment { get; set; }
    }
}
