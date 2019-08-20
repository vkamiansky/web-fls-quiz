using WebFlsQuiz.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebFlsQuiz.Interfaces;

namespace WebFlsQuiz.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDataStorage _dataStorage;

        public HomeController(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        [HttpHead]
        public IActionResult Head()
        {
            return Ok();
        }

        [HttpGet("{quizName}")]
        public IActionResult Index(string quizName = "HolyJS")
        {
            var quiz = _dataStorage.GetQuiz(quizName);

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
