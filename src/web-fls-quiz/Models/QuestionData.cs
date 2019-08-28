namespace WebFlsQuiz.Models
{
    public class QuestionData
    {
        public int Id { get; set; }

        public StandardImage Image { get; set; }

        public string Text { get; set; }

        public AnswerData[] Answers { get; set; }

        public bool MultipleAnswer { get; set; }
    }
}
