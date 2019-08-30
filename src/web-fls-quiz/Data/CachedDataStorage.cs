using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebFlsQuiz.Interfaces;
using WebFlsQuiz.Models;

namespace WebFlsQuiz.Data
{
    public class CachedDataStorage : IDataStorage
    {
        private const string _quizzesPrefix = "Quiz_";

        private const string _standardImagesPrefix = "StandardImage_";

        private readonly IDataStorage _innerDataStorage;

        private readonly IMemoryCache _memoryCache;

        private readonly Queue<QuizResult> _results;

        public CachedDataStorage(
            IConfigurationService configurationService,
            IMemoryCache memoryCache)
        {
            _innerDataStorage = new DataStorage(configurationService);
            _memoryCache = memoryCache;
            _results = new Queue<QuizResult>();
        }

        public async Task<QuestionData> GetQuestion(string quizName, int id)
        {
            var quiz = await GetQuiz(quizName);
            if (quiz == null)
                return null;

            return quiz
                .Questions
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        public async Task<int?> GetQuestionsNumber(string quizName)
        {
            var quiz = await GetQuiz(quizName);
            if (quiz == null)
                return null;

            return quiz
                .Questions
                .Length;
        }

        public async Task<QuizInfo> GetQuiz(string quizName)
        {
            var key = _quizzesPrefix + quizName;

            var cached = _memoryCache.TryGetValue(key, out QuizInfo quiz);
            if (cached)
                return quiz;

            quiz = await _innerDataStorage.GetQuiz(quizName);
            _memoryCache.Set(key, quiz);
            return quiz;
        }

        public async Task<StandardImage> GetStandardImage(int id)
        {
            var key = _standardImagesPrefix + id.ToString();

            var cached = _memoryCache.TryGetValue(key, out StandardImage image);
            if (cached)
                return image;

            image = await _innerDataStorage.GetStandardImage(id);
            _memoryCache.Set(key, image);
            return image;
        }

        public Task<int[]> GetStandardImagesIds()
        {
            return _innerDataStorage.GetStandardImagesIds();
        }

        public Task<int[]> GetStandardImagesIds(ImageType imageType)
        {
            return _innerDataStorage.GetStandardImagesIds(imageType);
        }

        public async Task<bool> InsertQuizResult(QuizResult quizResult)
        {
            var inserted = await _innerDataStorage.InsertQuizResult(quizResult);

            if (!inserted)
                _results.Enqueue(quizResult);

            return true;
        }

        public async Task<bool> TryInsertResult()
        {
            var success = _results.TryDequeue(out QuizResult result);
            if (!success)
                return false;

            success = await _innerDataStorage.InsertQuizResult(result);
            if (!success)
            {
                _results.Enqueue(result);
                return false;
            }

            return true;
        }
    }
}
