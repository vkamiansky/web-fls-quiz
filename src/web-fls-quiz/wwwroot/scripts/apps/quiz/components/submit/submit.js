define('apps/quiz/components/submit/submit', [
        'knockout',
        'jquery',
        'knockout.validation'
    ],
    function(ko, $) {
        'use strict';

        return function(params) {
            var self = this;

            self.email = ko.observable();
            self.name = ko.observable();
            self.comment = ko.observable();

            _initValidation.call(self);
            
            self.loading = params && params.loading;
            self.userAnswers = params && params.userAnswers || [];
            self.showFinish = params && params.showFinish;
            self.showModalErrorMessage = params && params.showModalErrorMessage;
            self.quizName = params && params.quizName;
            self.submitScreenMessage = params && params.submitScreenMessage;

            self.isReadyForSubmit = ko.pureComputed(_isReadyForSubmit.bind(self));

            var submitButtonClick = _submit.bind(self);

            window.scrollTo(0, 0);

            return {
                email: self.email,
                name: self.name,
                comment: self.comment,
                submitButtonClick: submitButtonClick,
                isReadyForSubmit: self.isReadyForSubmit,
                submitScreenMessage: self.submitScreenMessage
            };
        };

        function _initValidation() {
            var self = this;
            
            ko.validation.init({ insertMessages: false }, true);
            
            self.name.extend({
                required: { message: 'Имя должно быть заполнено!' }
            });
            
            self.email.extend({
                required: { message: 'Email должен быть заполнен!' },
                email: { message: 'Некорректный Email!' }
            });

            self.errors = ko.validation.group(self);
        }

        function _isReadyForSubmit() {
            var self = this;

            return self.userAnswers &&
                ko.unwrap(self.userAnswers()).length &&
                self.isValid();
        }

        function _submit() {
            var self = this;

            if (self.errors().length) {
                self.errors.showAllMessages(true);
                return;
            }

            _saveResults.call(self);
        }

        function _saveResults() {
            var self = this;
            var genericErrorMsg = 'Упс, что-то пошло не так. Приносим наши извинения за причиненные неудобства. :(';

            self.loading(true);
            $.post('/result/saveResults',
                    {
                        email: self.email(),
                        name: self.name(),
                        comment: self.comment(),
                        userAnswers: self.userAnswers(),
                        quizName: self.quizName
                    },
                    function _onSuccess(response) {
                        if (response.hasErrors) {
                            if (response.usedEmail) {
                                self.showModalErrorMessage('Этот email уже участвовал. Чтобы отправить результаты повторно, нужно указать другой email.');
                                return;
                            }

                            var emailWasNotSent = 'Упс, что-то пошло не так. Произошла ошибка при отправке письма с результатами по указанному адресу. :(';
                            if (response.mailSendError) {
                                self.showModalErrorMessage(emailWasNotSent);
                                return;
                            }

                            if (!response.mailSent) {
                                self.showModalErrorMessage(response.mailSendError
                                    ? emailWasNotSent
                                    : genericErrorMsg);
                                return;
                            }

                            self.showModalErrorMessage(emailWasNotSent);
                            return;
                        }

                        self.showFinish();
                    },
                    'json'
                )
                .fail(function _onError() {
                    self.showModalErrorMessage(genericErrorMsg);
                })
                .always(function _always() {
                    self.loading(false);
                });
        }
    }
);
