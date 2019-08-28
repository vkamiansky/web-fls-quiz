namespace WebFlsQuiz.Models
{
    public class QuizInfo
    {
        public MongoDB.Bson.ObjectId Id { get; set; }

        public string Name { get; set; }

        public string Greeting { get; set; }

        public string SubmitScreenMessage { get; set; }

        public string FinishScreenMessage { get; set; }

        public QuizOrganizer Organizer { get; set; }

        public QuestionData[] Questions { get; set; }

        public ParticipantMailMessageTemplate ParticipantMailMessageTemplate { get; set; }

        public StandardImage LogoImage { get; set; }

        public StandardImage IntroScreenImage { get; set; }

        public StandardImage FinishScreenImage { get; set; }

        public int NumberOfQuestions { get; set; }
    }
}
