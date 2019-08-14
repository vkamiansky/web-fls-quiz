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

        private string _ParticipantMailTemplate = @"<!DOCTYPE html PUBLIC "" -//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>

    <meta name = ""viewport"" content=""width=device-width, initial-scale=1.0"" /> <!-- So that mobile will display zoomed in -->
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge""> <!-- enable media queries for windows phone 8 -->
    <title>Викторина</title>

    <style type = ""text/css"" >
            /* Client-specific Styles */
# outlook a {
            padding: 0;
    }
    /* Force Outlook to provide a ""view in browser"" menu link. */
    body {
            width: 100% !important;
            -webkit-text-size-adjust: 100%;
            -ms-text-size-adjust: 100%;
            margin: 0;
            padding: 0;
        }
        /* Prevent Webkit and Windows Mobile platforms from changing default font sizes, while not breaking desktop design. */
        .ExternalClass {
            width: 100%;
        }
            /* Force Hotmail to display emails at full width */
            .ExternalClass, .ExternalClass p, .ExternalClass span, .ExternalClass font, .ExternalClass td, .ExternalClass div
{
    line-height: 100%;
}
                /* Force Hotmail to display normal line spacing.*/
                .ExternalClass, .ExternalClass p, .ExternalClass span, .ExternalClass font, .ExternalClass td, .ExternalClass div
{
    mso-line-height-rule: exactly;
}
/* Force Outlook 2016 to display normal line spacing.*/
# backgroundTable {
margin: 0;
            padding: 0;
            width: 100% !important;
            line-height: 100% !important;
        }

        img {
            outline: none;
            text-decoration: none;
            border: none;
            -ms-interpolation-mode: bicubic;
        }

        a img
{
    border: none;
}

        .image_fix {
            display: block;
        }

        p {
            margin: 0px 0px !important;
        }

        table td
{
    border-collapse: collapse;
}
/* so Outlook 2016 doesn't add a 1px border to table cells */
table {
            border-collapse: collapse;
            mso-table-lspace: 0pt;
            mso-table-rspace: 0pt;
        }

        a {
            color: #0072cf;
            text-decoration: none;
            text-decoration: none !important;
        }
        /*STYLES*/
        table[class=full] {
            width: 100%;
            clear: both;
        }
        /*IPAD STYLES*/
        @media only screen and(max-width: 640px)
{
    a[href ^= ""tel""], a[href ^= ""sms""] {
        text - decoration: none;
    color: #0072cf; /* or whatever your want */
                pointer - events: none;
    cursor: default;
    }

            .mobile_link a[href ^= ""tel""], .mobile_link a[href ^= ""sms""] {
        text - decoration: default;
    color: #0072cf !important;
                pointer - events: auto;
    cursor: default;
    }

    table[class=devicewidth] {
                width: 440px !important;
                text-align: center !important;
            }

            table[class=devicewidthinner] {
                width: 420px !important;
                text-align: center !important;
            }

            img[class=banner] {
                width: 440px !important;
                height: 220px !important;
            }

            img[class=colimg2] {
                width: 380px !important;
                height: 111px !important;
            }
        }
        /*IPHONE STYLES*/
        @media only screen and(max-width: 480px)
{
    a[href ^= ""tel""], a[href ^= ""sms""] {
        text - decoration: none;
    color: #0072cf; /* or whatever your want */
                pointer - events: none;
    cursor: default;
    }

            .mobile_link a[href ^= ""tel""], .mobile_link a[href ^= ""sms""] {
        text - decoration: default;
    color: #0072cf !important;
                pointer - events: auto;
    cursor: default;
    }

    table[class=devicewidth] {
                width: 280px !important;
                text-align: center !important;
            }

            table[class=devicewidthinner] {
                width: 260px !important;
                text-align: center !important;
            }

            img[class=banner] {
                width: 280px !important;
                height: 140px !important;
            }

            img[class=colimg2] {
                width: 240px !important;
                height: 70px !important;
            }

            img[class=logoimg] {
                width: 260px !important;
                height: 25px !important;
            }

            td[class=mobile-hide] {
                display: none !important;
            }

            td[class=""padding-bottom25""] {
                padding-bottom: 25px !important;
            }
        }
    </style>
