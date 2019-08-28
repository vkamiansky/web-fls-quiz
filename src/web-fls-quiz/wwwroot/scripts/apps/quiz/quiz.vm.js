define([
        'knockout',
        'jquery',
        'jquery.modal'
    ],
    function quizViewModelModule(ko, $) {
        'use strict';

        return QuizViewModel;

        function QuizViewModel() {

            var self = this;

            self.loading = ko.observable(false);

            self.isIntroActive = ko.observable(true);
            self.isTestActive = ko.observable(false);
            self.isSubmitActive = ko.observable(false);
            self.isFinishActive = ko.observable(false);

            self.modalErrorMessage = ko.observable();
            self.showModalErrorMessage = _showModalErrorMessage.bind(self);
            
            self.userAnswers = ko.observableArray([]);

            self.showIntro = _showIntro.bind(self);
            self.showTest = _showTest.bind(self);
            self.showSubmit = _showSubmit.bind(self);
            self.showFinish = _showFinish.bind(self);
            self.addUserAnswer = _addUserAnswer.bind(self);
        }

        function _showIntro() {
            var self = this;

            self.isSubmitActive(false);
            self.isFinishActive(false);
            self.isTestActive(false);
            self.isIntroActive(true);
        }

        function _showTest() {
            var self = this;

            self.isIntroActive(false);
            self.isSubmitActive(false);
            self.isFinishActive(false);
            self.isTestActive(true);
        }

        function _showSubmit() {
            var self = this;

            self.isIntroActive(false);
            self.isTestActive(false);
            self.isFinishActive(false);
            self.isSubmitActive(true);
        }

        function _showFinish() {
            var self = this;

            self.isIntroActive(false);
            self.isTestActive(false);
            self.isSubmitActive(false);
            self.isFinishActive(true);
        }

        function _addUserAnswer(questionId, answerIds) {
            var self = this;

            self.userAnswers.push({ questionId: questionId, answerIds: answerIds });
        }

        function _showModalErrorMessage(message, closeCallback) {
            var self = this;

            self.modalErrorMessage(message);
            if (closeCallback)
                $('#modalError').on($.modal.AFTER_CLOSE, function(event, modal) {
                    $('#modalError').off($.modal.AFTER_CLOSE);
                    window.setTimeout(function() { closeCallback(event, modal); }, 0);
                });

            $("#modalError").modal({ closeClass: 'icon-remove', closeText: '&times' });
        }
    }
);