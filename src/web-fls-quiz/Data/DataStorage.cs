using WebFlsQuiz.Interfaces;
using WebFlsQuiz.Models;
using MongoDB.Driver;
using System.Linq;
using System;

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
        private IOperationResult<IMongoDatabase> GetDatabase()
        {
            return OperationResult.All(new Func<IOperationResult<string>>[]{
                () => OperationResult.Try(() => _configurationService.GetString(ConfigurationKey.DbConnectionString)),
                () => OperationResult.Try(() => _configurationService.GetString(ConfigurationKey.DbName))
            })
            .Bind(config => new
            {
                ConnectionString = config[0],
                DbName = config[1]
            }.ToResult())
            .Bind(dbConfig =>
                    OperationResult.Try(() => new MongoClient(dbConfig.ConnectionString).ToResult())
                        .Bind(client => OperationResult.Try(() => client.GetDatabase(dbConfig.DbName).ToResult())));
        }
        private IOperationResult<IMongoCollection<T>> GetCollection<T>(IMongoDatabase database, string collectionName)
        {
            return OperationResult.Try(() => database.GetCollection<T>(collectionName).ToResult());
        }
        private IOperationResult<IMongoCollection<QuizInfo>> GetQuizzesCollection()
        {
            return GetDatabase().Bind(db => GetCollection<QuizInfo>(db, _quizzesCollectionName));
        }
        private IOperationResult<IMongoCollection<QuizResult>> GetQuizResultsCollection()
        {
            return GetDatabase().Bind(db => GetCollection<QuizResult>(db, _quizResultsCollectionName));
        }
        private IOperationResult<IMongoCollection<StandardImage>> GetStandardImagesCollection()
        {
            return GetDatabase().Bind(db => GetCollection<StandardImage>(db, _standardImagesCollectionName));
        }
        public IOperationResult<QuestionData> GetQuestion(string quizName, int id)
        {
            return GetQuizzesCollection().Bind(collection => OperationResult.Try(() =>
                collection.AsQueryable()
                    .Where(x => x.Name == quizName)
                    .SelectMany(x => x.Questions)
                    .Where(x => x.Id == id)
                    .First()
                    .ToResult()));
        }

        public IOperationResult<int> GetQuestionsNumber(string quizName)
        {
            return GetQuizzesCollection().Bind(collection => OperationResult.Try(() =>
                collection.AsQueryable()
                    .Where(x => x.Name == quizName)
                    .SelectMany(x => x.Questions)
                    .Count()
                    .ToResult()));
        }

        public IOperationResult<QuizInfo> GetQuiz(string quizName)
        {
            return GetQuizzesCollection().Bind(collection => OperationResult.Try(() =>
                collection.AsQueryable()
                    .AsQueryable()
                    .Where(x => string.Equals(x.Name, quizName))
                    .First()
                    .ToResult()));
        }

        public IOperationResult InsertQuizResult(QuizResult quizResult)
        {
            return GetQuizResultsCollection().Bind(collection => OperationResult.Try(() =>
            {
                collection.InsertOne(quizResult);
                return OperationResult.Success();
            }));
        }

        public IOperationResult<StandardImage> GetStandardImage(int id)
        {
            return GetStandardImagesCollection().Bind(collection => OperationResult.Try(() =>
                collection.AsQueryable()
                    .Where(x => x.Id == id)
                    .First()
                    .ToResult()));
        }

        public IOperationResult<int[]> GetStandardImagesIds()
        {
            return GetStandardImagesCollection().Bind(collection => OperationResult.Try(() =>
                collection.AsQueryable()
                    .Select(x => x.Id)
                    .ToArray()
                    .ToResult()));
        }

        public IOperationResult<int[]> GetStandardImagesIds(ImageType imageType)
        {
            return GetStandardImagesCollection().Bind(collection => OperationResult.Try(() =>
                collection.AsQueryable()
                    .Where(x => x.ImageType == imageType)
                    .Select(x => x.Id)
                    .ToArray()
                    .ToResult()));
        }
    }
}
