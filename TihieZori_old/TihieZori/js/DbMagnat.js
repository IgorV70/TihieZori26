
function Table(name) {
    this.Name = name;

    this.load = function (after) {
        var $this = this;
        var param = this.where ? { "where": JSON.stringify(this.where) } : null;
        $.getJSON('dataf/' + this.Name + '.dbo', param, function (dataArray) {
            var hl = $this.headline = dataArray.shift(); // первая строка
            $this.head = hl.reduce(function (acc, val, i) { acc[val] = i; return acc;}, {});
            $this.dataArray = dataArray;
            if (hl[0] == "Id") {
                $this.data = dataArray.reduce(function (acc, val, i) {
                    dataArray[i] = acc[val[0]] = new $this.toObj($this.head, val);                    
                    return acc;
                }, {});
            }
        }).always(after);
    };

    this.Union = function (other) {
        for (var id in other.data) {
            if (!this.data[id]) {
                var obj = other.data[id];
                this.data[id] = obj;
                this.dataArray.push(obj); s
            }
        }
    };

    this.toObj = function (head, objArray) {
        for (var name in head)
            this[name] = objArray[head[name]];
    }

    this.rowProto = this.toObj.prototype;

    this.rowProto.GetPathValue = function (datapath) {
        datapath = datapath.split('.');
        var ret = this;
        for (var i = 0; i < datapath.length; i++) {
            ret = ret[datapath[i]];
            if (ret == null)
                //return "свойство " + datapath[i] + "неопределено";
                return "";
        }
        return ret;
    }

    this.AddRow = function (data) {
        var s1 = $("." + this.Name);
        var row = s1.clone(true);
        row.removeClass(this.Name);
        s1.parent().append(row);
        this.ShowRowData(row, data);
        this.AttachRowActions(row, data);
        row.show();
    }

    this.ShowRowData = function (row, data) {
        row.find("[datapath]").each(function () {
            var datapath = this.getAttribute("datapath");
            $(this).text(data.GetPathValue(datapath));
        });
        row.data("id", data.Id);
    }

    this.Actions = {};

    this.AttachRowActions = function (row, data) {
        var $this = this;
        row.find("[action]").each(function () {
            var actionAtr = this.getAttribute("action");
            var action = $this.Actions[actionAtr];
            action && $(this).click(function () { action(row); });
        });
    }

    this.createRows = function () {
        for (var i = 0, length = this.dataArray.length; i < length; i++) {
            this.AddRow(this.dataArray[i]);
        }
    };

    this.Delete = function (dataObj) {
        this.data[dataObj.Id] = undefined;
        this.dataArray.splice(this.dataArray.indexOf(dataObj), 1);
    }
}


