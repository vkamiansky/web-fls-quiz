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

        private IMongoCollection<string> _confirmCodes =>
            _database.GetCollection<string>("ConfirmCodes");

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

        public void InsertConfirmCode(string confirmCode)
        {
            _confirmCodes
                .InsertOne(confirmCode);
        }

        public bool DoesConfirmCodeExist(string confirmCode)
        {
            var count = _confirmCodes
                .AsQueryable()
                .Where(x => string.Equals(x, confirmCode))
                .Count();

            return count > 0;
        }
    }
}
