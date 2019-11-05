using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using WebFlsQuiz.Interfaces;
using WebFlsQuiz.Models;

namespace WebFlsQuiz.Controllers
{
    public class HomeController : Controller
    {
        private readonly IQuestionService _questionService;
        private readonly IImageService _imageService;
        private readonly ILogger _logger;
        public HomeController(
            IQuestionService questionService,
            IImageService imageService,
            ILoggerFactory loggerFactory)
        {
            _questionService = questionService;
            _imageService = imageService;
            _logger = loggerFactory.CreateLogger("Home");
        }
        [HttpHead]
        public IActionResult Head()
        {
            return Ok();
        }
        [HttpGet("{quizName}")]
        public IActionResult Index(string quizName = "java")
        {
            return _questionService.GetQuizInfo(quizName)
                .WithLogging(_logger)
                .ToViewResult(this, "Index");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
