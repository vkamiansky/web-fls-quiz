using WebFlsQuiz.Interfaces;
using WebFlsQuiz.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Linq;
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
        private IOperationResult<string> GetCommitteeMailText(string email, string name, string comment, UserResult result)
        {
            return OperationResult.Try(() =>
                OperationResult.Success(
                    "Email: " + email + Environment.NewLine
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
                                           + y.AnswerText))))));
        }
        public IOperationResult SendResults(string email, string name, string comment, UserResult result, string quizName)
        {
            return OperationResult.All(new Func<IOperationResult<string>>[]{
                () => OperationResult.Try(() => _configurationService.GetString(ConfigurationKey.SmtpHost)),
                () => OperationResult.Try(() => _configurationService.GetString(ConfigurationKey.SmtpPort)),
                () => OperationResult.Try(() => _configurationService.GetString(ConfigurationKey.MailingAccountLogin)),
                () => OperationResult.Try(() => _configurationService.GetString(ConfigurationKey.MailingAccountPassword))
            })
            .Bind(config => OperationResult.Success(new
            {
                Host = config[0],
                Port = config[1],
                Login = config[2],
                Password = config[3]
            }))
            .Bind(mailSettings => OperationResult.Try(() =>
            {
                return _dataStorage.GetQuiz(quizName)
                    .Bind(quiz => OperationResult.Success(quiz.ParticipantMailMessageTemplate))
                    .Bind(template =>
                    {
                        return OperationResult.All(new Func<IOperationResult<MimeMessage>>[]{
                            // Participant message
                            () =>
                            {
                                var message = new MimeMessage();
                                message.From.Add(new MailboxAddress(template.SenderName, mailSettings.Login));
                                message.To.Add(new MailboxAddress(name, email));
                                message.Subject = template.Subject;
                                message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                                {
                                    Text = template.BodyTemplate.Replace("%%name%%", name).Replace("%%percent-correct%%", result.PercentUserAnswersCorrect.ToString()),
                                };
                                return OperationResult.Success(message);
                            },
                            // Committee message
                            () => GetCommitteeMailText(email, name, comment, result)
                                .Bind(text =>
                                {
                                    var message = new MimeMessage();

                                    message.From.Add(new MailboxAddress(template.SenderName, mailSettings.Login));
                                    message.To.Add(new MailboxAddress(template.SenderName, mailSettings.Login));
                                    message.Subject = "Результаты " + name;
                                    message.Body = new TextPart(MimeKit.Text.TextFormat.Text)
                                    {
                                        Text = text
                                    };
                                    return OperationResult.Success(message);
                                })
                        });
                    })
                    .Bind(messages =>
                    {
                        using (var client = new SmtpClient())
                        {
                            client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                            client.Connect(mailSettings.Host, int.Parse(mailSettings.Port), SecureSocketOptions.SslOnConnect);
                            client.Authenticate(mailSettings.Login, mailSettings.Password);
                            client.Send(messages[0]);
                            client.Send(messages[1]);
                            client.Disconnect(true);
                            return OperationResult.Success();
                        }
                    });
            }));
        }
        public IOperationResult SendConfirmCode(ConfirmationRequestData data)
        {
            return OperationResult.Try(() =>
            {
                if (string.IsNullOrEmpty(data.AdminEmail) ||
                    string.IsNullOrEmpty(data.SmtpHost) ||
                    string.IsNullOrEmpty(data.SmtpPort) ||
                    string.IsNullOrEmpty(data.MailingAccountLogin) ||
                    string.IsNullOrEmpty(data.SmtpPort))
                    return OperationResult.Failure(new Exception("One of necessary configuration parameters is empty"));

                var message = new MimeMessage();

                message.From.Add(new MailboxAddress("Quiz", data.MailingAccountLogin));
                message.To.Add(new MailboxAddress("Quiz", data.AdminEmail));
                message.Subject = "An attempt to change configuration";

                var confirmUrl = string.Format("{0}://{1}/Configuration/Confirm?confirmCode={2}",
                    _contextAccessor.HttpContext.Request.Scheme,
                    _contextAccessor.HttpContext.Request.Host,
                    data.ConfirmationCode);

                message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = $"To confirm new configuration visit <a href='{confirmUrl}'>this link</a>"
                };

                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect(data.SmtpHost, int.Parse(data.SmtpPort), SecureSocketOptions.SslOnConnect);
                    client.Authenticate(data.MailingAccountLogin, data.MailingAccountPassword);
                    client.Send(message);
                    client.Disconnect(true);
                    return OperationResult.Success();
                }
            });
        }
    }
}
