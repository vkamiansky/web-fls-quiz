using WebFlsQuiz.Interfaces;
using WebFlsQuiz.Models;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace WebFlsQuiz.Data
{
    public class DataStorage : IDataStorage
    {
        private readonly IConfigurationService _configurationService;

        private const string _quizzesCollectionName = "Quizzes";

        private const string _quizResultsCollectionName = "QuizResults";

        private const string _standardImagesCollectionName = "StandardImages";

        public DataStorage(IConfigurationService configuration)
        {
            _configurationService = configuration;
        }

        #region Creating connection and getting access to collections

        private async Task<IMongoDatabase> GetDatabase()
        {
            var connectionString = await _configurationService.GetDbConnectionString();
            var dbName = await _configurationService.GetDbName();

            if (string.IsNullOrEmpty(connectionString) ||
                string.IsNullOrEmpty(dbName))
            {
                return null;
            }

            try
            {
                var client = new MongoClient(connectionString);
                return client.GetDatabase(dbName);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        private IMongoCollection<T> GetCollection<T>(IMongoDatabase database, string collectionName)
        {
            if (database == null ||
                string.IsNullOrEmpty(collectionName))
            {
                return null;
            }

            try
            {
                return database.GetCollection<T>(collectionName);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        private async Task<IMongoCollection<QuizInfo>> GetQuizzesCollection()
        {
            var db = await GetDatabase();
            return GetCollection<QuizInfo>(db, _quizzesCollectionName);
        }

        private async Task<IMongoCollection<QuizResult>> GetQuizResultsCollection()
        {
            var db = await GetDatabase();
            return GetCollection<QuizResult>(db, _quizResultsCollectionName);
        }

        private async Task<IMongoCollection<StandardImage>> GetStandardImagesCollection()
        {
            var db = await GetDatabase();
            return GetCollection<StandardImage>(db, _standardImagesCollectionName);
        }

        #endregion

        public QuestionData GetQuestion(string quizName, int id)
        {
            var collection = GetQuizzesCollection().Result;

            if (collection == null)
                return null;

            return collection
                .AsQueryable()
                .Where(x => x.Name == quizName)
                .SelectMany(x => x.Questions)
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        public int? GetQuestionsNumber(string quizName)
        {
            var collection = GetQuizzesCollection().Result;

            if (collection == null)
                return null;

            return collection
                .AsQueryable()
                .Where(x => x.Name == quizName)
                .SelectMany(x => x.Questions)
                .Count();
        }

        public QuizInfo GetQuiz(string quizName)
        {
            var collection = GetQuizzesCollection().Result;

            if (collection == null)
                return null;

            return collection
                .AsQueryable()
                .Where(x => string.Equals(x.Name, quizName))
                .FirstOrDefault();
        }

        public bool InsertQuizResult(QuizResult quizResult)
        {
            var collection = GetQuizResultsCollection().Result;

            if (collection == null)
                return false;

            collection
                .InsertOne(quizResult);

            return true;
        }

        public async Task<StandardImage> GetStandardImage(int id)
        {
            var collection = await GetStandardImagesCollection();

            if (collection == null)
                return null;

            return collection
                .AsQueryable()
                .Where(x => x.ImageId == id)
                .FirstOrDefault();
        }

        public async Task<int[]> GetStandardImagesIds()
        {
            var collection = await GetStandardImagesCollection();

            if (collection == null)
                return null;

            return collection
                .AsQueryable()
                .Select(x => x.ImageId)
                .ToArray();
        }
    }
}
