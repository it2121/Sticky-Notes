define(["require", "exports"], function(require, exports) {
    var ChangeTracker = (function () {
        function ChangeTracker(object, isAlreadyModified, hashFunction, params) {
            if (typeof isAlreadyModified === "undefined") { isAlreadyModified = false; }
            if (typeof hashFunction === "undefined") { hashFunction = ko.toJSON; }
            this.hashFunction = hashFunction;
            this.params = params;
            this.tracked = object;
            this.lastData = ko.observable(hashFunction(object, params));
            this.isModified = ko.observable(isAlreadyModified);

            this.hasChanges = ko.computed(function () {
                return this.isModified() || this.hashFunction(this.tracked, this.params) !== this.lastData();
            }, this);
        }
        ChangeTracker.prototype.reset = function () {
            this.lastData(this.hashFunction(this.tracked, this.params));
            this.isModified(false);
        };
        return ChangeTracker;
    })();

    
    return ChangeTracker;
});
