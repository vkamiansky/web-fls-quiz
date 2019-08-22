using WebFlsQuiz.Interfaces;
using WebFlsQuiz.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebFlsQuiz.Services
{
    public class MailService : IMailService
    {
        private readonly IConfigurationService _configurationService;

        private readonly IDataStorage _dataStorage;

        public MailService(
            IConfigurationService configurationService,
            IDataStorage dataStorage)
        {
            _configurationService = configurationService;
            _dataStorage = dataStorage;
        }

        private string GetCommitteeMailText(string email, string name, string comment, UserResult result)
        {
            return "Email: " + email + Environment.NewLine
                     + "Имя: " + name + Environment.NewLine
                     + "Заметки: " + comment + Environment.NewLine
                     + "Общий результат(%): " + result.PercentUserAnswersCorrect + Environment.NewLine + Environment.NewLine
                     + "Ответы:" + Environment.NewLine + Environment.NewLine + string.Join(
                         Environment.NewLine + Environment.NewLine,
                         result.QuestionResults.Select(
                             x => "Вопрос: " + x.QuestionText + Environment.NewLine
                                + string.Join(
                                    Environment.NewLine,
                                    x.AnswerResults.Select(
                                        y => (y.IsCorrect ? "(+)" : "(-)")
                                           + (y.IsUserChosen ? "(v)" : "")
                                           + y.AnswerText))));
        }

        public async Task SendResults(string email, string name, string comment, UserResult result, string quizName)
        {
            var host = await _configurationService.GetMailingSmtpHost();
            var port = await _configurationService.GetMailingSmtpPort();
            var login = await _configurationService.GetMailingAccountLogin();
            var password = await _configurationService.GetMailingAccountPassword();

            // Письмо для участника
            var participantEmailMessage = new MimeMessage();

            var participantMailTemplate = _dataStorage.GetQuiz(quizName).ParticipantMailMessageTemplate;

            participantEmailMessage.From.Add(new MailboxAddress(participantMailTemplate.SenderName, login));
            participantEmailMessage.To.Add(new MailboxAddress(name, email));
            participantEmailMessage.Subject = participantMailTemplate.Subject;
            participantEmailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = participantMailTemplate.BodyTemplate.Replace("%%name%%", name).Replace("%%percent-correct%%", result.PercentUserAnswersCorrect.ToString()),
            };

            // Письмо для организаторов
            var committeeEmailMessage = new MimeMessage();

            committeeEmailMessage.From.Add(new MailboxAddress(participantMailTemplate.SenderName, login));
            committeeEmailMessage.To.Add(new MailboxAddress(participantMailTemplate.SenderName, login));
            committeeEmailMessage.Subject = "Результаты " + name;
            committeeEmailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = GetCommitteeMailText(email, name, comment, result)
            };

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(host, port, SecureSocketOptions.SslOnConnect);
                client.Authenticate(login, password);
                client.Send(participantEmailMessage);
                client.Send(committeeEmailMessage);
                client.Disconnect(true);
            }
        }

        public async Task<bool> SendConfirmCode(string confirmCode, bool useNotConfirmedConfiguration = false)
        {
            MailSettings mailSettings;
            if (useNotConfirmedConfiguration)
            {
                mailSettings = await _configurationService.GetMailSettingsUsingNotConfirmedConfiguration();
            }
            else
            {
                mailSettings = await _configurationService.GetMailSettings();
            }

            if (mailSettings == null)
                return false;

            if (string.IsNullOrEmpty(mailSettings.Address) ||
                string.IsNullOrEmpty(mailSettings.Login) ||
                string.IsNullOrEmpty(mailSettings.Password) ||
                mailSettings.Port == 0)
            {
                return false;
            }

            var admin = Environment.GetEnvironmentVariable("ADMIN_EMAIL");

            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Quiz", mailSettings.Login));
            message.To.Add(new MailboxAddress("Quiz", admin));
            message.Subject = "An attempt to change secret token";
            message.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = $"To confirm new configuration visit http://<web-fls-quiz-address>/Configuration/Confirm?confirmCode={confirmCode}"
            };

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(mailSettings.Address, mailSettings.Port, SecureSocketOptions.SslOnConnect);
                client.Authenticate(mailSettings.Login, mailSettings.Password);
                client.Send(message);
                client.Disconnect(true);
            }

            return true;
        }
    }
}
