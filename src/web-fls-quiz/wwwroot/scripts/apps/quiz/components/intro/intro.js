define('apps/quiz/components/intro/intro', [],
    function() {
        'use strict';

        return function (params) {
            var self = this;

            var showTest = params && params.showTest;
            self.quizGreeting = params && params.quizGreeting;
            self.introScreenImageBase64 = params && params.introScreenImageBase64;

            return {
                startTestButtonClick: _startTestButtonClick.bind(null, showTest),
                quizGreeting: self.quizGreeting,
                introScreenImageBase64: self.introScreenImageBase64
            };
        };

        function _startTestButtonClick(showTestCallback) {
            showTestCallback && showTestCallback();
        }
    }
);
