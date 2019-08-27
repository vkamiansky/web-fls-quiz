using WebFlsQuiz.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebFlsQuiz.Interfaces;

namespace WebFlsQuiz.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDataStorage _dataStorage;

        private readonly IImageService _imageService;

        public HomeController(
            IDataStorage dataStorage,
            IImageService imageService)
        {
            _dataStorage = dataStorage;
            _imageService = imageService;
        }

        [HttpHead]
        public IActionResult Head()
        {
            return Ok();
        }

        [HttpGet("{quizName}")]
        public IActionResult Index(string quizName = "java")
        {
            var quiz = _dataStorage.GetQuiz(quizName.ToLower());

            _imageService.LoadIfNeeded(quiz.LogoImage);
            _imageService.LoadIfNeeded(quiz.FinishScreenImage);
            _imageService.LoadIfNeeded(quiz.IntroScreenImage);

            if (quiz != null)
                return View("Index", quiz);
            else
                return View("Index", null);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
