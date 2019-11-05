using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebFlsQuiz.Interfaces;
using WebFlsQuiz.Models;

namespace WebFlsQuiz.Controllers
{
    public class QuestionController : Controller
    {
        private readonly IQuestionService _questionService;
        private readonly ILogger _logger;
        public QuestionController(
            IQuestionService questionService,
            ILoggerFactory loggerFactory)
        {
            _questionService = questionService;
            _logger = loggerFactory.CreateLogger("Question");
        }
        [HttpPost]
        public IActionResult GetRandom(int[] excludedQuestionsIds, string quizName)
        {
            return _questionService.GetRandom(excludedQuestionsIds, quizName)
            .Bind(question => new
            {
                Question = new
                {
                    Id = question.Id,
                    ImageBase64 = question.ImageBase64,
                    Text = question.Text,
                    Answers = question.Answers,
                    MultipleAnswer = question.MultipleAnswer
                }
            }.ToResult())
            .WithLogging(_logger)
            .ToApiResult();
        }
    }
}
