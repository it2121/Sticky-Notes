define(["require", "exports", "./utils"], function(require, exports, __utils__) {
    var utils = __utils__;

    var stores = {};
    function createFromIStorage(type, storage) {
        stores[type] = storage;
    }

    var MemoryStorage = (function () {
        function MemoryStorage() {
            this.memory = {};
            this.timeouts = {};
            this.length = 0;
            this.clone = function (obj) {
                return obj === undefined ? undefined : JSON.parse(JSON.stringify(obj));
            };
            this.key = function (index) {
                return _.find(_.values(this.memory), function (val, i) {
                    return i === index;
                });
            };
            this.getItem = function (key) {
                return this.clone(this.memory[key]);
            };
            this.setItem = function (key, value) {
                this.memory[key] = value;
            };
            this.removeItem = function (key) {
                delete this.memory[key];
            };
            this.clear = function () {
                this.memory = {};
                return null;
            };
        }
        return MemoryStorage;
    })();
    createFromIStorage("memory", new MemoryStorage());

    var WebSQLStorage = (function () {
        function WebSQLStorage() {
            this.memory = {};
            this.db = null;
            this.length = 0;
            this.dbname = "spastore";
            this.tablename = "storetable";
            this.dbsize = 10 * 1024 * 1024;
        }
        WebSQLStorage.prototype.transaction = function (db) {
            return $.Deferred(function (dfd) {
                db.transaction(dfd.resolve, dfd.reject);
            }).promise();
        };
        WebSQLStorage.prototype.executeSql = function (db, req, values) {
            return this.transaction(db).then(function (tx) {
                return $.Deferred(function (dfd) {
                    tx.executeSql(req, values || [], function (tx, result) {
                        dfd.resolve(result, tx);
                    }, dfd.reject);
                });
            });
        };
        WebSQLStorage.prototype.ensureDb = function () {
            if (!this.db) {
                var db = this.db = (window).openDatabase(this.dbname, "1.0", "SPA Store Database", this.dbsize);
                return this.executeSql(db, "CREATE TABLE IF NOT EXISTS " + this.tablename + " (id unique, data)").then(function () {
                    return db;
                });
            }

            return $.when(this.db);
        };

        WebSQLStorage.prototype.init = function () {
            var _this = this;
            return this.ensureDb().then(function (db) {
                return _this.executeSql(db, "SELECT * FROM " + _this.tablename).then(function (result) {
                    var len = result.rows.length, i, item;
                    for (i = 0; i < len; i++) {
                        item = result.rows.item(i);
                        _this.memory[item.id] = item.data;
                    }

                    _this.length = len;
                });
            });
        };
        WebSQLStorage.prototype.clear = function () {
            var _this = this;
            this.memory = {};
            this.length = 0;
            return this.ensureDb().then(function (db) {
                return _this.executeSql(db, "DELETE FROM " + _this.tablename + " WHERE 1=1");
            });
        };

        WebSQLStorage.prototype.key = function (index) {
            return _.find(_.values(this.memory), function (val, i) {
                return i === index;
            }) || null;
        };
        WebSQLStorage.prototype.getItem = function (key) {
            return this.memory[key] || null;
        };
        WebSQLStorage.prototype.setItem = function (key, value) {
            var _this = this;
            var toUpdate = !!this.memory[key];
            this.memory[key] = value;

            return this.ensureDb().then(function (db) {
                if (toUpdate) {
                    return _this.executeSql(db, "UPDATE " + _this.tablename + " SET data=? WHERE id=?", [value, key]);
                } else {
                    return _this.executeSql(db, "INSERT INTO " + _this.tablename + " (id, data) VALUES (?, ?)", [key, value]).done(function () {
                        _this.length++;
                    });
                }
            });
        };
        WebSQLStorage.prototype.removeItem = function (key) {
            var _this = this;
            if (this.memory[key]) {
                delete this.memory[key];
                this.length--;

                return this.ensureDb().then(function (db) {
                    return _this.executeSql(db, "DELETE FROM " + _this.tablename + " WHERE id=?", [key]);
                });
            }
        };
        return WebSQLStorage;
    })();
    createFromIStorage("websql", new WebSQLStorage());

    _.each(["localStorage", "sessionStorage"], function (storageType) {
        try  {
            if (window[storageType] && window[storageType].getItem) {
                createFromIStorage(storageType, window[storageType]);
            }
        } catch (e) {
            return false;
        }
    });

    (function () {
        if (window.globalStorage) {
            try  {
                createFromIStorage("globalStorage", window.globalStorage[window.location.hostname]);
            } catch (e) {
                return false;
            }
        }
    })();

    var _store = stores.localStorage;

    if (!_store) {
        _store = stores.sessionStorage;
        if (stores.globalStorage)
            _store = stores.globalStorage;
    }

    if (!_store)
        _store = stores.memory;

    exports.length = 0;

    function key(index) {
        var result = _store.key(index);
        exports.length = _store.length;
        return result;
    }
    exports.key = key;
    function getItem(key) {
        var result = _store.getItem(key);
        exports.length = _store.length;
        return result;
    }
    exports.getItem = getItem;
    function setItem(key, data) {
        _store.setItem(key, data);
        exports.length = _store.length;
    }
    exports.setItem = setItem;
    function removeItem(key) {
        _store.removeItem(key);
        exports.length = _store.length;
    }
    exports.removeItem = removeItem;
    function clear() {
        _store.clear();
        exports.length = _store.length;
    }
    exports.clear = clear;

    function changeStore(type) {
        if (stores[type]) {
            _store = stores[type];
            exports.length = _store.length;

            return _store.init && _store.init();
        }

        return utils.wrapError("NOT FOUND");
    }
    exports.changeStore = changeStore;
    function addStorageType(type, store, change) {
        if (stores[type]) {
            throw new Error("This store already exists !");
        }

        if (_.isNumber(store.length) && store.clear && store.getItem && store.setItem && store.key && store.removeItem) {
            createFromIStorage(type, store);
        }

        if (change === true) {
            return exports.changeStore(type);
        }

        return $.when();
    }
    exports.addStorageType = addStorageType;
});
