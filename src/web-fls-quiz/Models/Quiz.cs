namespace WebFlsQuiz.Models
{
    public class Quiz
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Greeting { get; set; }

        public QuizOrganizer Organizer { get; set; }
    }
}
