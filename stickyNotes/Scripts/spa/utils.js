define(["require", "exports"], function(require, exports) {
    exports.isIE = 0;

    function createAccessor(value) {
        return function () {
            return value;
        };
    }
    exports.createAccessor = createAccessor;

    function createObservable(value, _default) {
        if (_.isUndefined(value) || _.isNull(value)) {
            return ko.observable(_default);
        }

        if (ko.isSubscribable(value)) {
            return value;
        }

        return ko.observable(value);
    }
    exports.createObservable = createObservable;

    function createObservableArray(value, mapFunction, context) {
        if (typeof value === "undefined") {
            return ko.observableArray();
        }

        if (ko.isSubscribable(value) && _.isArray(value())) {
            return value;
        }

        if (_.isArray(value) && typeof mapFunction === "function") {
            value = _.map(value, mapFunction, context);
        }

        return ko.observableArray(value);
    }
    exports.createObservableArray = createObservableArray;

    function isUndefined(value) {
        return typeof value === "undefined";
    }
    exports.isUndefined = isUndefined;

    function isDate(value) {
        return (/\d{2,4}-\d{2}-\d{2}[T -_]\d{2}:\d{2}:\d{2}/).test(value);
    }
    exports.isDate = isDate;

    function isNullOrWhiteSpace(value) {
        return !value || (/^\s*$/).test(value);
    }
    exports.isNullOrWhiteSpace = isNullOrWhiteSpace;

    function extend(target) {
        var objs = [];
        for (var _i = 0; _i < (arguments.length - 1); _i++) {
            objs[_i] = arguments[_i + 1];
        }
        if (objs.length === 0) {
            return target;
        }

        _.each(objs, function (obj) {
            if (!obj) {
                return;
            }

            _.each(obj, function (value, key) {
                target[key] = value;
            });
        });

        return target;
    }
    exports.extend = extend;

    function inherits(obj, base, prototype) {
        if (_.isFunction(base.constructor)) {
            obj.prototype = new base();
            obj.prototype.constructor = obj;
            obj.prototype.parent = base.prototype;
        } else {
            obj.prototype = base;
            obj.prototype.constructor = obj;
            obj.prototype.parent = base;
        }

        if (prototype) {
            exports.extend(obj.prototype, prototype);
        }

        return obj;
    }
    exports.inherits = inherits;

    function unsafe(callback) {
        if (typeof MSApp === "undefined") {
            return callback.call(null);
        } else {
            return MSApp.execUnsafeLocalFunction(callback);
        }
    }
    exports.unsafe = unsafe;

    function getWindowSize() {
        var winW = 630, winH = 460;

        if (document.body && document.body.offsetWidth) {
            winW = document.body.offsetWidth;
            winH = document.body.offsetHeight;
        }

        if (document.compatMode === "CSS1Compat" && document.documentElement && document.documentElement.offsetWidth) {
            winW = document.documentElement.offsetWidth;
            winH = document.documentElement.offsetHeight;
        }

        if (window.innerWidth && window.innerHeight) {
            winW = window.innerWidth;
            winH = window.innerHeight;
        }

        return {
            width: winW,
            height: winH
        };
    }
    exports.getWindowSize = getWindowSize;

    function getQueryString(key) {
        var dictionary = {}, qs = window.location.search.replace("?", ""), pairs = qs.split("&");

        _.each(pairs, function (val) {
            var pair = val.split("=");
            dictionary[pair[0]] = pair[1];
        });

        if (key) {
            return dictionary[key];
        }

        return dictionary;
    }
    exports.getQueryString = getQueryString;

    function load() {
        var modules = [];
        for (var _i = 0; _i < (arguments.length - 0); _i++) {
            modules[_i] = arguments[_i + 0];
        }
        return $.Deferred(function (dfd) {
            var args = _.flatten(modules, true);

            try  {
                require(args, dfd.resolve);
            } catch (ex) {
                dfd.reject(ex);
            }
        }).promise();
    }
    exports.load = load;

    function format(text) {
        var args = [];
        for (var _i = 0; _i < (arguments.length - 1); _i++) {
            args[_i] = arguments[_i + 1];
        }
        return text.replace(/\{+-?[0-9]+(:[^}]+)?\}+/g, function (tag) {
            var match = tag.match(/(\{+)(-?[0-9]+)(:([^\}]+))?(\}+)/), index = parseInt(match[2], 10), value = args[index];

            if (match[1].length > 1 && match[5].length > 1) {
                return "{" + index + (match[3] || "") + "}";
            }

            if (typeof value === "undefined") {
                value = "";
            }

            if (match[3]) {
                switch (match[4]) {
                    case "U":
                        return value.toString().toUpperCase();
                    case "u":
                        return value.toString().toLowerCase();
                    default:
                        if (window.Globalize) {
                            return Globalize.format(value, match[4]);
                        }
                        break;
                }
            }

            return value;
        });
    }
    exports.format = format;

    function str_pad(text, length, char, right) {
        if (typeof right === "undefined") { right = false; }
        var str = "" + text;
        while (str.length < length) {
            str = right ? str + char : char + str;
        }

        return str;
    }
    exports.str_pad = str_pad;

    function arrayCompare(array1, array2) {
        return {
            added: _.difference(array2, array1),
            removed: _.difference(array1, array2)
        };
    }
    exports.arrayCompare = arrayCompare;

    function arrayEquals(array1, array2) {
        var report = exports.arrayCompare(array1, array2);
        return report.added.length === 0 && report.removed.length === 0;
    }
    exports.arrayEquals = arrayEquals;

    function whenAll(promises) {
        return $.when.apply($, promises);
    }
    exports.whenAll = whenAll;

    function wrapError(error) {
        return $.Deferred().reject(error).promise();
    }
    exports.wrapError = wrapError;

    function timeout(milliseconds) {
        if (typeof milliseconds === "undefined") { milliseconds = 1; }
        return $.Deferred(function (dfd) {
            setTimeout(function () {
                return dfd.resolve();
            }, milliseconds);
        }).promise();
    }
    exports.timeout = timeout;

    var vendorPrefix = null;

    function getVendorPrefix() {
        if (vendorPrefix !== null) {
            return vendorPrefix;
        }

        var regex = /^(moz|webkit|Moz|Webkit|Khtml|O|ms|Icab)(?=[A-Z])/, someScript = document.getElementsByTagName("script")[0];

        for (var prop in someScript.style) {
            if (regex.test(prop)) {
                return (vendorPrefix = prop.match(regex)[0]);
            }
        }

        if ("webkitOpacity" in someScript.style) {
            return (vendorPrefix = "webkit");
        }
        if ("KhtmlOpacity" in someScript.style) {
            return (vendorPrefix = "Khtml");
        }

        return (vendorPrefix = "");
    }
    exports.getVendorPrefix = getVendorPrefix;

    function prefixStyle(prop) {
        if ($.support[prop]) {
            return $.support[prop];
        }

        var vendorProp, supportedProp, capProp = prop.charAt(0).toUpperCase() + prop.slice(1), prefixes = ["webkit", "moz", "o", "Moz", "Webkit", "O", "ms"], div = document.createElement("div");

        if (prop in div.style) {
            supportedProp = prop;
        } else {
            for (var i = 0; i < prefixes.length; i++) {
                vendorProp = prefixes[i] + capProp;
                if (vendorProp in div.style) {
                    supportedProp = vendorProp;
                    break;
                }
            }
        }

        div = null;

        $.support[prop] = supportedProp;

        return supportedProp;
    }
    exports.prefixStyle = prefixStyle;

    function createCssHook(prop) {
        var property = exports.prefixStyle(prop);
        if (property && property !== prop) {
            $.cssHooks[prop] = {
                get: function (elem, computed, extra) {
                    return $.css(elem, property);
                },
                set: function (elem, value) {
                    elem.style[property] = value;
                }
            };
        }
    }
    exports.createCssHook = createCssHook;
});
