define('apps/quiz/components/question/question', [
        'knockout'
    ],
    function(ko) {
        'use strict';

        return function(params) {
            var self = this;
            
            var model = params && params.question || {};

            self.id = model.id;
            self.text = model.text;
            self.imageBase64 = model.imageBase64;
            self.answers = model.answers || [];
            self.selectedAnswerId = ko.observable();
            self.multipleAnswer = model.multipleAnswer;

            var answersArray = [];
            self.answers.forEach(function (element) {
                answersArray.push(new Answer(element.text, false, element.answerId));
            });

            self.answers = ko.observableArray(answersArray);
            
            self.nextQuestionHandler = params && params.nextQuestionHandler;
            self.addUserAnswer = params && params.addUserAnswer;
            self.currentQuestionNumber = params && params.currentQuestionNumber;
            self.countOfQuestions = params && params.countOfQuestions;

            var answerButtonClick = _answerButtonClick.bind(self);

            window.scrollTo(0, 0);

            return {
                text: self.text,
                imageBase64: self.imageBase64,
                answers: self.answers,
                selectedAnswerId: self.selectedAnswerId,
                answerButtonClick: answerButtonClick,
                countOfQuestions: self.countOfQuestions,
                currentQuestionNumber: self.currentQuestionNumber,
                multipleAnswer: self.multipleAnswer
            };
        };

        function _answerButtonClick() {
            var self = this;

            var selectedAnswerId = self.selectedAnswerId();

            var answers = self.answers();
            var i = 0;
            answers.forEach(function (element) {
                if (element.isChecked) {
                    i++;
                }
            });

            if (!selectedAnswerId && i == 0) {
                return;
            }

            if (self.multipleAnswer) {
                var userAnswers = [];
                answers.forEach(function (element) {
                    if (element.isChecked) {
                        userAnswers.push(element.answerId);
                    }
                });
                self.addUserAnswer(self.id, userAnswers);
            }
            else {
                self.addUserAnswer(self.id, selectedAnswerId);
            }
            self.nextQuestionHandler();
        }

        function Answer(text, isChecked, answerId) {
            this.text = text;
            this.isChecked = isChecked;
            this.answerId = answerId;
        }
    }
);