</head>
<body>
    <!-- Start of preheader -->
    <!-- Start of invisible preview text section -->
    <table width = ""100%"" bgcolor=""#f0f0f0"" cellpadding=""0"" cellspacing=""0"" border=""0"">
        <tr>
            <td>
                <table width = ""600"" cellpadding=""0"" cellspacing=""0"" border=""0"" align=""center"" class=""devicewidth"">
                    <tr>
                        <td width = ""100%"" >
                            <table bgcolor=""f0f0f0"" width=""600"" cellpadding=""0"" cellspacing=""0"" border=""0"" align=""center"" class=""devicewidth"">
                                <!-- full width text spanning all columns -->
                                <!-- Spacing -->
                                <tr>
                                    <td height = ""5"" style=""font-size:1px; line-height:1px; mso-line-height-rule: exactly;"">&nbsp;</td>
                                </tr>
                                <!-- end Spacing -->
                                <tr>
                                    <td>
                                        <table width = ""560"" align=""center"" cellpadding=""0"" cellspacing=""0"" border="""" class=""devicewidthinner"">
                                            <!-- Invisible preheader text -->
                                            <tr>
                                                <td style = ""font-family: Helvetica, arial, sans-serif; font-size: 12px; color: #ffffff; text-align:center; line-height: 16px;display:none !important; mso-hide:all;"" >
                                                   Привет, <strong>%%name%%</strong>!
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style = ""font-family: Helvetica, arial, sans-serif; font-size: 12px; color: #ffffff; text-align:center; line-height: 16px;display:none !important; mso-hide:all;"" >
                                                    Благодарим за участие в викторине! 
                                                    Вы успешно ответили на %%percent-correct%%% вопросов!  
                                                    Пожалуйста, подойдите на стенд, покажите это письмо представителю и получите приятный подарок.
                                                </td>
                                            </tr>
                                            <!-- End Invisible preheader text -->
                                        </table>
                                    </td>
                                </tr>
                                <!-- end full width text spanning all columns -->
                                <!-- Spacing -->
                                <tr>
                                    <td width = ""100%"" height=""5"" style=""font-size: 10px; line-height: 10px;"">&nbsp;</td>
                                </tr>
                                <!-- end Spacing -->
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <!-- end of invisible preview text section -->
    <!-- End of preheader -->
    <!-- Start of header -->
    <table width = ""100%"" style=""min-width:100%"" bgcolor=""#f0f0f0"" cellpadding=""0"" cellspacing=""0"" border=""0"">
        <tr>
            <td>
                <table width = ""600"" cellpadding=""0"" cellspacing=""0"" border=""0"" align=""center"" bgcolor=""#f0f0f0"" class=""devicewidth"">
                    <tr>
                        <td width = ""100%"" >
                            <table width=""600"" cellpadding=""0"" cellspacing=""0"" border=""0"" align=""center"" class=""devicewidth"">
                                <!-- Spacing -->
                                <tr>
                                    <td height = ""2"" style=""font-size:1px;  line-height:1px; mso-line-height-rule: exactly;"">&nbsp;</td>
                                </tr>
                                <!-- end Spacing -->
                                <tr>
                                    <td>
                                        <!-- logo -->
                                        <table width = ""100%"" style=""width:100%; background-color:#8888cc;"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""devicewidth"" bgcolor=""8888cc"">
                                            <!-- Spacing -->
                                            <tr>
                                                <td colspan = ""3"" height=""10"" style=""font-size:1px; line-height:1px; mso-line-height-rule: exactly;"">&nbsp;</td>
                                            </tr>
                                            <!-- end Spacing -->
                                            <!-- Spacing -->
                                            <tr>
                                                <td colspan = ""3"" height=""10"" style=""font-size:1px; line-height:1px; mso-line-height-rule: exactly;"">&nbsp;</td>
                                            </tr>
                                            <!-- end Spacing -->
                                        </table>
                                        <!-- end of logo -->
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <!-- End of Header -->
    <!-- Start Full Text -->
    <table width = ""100%"" bgcolor=""#f0f0f0"" cellpadding=""0"" cellspacing=""0"" border=""0"">
        <tr>
            <td>
                <table width = ""600"" bgcolor=""#ffffff"" cellpadding=""0"" cellspacing=""0"" border=""0"" align=""center"" class=""devicewidth"">
                    <tr>
                        <td width = ""100%"" >
                            <table width=""600"" cellpadding=""0"" cellspacing=""0"" border=""0"" align=""center"" class=""devicewidth"">
                                <!-- Spacing -->
                                <tr>
                                    <td height = ""20"" style=""font-size:1px; line-height:1px; mso-line-height-rule: exactly;"">&nbsp;</td>
                                </tr>
                                <!-- Spacing -->
                                <tr>
                                    <td align = ""center"" >
                                        <table width=""560"" align=""center"" cellpadding=""0"" cellspacing=""0"" border=""0"" class=""devicewidthinner"">
                                            <!-- Title -->
                                            <tr>
                                                <td style = ""font-family: Helvetica, arial, sans-serif; font-size: 20px; font-weight:100; color: #666666; text-align:left; line-height: 30px;"" >
                                                    Привет, <strong>%%name%%</strong>!
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style = ""font-family: Helvetica, arial, sans-serif; font-size: 20px; font-weight:100; color: #666666; text-align:left; line-height: 30px;"" >
                                                    Благодарим за участие в викторине! 
                                                    Вы успешно ответили на %%percent-correct%%% вопросов!  
                                                    Пожалуйста, подойдите на стенд, покажите это письмо представителю и получите приятный подарок.
                                                </td>
                                            </tr>
                                            <!-- End of Title -->
                                            <!-- spacing -->
                                            <tr>
                                                <td width = ""100%"" height=""20"" style=""font-size:1px; line-height:1px; mso-line-height-rule: exactly;"">&nbsp;</td>
                                            </tr>
                                            <!-- End of spacing -->
                                            <!-- content -->
                                            <tr>
                                                <td style = ""font-family: Helvetica, arial, sans-serif; font-size: 14px; font-weight:100; color: #666666; text-align:left; line-height: 24px;"" >
                                                    Более детальную информацию о нашей компании и проектах можно найти на сайте - <a href = ""/"" target=""_blank"" style=""text-decoration:none; color:#E3A002""><strong>site.ru</strong></a>.    
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style = ""font-family: Helvetica, arial, sans-serif; font-size: 14px; font-weight:100; color: #666666; text-align:left; line-height: 24px;"" >
                                                    А так же, вступайте в наши группы <a href = ""/"" target=""_blank"" style=""text-decoration:none; color:#E3A002""><strong>vk.com</strong></a> и <a href = ""/"" target=""_blank"" style=""text-decoration:none; color:#E3A002""><strong>www.facebook.com</strong></a> там много интересного.
                                                </td>
                                            </tr>
                                            <!-- spacing -->
                                            <tr>
                                                <td width = ""100%"" height= ""20"" style= ""font-size:1px; line-height:1px; mso-line-height-rule: exactly;"" > &nbsp;</td>
                                            </tr>
                                            <!-- End of spacing -->
                                            <!-- End of content -->
                                        </table>
                                    </td>
                                </tr>
                                <!-- Spacing -->
                                <tr>
                                    <td height = ""20"" style=""font-size:1px; line-height:1px; mso-line-height-rule: exactly;"">&nbsp;</td>
                                </tr>
                                <!-- Spacing -->
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <!-- end of full text -->
    <!-- Start of separator -->
    <table width = ""100%"" bgcolor=""#f0f0f0"" cellpadding=""0"" cellspacing=""0"" border=""0"">
        <tr>
            <td align = ""center"" height=""20"" style=""font-size:1px; line-height:1px;"">&nbsp;</td>
        </tr>
    </table>
    <!-- End of separator -->
</body>
</html>
";

        private readonly IConfigurationService _configurationService;

        public MailService(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
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

        public async Task SendResults(string email, string name, string comment, UserResult result)
        {
            var host = await _configurationService.GetMailingSmtpHost();
            var port = await _configurationService.GetMailingSmtpPort();
            var login = await _configurationService.GetMailingAccountLogin();
            var password = await _configurationService.GetMailingAccountPassword();

            // Письмо для участника
            var participantEmailMessage = new MimeMessage();

            participantEmailMessage.From.Add(new MailboxAddress("Команда Quiz", login));
            participantEmailMessage.To.Add(new MailboxAddress(name, email));
            participantEmailMessage.Subject = "Викторина";
            participantEmailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = _ParticipantMailTemplate.Replace("%%name%%", name).Replace("%%percent-correct%%", result.PercentUserAnswersCorrect.ToString()),
            };

            // Письмо для организаторов
            var committeeEmailMessage = new MimeMessage();

            committeeEmailMessage.From.Add(new MailboxAddress("Команда Quiz", login));
            committeeEmailMessage.To.Add(new MailboxAddress("Quiz Team", login));
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

        public async Task SendConfirmCode(string confirmCode)
        {
            var host = await _configurationService.GetMailingSmtpHost();
            var port = await _configurationService.GetMailingSmtpPort();
            var login = await _configurationService.GetMailingAccountLogin();
            var password = await _configurationService.GetMailingAccountPassword();

            var admin = Environment.GetEnvironmentVariable("ADMIN_EMAIL");

            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Quiz", login));
            message.To.Add(new MailboxAddress("Quiz", admin));
            message.Subject = "An attempt to change secret token";
            message.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = $"To confirm new secret token visit http://localhost:5000/Configuration/ConfirmConfig?confirmCode={confirmCode}"
            };

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(host, port, SecureSocketOptions.SslOnConnect);
                client.Authenticate(login, password);
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
