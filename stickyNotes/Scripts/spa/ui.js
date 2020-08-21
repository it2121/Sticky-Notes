define(["require", "exports", "./utils"], function(require, exports, __utils__) {
    var utils = __utils__;

    function addTemplate(id, template, engine) {
        if (engine && engine.addTemplate) {
            engine.addTemplate(id, template);
            return;
        }

        if ($("#" + id).length === 0) {
            utils.unsafe(function () {
                $("body").append("<script id='" + id + "' type='text/html'>" + template + "</script>");
            });
        }
    }
    exports.addTemplate = addTemplate;

    function load() {
        var widgets = [];
        for (var _i = 0; _i < (arguments.length - 0); _i++) {
            widgets[_i] = arguments[_i + 0];
        }
        widgets = _.map(widgets, function (widget) {
            return "./ui/" + widget;
        });
        return utils.load.apply(null, widgets);
    }
    exports.load = load;
});
