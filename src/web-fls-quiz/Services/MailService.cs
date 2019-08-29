using WebFlsQuiz.Interfaces;
using WebFlsQuiz.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebFlsQuiz.Services
{
    public class MailService : IMailService
    {
        private readonly IConfigurationService _configurationService;

        private readonly IDataStorage _dataStorage;

        private readonly IHttpContextAccessor _contextAccessor;

        public MailService(
            IConfigurationService configurationService,
            IDataStorage dataStorage,
            IHttpContextAccessor contextAccessor)
        {
            _configurationService = configurationService;
            _dataStorage = dataStorage;
            _contextAccessor = contextAccessor;
        }

        public async Task<string> GetAdminEmail()
        {
            var result = await _configurationService.GetAdminEmail();

            if (!string.IsNullOrEmpty(result))
                return result;

            result = await _configurationService.GetAdminEmailUsingNotConfirmedConfiguration();

            if (!string.IsNullOrEmpty(result))
                return result;

            return null;
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

            var admin = await GetAdminEmail();
            if (admin == null)
                return false;

            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Quiz", mailSettings.Login));
            message.To.Add(new MailboxAddress("Quiz", admin));
            message.Subject = "An attempt to change configuration";

            var confirmUrl = string.Format("{0}://{1}/Configuration/Confirm?confirmCode={2}",
                _contextAccessor.HttpContext.Request.Scheme,
                _contextAccessor.HttpContext.Request.Host,
                confirmCode);

            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $"To confirm new configuration visit <a href='{confirmUrl}'>this link</a>"
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
