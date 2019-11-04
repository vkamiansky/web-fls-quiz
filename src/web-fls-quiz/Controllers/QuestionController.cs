using WebFlsQuiz.Interfaces;
using WebFlsQuiz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
            .Bind(q => new
            {
                question = new
                {
                    id = q.Id,
                    imageBase64 = q.ImageBase64,
                    text = q.Text,
                    answers = q.Answers,
                    multipleAnswer = q.MultipleAnswer
                }
            }.ToResult())
            .WithLogging(_logger)
            .ToApiResult();
        }
    }
}
