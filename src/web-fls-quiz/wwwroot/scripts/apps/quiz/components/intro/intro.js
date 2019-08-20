define('apps/quiz/components/intro/intro', [],
    function() {
        'use strict';

        return function(params) {
            var showTest = params && params.showTest;

            //self.quizGreeting = ko.observable(params.quizGreeting);

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
