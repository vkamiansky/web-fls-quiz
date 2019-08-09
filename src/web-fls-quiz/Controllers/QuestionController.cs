using WebFlsQuiz.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebFlsQuiz.Controllers
{
    public class QuestionController : Controller
    {
        private IQuestionService _QuestionService { get; set; }

        private static JsonSerializerSettings JsonSerializerSettings => 
            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        public QuestionController(IQuestionService questionService)
        {
            _QuestionService = questionService;
        }

        [HttpPost]
        public string GetRandom(int[] excludedQuestionsIds, int quizId)
        {
            var question = _QuestionService.GetRandom(excludedQuestionsIds, quizId);

            return JsonConvert.SerializeObject(
                new {
                    Question = new {
                        question.Id,
                        question.ImageBase64,
                        question.Text,
                        question.Answers
                    }}, JsonSerializerSettings);
        }
    }
}
