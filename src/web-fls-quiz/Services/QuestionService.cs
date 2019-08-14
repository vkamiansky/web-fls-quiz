﻿using WebFlsQuiz.Interfaces;
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
        
        public Question GetRandom(int[] excludedQuestionIds, int quizId)
        {
            var availableIds = _dataStorage.GetQuestionIds(quizId).ToList().Except(excludedQuestionIds).ToArray();
            var nextQuestionIdPosition = _random.Next(0, availableIds.Length);
            var questionData = _dataStorage.GetQuestion(availableIds[nextQuestionIdPosition]);
            return new Question
            {
                Id = questionData.Id,
                ImageBase64 = questionData.ImageBase64,
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

        public UserResult GetUserResult(UserAnswer[] userAnswers)
        {
            return new UserResult
            {
                QuestionResults = Array.ConvertAll(
                userAnswers,
                x =>
                {
                    var questionData = _dataStorage.GetQuestion(x.QuestionId);
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