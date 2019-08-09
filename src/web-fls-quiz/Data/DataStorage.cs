using WebFlsQuiz.Interfaces;
using WebFlsQuiz.Models;
using MongoDB.Driver;
using System.Linq;

namespace WebFlsQuiz.Data
{
    public class DataStorage : IDataStorage
    {
        private readonly IMongoDatabase _database;

        private IMongoCollection<QuestionData> _questions =>
            _database.GetCollection<QuestionData>("Questions");

        private IMongoCollection<Quiz> _quizzes =>
            _database.GetCollection<Quiz>("Quizzes");

        private IMongoCollection<QuizResult> _quizResults =>
            _database.GetCollection<QuizResult>("QuizResults");

        public DataStorage(IConfigurationService configuration)
        {
            var client = new MongoClient(configuration.GetDbConnectionString().Result);
            _database = client.GetDatabase(configuration.GetDbName().Result);
        }

        public QuestionData GetQuestion(int id)
        {
            return _questions
                .AsQueryable()
                .Where(x => x.Id == id)
                .First();
        }

        public IQueryable<int> GetQuestionIds(int quizId)
        {
            return _questions
                .AsQueryable()
                .Where(x => x.QuizId == quizId)
                .Select(x => x.Id);
        }

        public Quiz GetQuiz(string quizName)
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
