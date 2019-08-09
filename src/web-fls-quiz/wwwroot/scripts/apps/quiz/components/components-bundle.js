define('text!apps/quiz/components/finish/finish.html', [], function () { return '<div class="content-v-centered indent-b"><div class="bg-colored indent-all"><div class=felix-cat></div><h2 class=centered-h><br>Результаты были отправлены на указанный email адрес.<br><br>Спасибо за участие в нашей викторине!</h2></div></div>';});define('text!apps/quiz/components/intro/intro.html', [], function () { return '<div class="content-v-centered indent-b"><div class="bg-colored indent-all"><h2 class=centered-h>Приветствуем вас в нашем космическом пространстве!</h2><div class=felix></div><p class="centered-h content-felix" data-bind="text: quizGreeting"><div class=centered-h><input readonly class=form-button data-bind="click: startTestButtonClick" onfocus=this.blur() value=Начать></div></div></div>';});define('text!apps/quiz/components/question/question.html', [], function () { return '<div class=polls-form><!--ko if: text.length--><div class=polls-form-header><h2 data-bind="text: text"></h2></div><!--/ko--><div class="polls-form-content indent-all"><div class=polls-img-container><img class=polls-img alt=img data-bind="attr: {src: \'data:image/png;base64,\' + imageBase64, alt: currentQuestionNumber() }"></div><div class=polls-counter><span class=current data-bind="text: currentQuestionNumber"></span> из <span data-bind="text: countOfQuestions"></span></div><ul class=polls-ul data-bind="foreach: {data: answers, as: \'answer\'}"><li class=polls-li><input type=radio data-bind="value: answer.answerId, checked: $parent.selectedAnswerId, attr: {id: \'poll_\' + answer.answerId}"><label data-bind="attr: {for: \'poll_\' + answer.answerId}, text: answer.text"></label></ul><div class=polls-buttons><input readonly class=form-button onfocus=this.blur() value=Продолжить data-bind="enable: selectedAnswerId, click: answerButtonClick"></div></div></div>';});define('text!apps/quiz/components/submit/submit.html', [], function () { return '<div class="content-v-centered indent-b w50">    <div class="bg-colored">        <div class="sign-up-form">            <h2>С прибытием!<br>Ваши ответы приняты! Заполните форму ниже, и письмо с вашим результатом будет немедленно отправлено. Показав его представителю FLS, вы получите подарок от нашей дружной компании.</h2>            <div class="fields">                <div class="field">                    <label class="field-label" for="form-name">Имя *</label>                    <input class="field-input" type="text" data-bind="value: name" onkeyup="this.setAttribute(\'value\', this.value);" value="" required placeholder="Имя…*" id="form-name">                </div>                <div class="form-tip field-error" data-bind="validationMessage: name"></div>                <div class="field">                    <label class="field-label" for="form-email">E-mail *</label>                    <input class="field-input" data-bind="value: email" onkeydown="return event.key != \' \'" onkeyup="this.setAttribute(\'value\', this.value);" value="" placeholder="me@example.com…*" id="form-email">                </div>                <div class="form-tip field-error" data-bind="validationMessage: email"></div>                <div class="form-tip">Результаты будут отправлены на указанный почтовый ящик</div>                <div class="field margin-t">                    <label class="field-label" for="form-message">Сообщение</label>                    <textarea class="field-textarea" rows="10" cols="45" data-bind="value: comment" onkeyup="this.setAttribute(\'value\', this.value);" value="" placeholder="Пара слов о себе (необязательно)" id="form-message"></textarea>                </div>            </div>            <div class="sign-up-buttons">                <input readonly class="form-button" data-bind="click: submitButtonClick" onfocus="this.blur()" value="Отправить" />            </div>        </div>    </div></div>';});define('text!apps/quiz/components/test/test.html', [], function () { return '<div class="content-v-centered indent-b"><div class=bg-colored><!--ko if: currentQuestion--> <!--ko component: {                 name: "question",                 params: {                     question: currentQuestion(),                     nextQuestionHandler: nextQuestionClick,                     addUserAnswer: addUserAnswer,                     currentQuestionNumber: currentQuestionNumber,                     countOfQuestions: countOfQuestions                 }             }--> <!--/ko--> <!--/ko--></div></div>';});
define('apps/quiz/components/finish/finish',[],function(){'use strict';return function(){return{};};});define('apps/quiz/components/intro/intro',[],function(){'use strict';return function(params){var showTest=params&&params.showTest;return{startTestButtonClick:_startTestButtonClick.bind(null,showTest),quizName:params&&params.quizName,quizGreeting:params&&params.quizGreeting};};function _startTestButtonClick(showTestCallback){showTestCallback&&showTestCallback();}});define('apps/quiz/components/question/question',['knockout'],function(ko){'use strict';return function(params){var self=this;var model=params&&params.question||{};self.id=model.id;self.text=model.text;self.imageBase64=model.imageBase64;self.answers=model.answers||[];self.selectedAnswerId=ko.observable();self.nextQuestionHandler=params&&params.nextQuestionHandler;self.addUserAnswer=params&&params.addUserAnswer;self.currentQuestionNumber=params&&params.currentQuestionNumber;self.countOfQuestions=params&&params.countOfQuestions;var answerButtonClick=_answerButtonClick.bind(self);window.scrollTo(0,0);return{text:self.text,imageBase64:self.imageBase64,answers:self.answers,selectedAnswerId:self.selectedAnswerId,answerButtonClick:answerButtonClick,countOfQuestions:self.countOfQuestions,currentQuestionNumber:self.currentQuestionNumber};};function _answerButtonClick(){var self=this;var selectedAnswerId=self.selectedAnswerId();if(!selectedAnswerId){return;}
self.addUserAnswer(self.id,selectedAnswerId);self.nextQuestionHandler();}});define('apps/quiz/components/submit/submit',['knockout','jquery','knockout.validation'],function(ko,$){'use strict';return function(params){var self=this;self.email=ko.observable();self.name=ko.observable();self.comment=ko.observable();_initValidation.call(self);self.loading=params&&params.loading;self.userAnswers=params&&params.userAnswers||[];self.showFinish=params&&params.showFinish;self.showModalErrorMessage=params&&params.showModalErrorMessage;self.isReadyForSubmit=ko.pureComputed(_isReadyForSubmit.bind(self));var submitButtonClick=_submit.bind(self);window.scrollTo(0,0);return{email:self.email,name:self.name,comment:self.comment,submitButtonClick:submitButtonClick,isReadyForSubmit:self.isReadyForSubmit};};function _initValidation(){var self=this;ko.validation.init({insertMessages:false},true);self.name.extend({required:{message:'Имя должно быть заполнено!'}});self.email.extend({required:{message:'Email должен быть заполнен!'},email:{message:'Некорректный Email!'}});self.errors=ko.validation.group(self);}
function _isReadyForSubmit(){var self=this;return self.userAnswers&&ko.unwrap(self.userAnswers()).length&&self.isValid();}
function _submit(){var self=this;if(self.errors().length){self.errors.showAllMessages(true);return;}
_saveResults.call(self);}
function _saveResults(){var self=this;var genericErrorMsg='Упс, что-то пошло не так. Приносим наши извинения за причиненные неудобства. :(';self.loading(true);$.post('/result/saveResults',{email:self.email(),name:self.name(),comment:self.comment(),userAnswers:self.userAnswers()},function _onSuccess(response){if(response.hasErrors){if(response.usedEmail){self.showModalErrorMessage('Этот email уже участвовал. Чтобы отправить результаты повторно, нужно указать другой email.');return;}
var emailWasNotSent='Упс, что-то пошло не так. Произошла ошибка при отправке письма с результатами по указанному адресу. :(';if(response.mailSendError){self.showModalErrorMessage(emailWasNotSent);return;}
if(!response.mailSent){self.showModalErrorMessage(response.mailSendError?emailWasNotSent:genericErrorMsg);return;}
self.showModalErrorMessage(emailWasNotSent);return;}
self.showFinish();},'json').fail(function _onError(){self.showModalErrorMessage(genericErrorMsg);}).always(function _always(){self.loading(false);});}});define('apps/quiz/components/test/test',['knockout','jquery','json!/settings/quizOptions'],function(ko,$,options){'use strict';return function(params){var self=this;var optionsParsed=JSON.parse(options);var settings=optionsParsed&&optionsParsed.settings||{};self.loading=params&&params.loading;self.userAnswers=params&&params.userAnswers;self.showSubmit=params&&params.showSubmit;self.showIntro=params&&params.showIntro;self.addUserAnswer=params&&params.addUserAnswer;self.showModalErrorMessage=params&&params.showModalErrorMessage;self.quizId=params&&params.quizId;self.currentQuestion=ko.observable();self.currentQuestionNumber=ko.observable(0);self.countOfQuestions=settings.countOfQuestions||1;_loadQuestion.call(self);return{nextQuestionClick:_nextQuestionClick.bind(self),currentQuestion:self.currentQuestion,addUserAnswer:self.addUserAnswer,currentQuestionNumber:self.currentQuestionNumber,countOfQuestions:self.countOfQuestions};};function _nextQuestionClick(){var self=this;if(self.currentQuestionNumber()===self.countOfQuestions){self.showSubmit();return;}
_loadQuestion.call(self);}
function _loadQuestion(){var self=this;var genericErrorMsg='Упс, что-то пошло не так. Приносим наши извинения за причиненные неудобства. :(';self.loading(true);$.post('/question/getRandom',{excludedQuestionsIds:self.userAnswers().map(_mapQuestionId),quizId:self.quizId},function _onSuccess(result){if(!result||!result.question){self.showModalErrorMessage(genericErrorMsg,function(){if(self.currentQuestionNumber()===0){self.showIntro();}});}
self.currentQuestion(result.question);_incrementCurrentQuestionNumber.call(self);},'json').fail(function _onError(){self.showModalErrorMessage(genericErrorMsg,function(){if(self.currentQuestionNumber()===0){self.showIntro();}});}).always(function _always(){self.loading(false);});}
function _mapQuestionId(userAnswer){return userAnswer.questionId;}
function _incrementCurrentQuestionNumber(){var self=this;self.currentQuestionNumber(self.currentQuestionNumber()+1);}});
define('components', ['finish','intro','question','submit','test']);
define('finish', ['text!apps/quiz/components/finish/finish.html','apps/quiz/components/finish/finish']);define('intro', ['text!apps/quiz/components/intro/intro.html','apps/quiz/components/intro/intro']);define('question', ['text!apps/quiz/components/question/question.html','apps/quiz/components/question/question']);define('submit', ['text!apps/quiz/components/submit/submit.html','apps/quiz/components/submit/submit']);define('test', ['text!apps/quiz/components/test/test.html','apps/quiz/components/test/test']);