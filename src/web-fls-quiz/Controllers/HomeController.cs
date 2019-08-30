using WebFlsQuiz.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebFlsQuiz.Interfaces;
using System.Threading.Tasks;
using WebFlsQuiz.Common;

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
        public async Task<IActionResult> Index(string quizName = "java")
        {
            var quiz = await _dataStorage.GetQuiz(quizName.ToLower()).ExecuteWithTimeout(1000);

            if (quiz != null)
            {
                _imageService.LoadIfNeeded(quiz.LogoImage);
                _imageService.LoadIfNeeded(quiz.FinishScreenImage);
                _imageService.LoadIfNeeded(quiz.IntroScreenImage);
            }

            return View("Index", quiz);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
