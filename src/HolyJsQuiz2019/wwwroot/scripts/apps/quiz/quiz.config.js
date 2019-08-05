(function quizConfigModule() {
    'use strict';

    require.config({
        baseUrl: '/scripts',
        paths: {
            'knockout': 'vendor/knockout-3.4.2',
            'jquery': 'vendor/jquery-3.3.1.min',
            'knockout.validation': 'vendor/plugins/knockout.validation',
            'jquery.modal': 'vendor/plugins/jquery.modal.min',
            'text': 'vendor/plugins/text',
            'json': 'vendor/plugins/json'
        },
        shim: {
            'jquery.modal': {
                deps: ['jquery']
            }
        }
    });

    require([
            'apps/quiz/quiz.app',
            'apps/quiz/setup/component-factory',
            'apps/quiz/setup/component-registrator',
            'components'
        ],
        function _(quizApp, componentFactory, componentRegistrator) {

            _initComponents(componentFactory, componentRegistrator);

            var domElement = 'quiz-app';
            var settings = {};

            quizApp.initialize(domElement, settings);
        }
    );

    function _initComponents(componentFactory, componentRegistrator) {
        var componentNames = ['intro', 'test', 'question', 'submit', 'finish'];
        var components = componentNames.map(function _createComponent(componentName) {
            return componentFactory.create(componentName);
        });

        components.map(function _registerComponent(component) {
            componentRegistrator.register(component);
        });
    }
})();
