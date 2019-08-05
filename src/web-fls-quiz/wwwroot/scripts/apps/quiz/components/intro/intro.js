define([],
    function() {
        'use strict';

        return function(params) {
            var showTest = params && params.showTest;

            return {
                startTestButtonClick: _startTestButtonClick.bind(null, showTest)
            };
        };

        function _startTestButtonClick(showTestCallback) {
            showTestCallback && showTestCallback();
        }
    }
);
