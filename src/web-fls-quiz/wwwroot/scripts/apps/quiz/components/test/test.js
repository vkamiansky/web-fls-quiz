define('apps/quiz/components/test/test', [
        'knockout',
        'jquery',
        'json!/settings/quizOptions'
    ],
    function(ko, $, options) {
        'use strict';

        return function(params) {
            var self = this;

            var optionsParsed = JSON.parse(options);
            var settings = optionsParsed && optionsParsed.settings || {};

            self.loading = params && params.loading;
            self.userAnswers = params && params.userAnswers;
            self.showSubmit = params && params.showSubmit;
            self.showIntro = params && params.showIntro;
            self.addUserAnswer = params && params.addUserAnswer;
            self.showModalErrorMessage = params && params.showModalErrorMessage;

            self.currentQuestion = ko.observable();
            self.currentQuestionNumber = ko.observable(0);

            self.countOfQuestions = settings.countOfQuestions || 1;
            
            _loadQuestion.call(self);

            return {
                nextQuestionClick: _nextQuestionClick.bind(self),
                currentQuestion: self.currentQuestion,
                addUserAnswer: self.addUserAnswer,
                currentQuestionNumber: self.currentQuestionNumber,
                countOfQuestions: self.countOfQuestions
            };
        };

        function _nextQuestionClick() {
            var self = this;

            if (self.currentQuestionNumber() === self.countOfQuestions) {
                self.showSubmit();
                return;
            }

            _loadQuestion.call(self);
        }

        function _loadQuestion() {
            var self = this;
            var genericErrorMsg = 'Упс, что-то пошло не так. Приносим наши извинения за причиненные неудобства. :(';

            self.loading(true);
            $.post('/question/getRandom',
                    {
                        excludedQuestionsIds: self.userAnswers().map(_mapQuestionId)
                    },
                    function _onSuccess(result) {
                        if (!result || !result.question) {
                            self.showModalErrorMessage(genericErrorMsg, function() {
                                if (self.currentQuestionNumber() === 0) {
                                    self.showIntro();
                                }
                            });

                        }

                        self.currentQuestion(result.question);
                        _incrementCurrentQuestionNumber.call(self);
                    },
                    'json'
                )
                .fail(function _onError() {
                    self.showModalErrorMessage(genericErrorMsg, function() {
                        if (self.currentQuestionNumber() === 0) {
                            self.showIntro();
                        }
                    });
                })
                .always(function _always() {
                    self.loading(false);
                });
        }

        function _mapQuestionId(userAnswer) {
            return userAnswer.questionId;
        }

        function _incrementCurrentQuestionNumber() {
            var self = this;

            self.currentQuestionNumber(self.currentQuestionNumber() + 1);
        }
    }
);
