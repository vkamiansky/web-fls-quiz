define(['knockout'], function componentRegistrator(ko) {
    'use strict';

    return {
        register: _register
    };

    function _register(component) {
        ko.components.register(component.name, {
            viewModel: { require: component.viewModelPath },
            template: { require: component.templatePath }
        });
    }

});
