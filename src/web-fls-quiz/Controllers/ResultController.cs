using WebFlsQuiz.Interfaces;
using WebFlsQuiz.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebFlsQuiz.Controllers
{
    public class ResultController : Controller
    {
        private IQuestionService _questionService { get; set; }

        private IMailService _mailService { get; set; }

        private IDataStorage _dataStorage { get; set; }

        private static JsonSerializerSettings JsonSerializerSettings =>
            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        public ResultController(
            IQuestionService questionService,
            IMailService mailService,
            IDataStorage dataStorage)
        {
            _questionService = questionService;
            _mailService = mailService;
            _dataStorage = dataStorage;
        }

        [HttpPost]
        public string SaveResults(string email, string name, string comment, UserAnswer[] userAnswers, string quizName)
        {
            var quizResult = new QuizResult
            {
                Comment = comment,
                Email = email,
                Name = name,
                QuizName = quizName,
                UserAnswers = userAnswers
            };
            _dataStorage.InsertQuizResult(quizResult);

            var results = _questionService.GetUserResult(userAnswers, quizName);
            _mailService.SendResults(email, name, comment, results);
            return JsonConvert.SerializeObject(new { }, JsonSerializerSettings);
        }
    }
}
