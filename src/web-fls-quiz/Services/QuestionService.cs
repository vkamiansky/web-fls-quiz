using System;
using System.Linq;
using WebFlsQuiz.Interfaces;
using WebFlsQuiz.Models;

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
        public IOperationResult<QuizInfo> GetQuizInfo(string quizName)
        {
            return _dataStorage.GetQuiz(quizName.ToLower())
            .Bind(quiz =>
            {
                return OperationResult.All(new Func<IOperationResult>[]
                {
                    () => _imageService.LoadIfNeeded(quiz.LogoImage),
                    () => _imageService.LoadIfNeeded(quiz.FinishScreenImage),
                    () => _imageService.LoadIfNeeded(quiz.IntroScreenImage)
                })
                .Bind(() => quiz.ToResult());
            });
        }
        public IOperationResult<Question> GetRandom(int[] excludedQuestionIds, string quizName)
        {
            return _dataStorage.GetQuestionsNumber(quizName)
                .Bind(numberOfQuestions => OperationResult.Try(() =>
                    Enumerable.Range(1, numberOfQuestions)
                        .Except(excludedQuestionIds)
                        .ToArray()
                        .ToResult()))
                .Bind(availableIds =>
                {
                    var nextQuestionIdPosition = _random.Next(0, availableIds.Length);
                    return _dataStorage.GetQuestion(quizName, availableIds[nextQuestionIdPosition]);
                })
                .Bind(question => OperationResult.Try(() =>
                {
                    _imageService.LoadIfNeeded(question.Image);
                    return new Question
                    {
                        Id = question.Id,
                        ImageBase64 = question.Image.ImageBase64,
                        Text = question.Text,
                        Answers = Array.ConvertAll(
                            question.Answers,
                            x => new Answer
                            {
                                AnswerId = x.AnswerId,
                                QuestionId = x.QuestionId,
                                Text = x.Text
                            }),
                        MultipleAnswer = question.MultipleAnswer
                    }.ToResult();
                }));
        }
        public IOperationResult<UserResult> GetUserResult(UserAnswer[] userAnswers, string quizName)
        {
            return OperationResult.Try(() => OperationResult.All(
                userAnswers.Select<UserAnswer, Func<IOperationResult<QuestionResult>>>(userAnswer => () =>
                {
                    return _dataStorage.GetQuestion(quizName, userAnswer.QuestionId).Bind(questionData =>
                    new QuestionResult
                    {
                        QuestionText = questionData.Text,
                        AnswerResults = Array.ConvertAll(
                            questionData.Answers,
                            y => new AnswerResult
                            {
                                AnswerText = y.Text,
                                IsCorrect = y.IsCorrect,
                                IsUserChosen = userAnswer.AnswerIds.Contains(y.AnswerId)
                            }),
                    }.ToResult());
                })))
                .Bind(questionResults => new UserResult { QuestionResults = questionResults }.ToResult());
        }
    }
}
