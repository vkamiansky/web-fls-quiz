using WebFlsQuiz.Interfaces;
using WebFlsQuiz.Models;
using MongoDB.Driver;
using System.Linq;

namespace WebFlsQuiz.Data
{
    public class DataStorage : IDataStorage
    {
        private readonly IMongoDatabase _database;

        private IMongoCollection<QuizInfo> _quizzes =>
            _database.GetCollection<QuizInfo>("Quizzes");

        private IMongoCollection<QuizResult> _quizResults =>
            _database.GetCollection<QuizResult>("QuizResults");

        public DataStorage(IConfigurationService configuration)
        {
            var client = new MongoClient(configuration.GetDbConnectionString().Result);
            _database = client.GetDatabase(configuration.GetDbName().Result);
        }

        public QuestionData GetQuestion(string quizName, int id)
        {
            return _quizzes
                .AsQueryable()
                .Where(x => x.Name == quizName)
                .SelectMany(x => x.Questions)
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        public int GetQuestionsNumber(string quizName)
        {
            return _quizzes
                .AsQueryable()
                .Where(x => x.Name == quizName)
                .SelectMany(x => x.Questions)
                .Count();
        }

        public QuizInfo GetQuiz(string quizName)
        {
            return _quizzes
                .AsQueryable()
                .Where(x => string.Equals(x.Name, quizName))
                .FirstOrDefault();
        }

        public void InsertQuizResult(QuizResult quizResult)
        {
            _quizResults
                .InsertOne(quizResult);
        }
    }
}
