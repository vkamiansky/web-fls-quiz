namespace WebFlsQuiz.Models
{
    public class QuizInfo
    {
        public MongoDB.Bson.ObjectId Id { get; set; }

        public string Name { get; set; }

        public string Greeting { get; set; }

        public QuizOrganizer Organizer { get; set; }

        public QuestionData[] Questions { get; set; }
    }
}
