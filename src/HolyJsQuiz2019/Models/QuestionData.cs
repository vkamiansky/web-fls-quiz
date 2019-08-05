namespace HolyJsQuiz2019.Models
{
    public class QuestionData
    {
        public int Id { get; set; }

        public string ImageBase64 { get; set; }

        public string Text { get; set; }

        public AnswerData[] Answers { get; set; }
    }
}
