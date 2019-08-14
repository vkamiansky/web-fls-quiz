namespace WebFlsQuiz.Models
{
    public class QuizInfo
    {
        public string Name { get; set; }

        public string Greeting { get; set; }

        public QuizOrganizer Organizer { get; set; }

        public QuestionData[] Questions { get; set; }
    }
}
