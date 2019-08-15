define('apps/quiz/components/intro/intro', [],
    function() {
        'use strict';

        return function(params) {
            var showTest = params && params.showTest;

            return {
                startTestButtonClick: _startTestButtonClick.bind(null, showTest),
                quizGreeting: params && params.quizGreeting
            };
        };

        function _startTestButtonClick(showTestCallback) {
            showTestCallback && showTestCallback();
        }
    }
);
