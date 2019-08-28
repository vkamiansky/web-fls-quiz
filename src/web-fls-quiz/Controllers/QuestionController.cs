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
        public string GetRandom(int[] excludedQuestionsIds, string quizName)
        {
            Models.Question question;
            try
            {
                question = _QuestionService.GetRandom(excludedQuestionsIds, quizName);
            }
            catch (System.TimeoutException)
            {
                // Looks like MongoDB is not responding
                return null;
            }
            catch (MongoDB.Driver.MongoConnectionException)
            {
                // Looks like MongoDB is not responding
                return null;
            }

            return JsonConvert.SerializeObject(
                new {
                    Question = new {
                        question.Id,
                        question.ImageBase64,
                        question.Text,
                        question.Answers,
                        question.MultipleAnswer
                    }}, JsonSerializerSettings);
        }
    }
}
