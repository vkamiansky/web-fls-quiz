using WebFlsQuiz.Interfaces;
using WebFlsQuiz.Models;
using System;
using System.Linq;

namespace WebFlsQuiz.Services
{
    public class QuestionService : IQuestionService
    {
        private Random _random = new Random();

        private IDataStorage _dataStorage;

        public QuestionService(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }
        
        public Question GetRandom(int[] excludedQuestionIds, string quizName)
        {
            var availableIds = Enumerable
                .Range(1, _dataStorage.GetQuestionsNumber(quizName).Value)
                .Except(excludedQuestionIds)
                .ToArray();
            var nextQuestionIdPosition = _random.Next(0, availableIds.Length);
            var questionData = _dataStorage.GetQuestion(quizName, availableIds[nextQuestionIdPosition]);

            var imageBase64 = questionData.ImageBase64;
            if (questionData.ImageId != 0)
            {
                imageBase64 = _dataStorage.GetStandardImage(questionData.ImageId).Result.ImageBase64;
            }
            else
            {
                if (string.IsNullOrEmpty(questionData.ImageBase64))
                {
                    var standardImagesNumber = _dataStorage.GetStandardImagesNumber().Result.Value;
                    var randomImageId = _random.Next(1, standardImagesNumber);
                    imageBase64 = _dataStorage.GetStandardImage(randomImageId).Result.ImageBase64;
                }
            }

            return new Question
            {
                Id = questionData.Id,
                ImageBase64 = imageBase64,
                Text = questionData.Text,
                Answers = Array.ConvertAll(
                    questionData.Answers,
                    x => new Answer
                    {
                        AnswerId = x.AnswerId,
                        QuestionId = x.QuestionId,
                        Text = x.Text
                    })
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
                                IsUserChosen = y.AnswerId == x.AnswerId
                            }),
                    };
                })
            };
        }
    }
}
