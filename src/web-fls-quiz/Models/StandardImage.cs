namespace WebFlsQuiz.Models
{
    public class StandardImage
    {
        public MongoDB.Bson.ObjectId Id { get; set; }

        public int ImageId { get; set; }

        public string ImageBase64 { get; set; }
    }
}
