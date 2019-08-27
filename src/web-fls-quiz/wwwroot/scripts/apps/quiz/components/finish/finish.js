define('apps/quiz/components/finish/finish', [],
    function() {
        'use strict';

        return function (params) {
            var self = this;

            self.finishScreenMessage = params && params.finishScreenMessage;
            self.finishScreenImageBase64 = params && params.finishScreenImageBase64;

            return {
                finishScreenMessage: self.finishScreenMessage,
                finishScreenImageBase64: self.finishScreenImageBase64
            };
        };
    }
);
