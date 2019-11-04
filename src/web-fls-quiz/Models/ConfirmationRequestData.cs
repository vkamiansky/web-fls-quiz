namespace WebFlsQuiz.Models
{
        public class ConfirmationRequestData
        {
            public string ConfirmationCode { get; set; }
            public string AdminEmail { get; set; }
            public string SmtpHost { get; set; }
            public string SmtpPort { get; set; }
            public string MailingAccountLogin { get; set; }
            public string MailingAccountPassword { get; set; }
        }
}