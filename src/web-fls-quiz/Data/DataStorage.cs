using WebFlsQuiz.Interfaces;
using WebFlsQuiz.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;

namespace WebFlsQuiz.Data
{
    public class DataStorage : IDataStorage
    {
        private readonly IMongoDatabase _database;

        private IMongoCollection<QuestionData> _questions =>
            _database.GetCollection<QuestionData>("Questions");

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

        public long GetQuestionsNumber()
        {
            return _questions.CountDocuments(new BsonDocument());
        }
    }
}
