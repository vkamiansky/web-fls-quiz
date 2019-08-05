using HolyJsQuiz2019.Interfaces;
using HolyJsQuiz2019.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HolyJsQuiz2019.Controllers
{
    public class ResultController : Controller
    {
        private IQuestionService _QuestionService { get; set; }
        private IMailService _MailService { get; set; }


        private static JsonSerializerSettings JsonSerializerSettings =>
            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        public ResultController(IQuestionService questionService, IMailService mailService)
        {
            _QuestionService = questionService;
            _MailService = mailService;
        }

        [HttpPost]
        public string SaveResults(string email, string name, string comment, UserAnswer[] userAnswers)
        {
            var results = _QuestionService.GetUserResult(userAnswers);
            //_MailService.SendResults(email, name, comment, results);
            return JsonConvert.SerializeObject(new { }, JsonSerializerSettings);
        }
    }
}