function DbTihieZori() {

    this.tables = {};
    this.AddTable = function (tableName) {
        this[tableName] = new Table(tableName);
        this.tables[tableName] = this[tableName];
    };
    this.removeTable = function (tableName) {
        delete this[tableName];
        delete this.tables[tableName];
    };

    this.AddTable("Order");
    this.AddTable("ServiceCenter");
    this.AddTable("Client");
    this.AddTable("Shedule");
    this.AddTable("Usluga");

    var $thisDb = this;

    this.load = function (callback) {
        var counter = Object.keys(this.tables).length;
        var loadReady = function () {
            if (--counter == 0 && callback)
                setTimeout(callback, 0);
        };
        for (tname in this.tables)
            this.tables[tname].load(loadReady);
    }

    var now = new Date();
    var today = new Date(now.getFullYear(), now.getMonth(), now.getDate());
    this.Order.where = { name: "admorder", params: [today.valueOf() / 1000, -today.getTimezoneOffset()] };
    this.Shedule.where = { name: "admorder", params: [today.valueOf() / 1000, -today.getTimezoneOffset()] };

    this.Order.rowProto.__defineGetter__("ServiceCenter", function () {
        if (this._serviceCenter) return this._serviceCenter;
        this._serviceCenter = $thisDb.ServiceCenter.data[this.ServiceCenterId];
        return this._serviceCenter;
    });

    this.Order.rowProto.__defineGetter__("Client", function () {
        if (this._client) return this._client;
        this._client = $thisDb.Client.data[this.ClientId];
        return this._client;
    });

    this.Order.rowProto.__defineGetter__("StrOrderDat", function () {
        var offset = (new Date()).getTimezoneOffset() * 60 * 1000;
        var date = (new Date(this.OrderDat * 1000 + offset));
        var ret = date.toLocaleDateString() + " " + date.toTimeString().substr(0, 5);
        return ret;
    });

    this.Order.rowProto.__defineGetter__("StrOrderStady", function () {
        return { "Created": "Создан", "Verified": "Проверен", "Canceled": "Отменен" }[this.OrderStady];
    });

    this.Order.rowProto.__defineGetter__("Shedule", function () {
        if (this._shedule) return this._shedule;
        if (this.OrderStady == "Canceled")
            return null;
        for (var i = 0, length = $thisDb.Shedule.dataArray.length; i < length; i++) {
            var shedule = $thisDb.Shedule.dataArray[i];
            if (shedule.OrderId == this.Id) {
                this._shedule = shedule;
                return this._shedule;
            }
        }
        return null;
    });

    this.Order.rowProto.__defineGetter__("Uslugi", function () {
        var usl = $thisDb.Usluga.data;
        return this.UslugiIds.map(function (id) { return usl[id].Name; }).join(", ");
    });

    this.Shedule.rowProto.__defineGetter__("StrShTime", function () {
        var offset = (new Date()).getTimezoneOffset() * 60 * 1000;
        var date = (new Date(this.ShData * 1000 + offset + this.StartTime * 60 * 1000));
        var ret = date.toLocaleDateString() + " " + date.toTimeString().substr(0, 5);
        return ret;
    });

    $("#dialog-confirm").dialog({
        resizable: false,
        height: "auto",
        width: 400,
        modal: true,
        autoOpen: false,
        show: { effect: "blind", duration: 200 }
    });

    this.Order.Actions.checked = function (row) {
        var order = $thisDb.Order.data[row.data("id")];
        if (order.OrderStady == "Canceled") return;
        if (order.OrderStady == "Verified") return;
        $.getJSON('edit/Order.dbo', { "Id": order.Id, "OrderStady": "Verified" }, function (orderMod) {
            order.OrderStady = orderMod.OrderStady;
            $thisDb.Order.ShowRowData(row, order);
        });
    };

    this.Order.Actions.cancel = function (row) {
        var order = $thisDb.Order.data[row.data("id")];
        if (order.OrderStady == "Canceled") return;
        $("#dialog-confirm").dialog({
            buttons: {
                "Отменить заказ": function () {
                    $.getJSON('edit/Order.dbo', { "Id": order.Id, "OrderStady": "Canceled" }, function (orderMod) {
                        $.getJSON('del/Shedule.dbo', { "Id": order._shedule.Id }, function () {
                            order.OrderStady = orderMod.OrderStady;
                            $thisDb.Shedule.Delete(order._shedule);
                            order._shedule = null;
                            $thisDb.Order.ShowRowData(row, order);
                        });
                    });
                    $(this).dialog("close");
                },
                "Не отменять": function () {
                    $(this).dialog("close");
                }
            }
        });
        $("#dialog-confirm").dialog("open");
    };
}

var db;

function LoadNewOrders() {
    if (db.Order.dataArray.length == 0)
        db.load(function () {
            db.Order.createRows();
            setTimeout(LoadNewOrders, 20000);
            return;
        });

    var db2 = new DbTihieZori();
    var maxId = db.Order.dataArray.reduce(function (p, v) { return (p.Id > v.Id ? p : v); }).Id;
    var now = new Date();
    var today = new Date(now.getFullYear(), now.getMonth(), now.getDate());
    db2.removeTable("ServiceCenter");
    db2.Order.where = { name: "admorder", params: [maxId] };
    db2.Shedule.where = { name: "admorder", params: [maxId] };
    db2.Client.where = { name: "admorder", params: [maxId] };

    db2.load(function () {
        if (db2.Order.dataArray.length > 0) {
            db2.ServiceCenter = db.ServiceCenter;
            db2.Order.createRows();
            db.Order.Union(db2.Order);
            db.Shedule.Union(db2.Shedule);
            db.Client.Union(db2.Client);
            timerTitle.start();
        }
        setTimeout(LoadNewOrders, 20000);
    });
}

$(function () {
    db = new DbTihieZori();

    db.load(function () {
        db.Order.createRows();
        setTimeout(LoadNewOrders, 20000);
        timerTitle.start();
    });

})



