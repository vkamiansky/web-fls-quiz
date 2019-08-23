define('apps/quiz/components/finish/finish', [],
    function() {
        'use strict';

        return function (params) {
            var self = this;

            self.finishScreenMessage = params && params.finishScreenMessage;

            return {
                finishScreenMessage: self.finishScreenMessage
            };
        };
    }
);
