using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebFlsQuiz.Interfaces;
using WebFlsQuiz.Models;

namespace WebFlsQuiz.Controllers
{
    public class ResultController : Controller
    {
        private IQuestionService _questionService { get; set; }
        private IMailService _mailService { get; set; }
        private IDataStorage _dataStorage { get; set; }
        private readonly ILogger _logger;
        public ResultController(
            IQuestionService questionService,
            IMailService mailService,
            IDataStorage dataStorage,
            ILoggerFactory loggerFactory)
        {
            _questionService = questionService;
            _mailService = mailService;
            _dataStorage = dataStorage;
            _logger = loggerFactory.CreateLogger("Result");
        }
        [HttpPost]
        public IActionResult SaveResults(string email, string name, string comment, UserAnswer[] userAnswers, string quizName)
        {
            var quizResult = new QuizResult
            {
                SubmittedAt = DateTime.Now,
                Comment = comment,
                Email = email,
                Name = name,
                QuizName = quizName,
                UserAnswers = userAnswers
            };
            return _dataStorage.InsertQuizResult(quizResult)
            .Bind(() => _questionService.GetUserResult(userAnswers, quizName))
            .Bind(results => _mailService.SendResults(email, name, comment, results, quizName))
            .Bind(() => new { }.ToResult())
            .WithLogging(_logger)
            .ToApiResult();
        }
    }
}
