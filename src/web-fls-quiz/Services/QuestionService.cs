using WebFlsQuiz.Interfaces;
using WebFlsQuiz.Models;
using System;
using System.Linq;

namespace WebFlsQuiz.Services
{
    public class QuestionService : IQuestionService
    {
        private Random _random = new Random();

        private readonly IDataStorage _dataStorage;

        private readonly IImageService _imageService;

        public QuestionService(
            IDataStorage dataStorage,
            IImageService imageService)
        {
            _dataStorage = dataStorage;
            _imageService = imageService;
        }
        
        public Question GetRandom(int[] excludedQuestionIds, string quizName)
        {
            var availableIds = Enumerable
                .Range(1, _dataStorage.GetQuestionsNumber(quizName).Value)
                .Except(excludedQuestionIds)
                .ToArray();
            var nextQuestionIdPosition = _random.Next(0, availableIds.Length);
            var questionData = _dataStorage.GetQuestion(quizName, availableIds[nextQuestionIdPosition]);

            _imageService.LoadIfNeeded(questionData.Image);

            return new Question
            {
                Id = questionData.Id,
                ImageBase64 = questionData.Image.ImageBase64,
                Text = questionData.Text,
                Answers = Array.ConvertAll(
                    questionData.Answers,
                    x => new Answer
                    {
                        AnswerId = x.AnswerId,
                        QuestionId = x.QuestionId,
                        Text = x.Text
                    }),
                MultipleAnswer = questionData.MultipleAnswer
            };
        }

        public UserResult GetUserResult(UserAnswer[] userAnswers, string quizName)
        {
            return new UserResult
            {
                QuestionResults = Array.ConvertAll(
                userAnswers,
                x =>
                {
                    var questionData = _dataStorage.GetQuestion(quizName, x.QuestionId);
                    return new QuestionResult
                    {
                        QuestionText = questionData.Text,
                        AnswerResults = Array.ConvertAll(
                            questionData.Answers,
                            y => new AnswerResult
                            {
                                AnswerText = y.Text,
                                IsCorrect = y.IsCorrect,
                                IsUserChosen = x.AnswerIds.Contains(y.AnswerId)
                            }),
                    };
                })
            };
        }
    }
}
