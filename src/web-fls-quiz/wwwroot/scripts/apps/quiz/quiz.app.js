define([
        'knockout',
        'apps/quiz/quiz.vm'
    ],
    function quizAppModule(ko, QuizViewModel) {
        'use strict';

        return {
            initialize: _initialize
        };

        function _initialize(domElement, settings) {
            console.log('Quiz App loaded');

            var quiz = new QuizViewModel();

            ko.applyBindings(quiz, document.getElementById(domElement));
        }

    }
);
