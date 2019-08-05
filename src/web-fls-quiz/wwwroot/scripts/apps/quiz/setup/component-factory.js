define([], function componentFactory() {
    'use strict';

    var componentBasePath = 'apps/quiz/components/';
    var componentViewModelFormat = componentBasePath + '{0}/{0}';
    var componentTemplateFormat = 'text!' + componentBasePath + '{0}/{0}.html';

    return {
        create: _create
    };

    function _create(name) {
        return {
            name: name,
            viewModelPath: _format(componentViewModelFormat, [{ from: '{0}', to: name }]),
            templatePath: _format(componentTemplateFormat, [{ from: '{0}', to: name }])
        };
    }

    function _format(string, replacements) {
        var result = replacements.reduce(_replaceReplacement, string);

        return result;
    }

    function _replaceReplacement(result, replacement) {
        var regexp = new RegExp(_escapeRegExp(replacement.from), 'g');
        return result.replace(regexp, replacement.to);
    }

    function _escapeRegExp(string) {
        return string.replace(/[{}]/g, '\\$&');
    }
});
