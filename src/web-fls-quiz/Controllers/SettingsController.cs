using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebFlsQuiz.Controllers
{
    public class SettingsController : Controller
    {
        private static JsonSerializerSettings JsonSerializerSettings => 
            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        public string QuizOptions()
        {
            var settings = new
            {
                CountOfQuestions = 3
            };

            return JsonConvert.SerializeObject(new { settings }, JsonSerializerSettings);
        }
    }
}
