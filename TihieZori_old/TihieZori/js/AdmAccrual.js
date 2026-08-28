// общий класс для всех строк таблиц (прототип прототипов строк)
class dataRow {
    constructor(dataObj) {
        $.extend(this, dataObj);
        this._IsPersistent = false;
    }
    test() {
        console.log("test");
        return "test";
    }

    _getPathValue(path) {
        path = path.split('.');
        let obj = this;
        for (var i = 0; i < path.length; i++) {
            obj = obj[path[i]];
            if (obj == null)
                //return "свойство " + datapath[i] + "неопределено";
                return "";
        }
        return obj;
    }

    _save(after) {
        this.table.Save(this, after);
    }

    // возвращает объект, содержащий только значимые свойства 
    _getClean() {
        let ret = {};
        for (var pname in this.table.head) {
            if (this[pname])
                ret[pname] = this[pname];
        }
        return ret;
    }

    _update(obj) {
        for (var prop in obj)
            this[prop] = obj[prop];
    }
}


class Table {
    constructor(name) {
        this.Name = name;
        this.loadurl = "dataf/" + name + ".dbo";
        this.saveurl = "edit/" + name + ".dbo";
        this.addurl = "add/" + name + ".dbo";
        this.removeurl = "del/" + name + ".dbo";
        this.head = {};
        let $table = this;
        this.rowProto = function (obj) {
            this.table = $table;
            for (var prop in obj)
                this[prop] = obj[prop];
        };
        this.rowProto.prototype = new dataRow();
        this.onAddNew = new Event(this);
    }

    //class Table 
    Save(obj, after) {
        $.getJSON(obj._IsPersistent ? this.saveurl : this.addurl
            , obj._getClean(), (dataRow) => {
                obj._update(this.ToObj(dataRow));
                obj._IsPersistent = true;
                after & after(obj);
            });
    }

    //class Table 
    Load() {
        var ret = $.Deferred();
        var param = this.where ? { "where": JSON.stringify(this.where) } : null;
        if (this.fields) param.fields = this.fields;
        $.getJSON(this.loadurl, param, dataArray => {
            var hl = this.headline = dataArray.shift(); // первая строка
            this.head = hl.reduce(function (acc, val, i) { acc[val] = i; return acc; }, {});
            this.dataArray = dataArray;
            if (hl[0] == "Id") {
                this.data = dataArray.reduce((acc, val, i) => {
                    let obj = new this.rowProto(this.ToObj(val));
                    obj._IsPersistent = true;
                    dataArray[i] = obj;
                    acc[obj.Id] = obj;
                    return acc;
                }, {});
            }
            ret.resolve(this);
        });
        return ret;
    }

    AddNew(rowObj) {
        this.dataArray.push(rowObj);
        this.data[rowObj.Id] = rowObj;
        this.onAddNew.notify(rowObj);
    }

    ToObj(row) {
        let ret = {};
        let h = this.head;
        for (var name in h)
            ret[name] = row[h[name]];
        return ret;
    }

}

class TableView {
    constructor(db, tableElementId, addButtonElementId) {
        this.Database = db;
        this.el = $("#" + tableElementId)[0];
        this.table = db[this.el.dataset.name];

        this.InitActions();

        this.rowLayout = $(this.el).find(".s1");
        this.rowLayout.hide();

        this.body = $(this.el.tBodies[0]);
        //tbl.data(head);
        //let oi = this.head.OrderId;
        //data.sort((r1, r2) => r1[oi] - r2[oi]);
        this.rows = [];
        this.CreateRows();
        this.InitForm();
        this.InitPayment();
        $("#" + addButtonElementId).click(() => {
            var row = this.AddRow(new db.Accrual.rowProto({
                "AccDate": new Date(), "_propose": { "Id": -1, Name: "Взносы за ..." },
                "ProposeId": 0, "UserId": 0, "_user": { LandNumber: 0 }, "AccSum": 0.0
            }));
            this.Actions.edit(row);
        });
    }

    //class TableView
    CreateRows() {
        this.rows.forEach(row => row.remove());
        this.rows = [];
        this.table.dataArray.forEach(rowData => this.rows.push(this.AddRow(rowData)));
    }

    Filter(predicate) {
        this.rows.forEach(row => predicate(row[0].rowData) ? row.show() : row.hide());
    }

    //class TableView
    AddRow(rowData) {
        var row = this.rowLayout.clone(true);
        row.removeClass("s1");
        this.body.append(row);
        this.ShowRowData(row, rowData);
        this.AttachRowActions(row, rowData);
        row.show();
        return row;
    }


    //class TableView
    // заполняем значениями строку таблицы
    ShowRowData(row, rowData) {
        row[0].rowData = rowData;
        row.find("[data-name]").each((i, el) => {
            let scnt = el.dataset.control || "TextControl";
            let cnt = eval("new " + scnt + "(el)");
            cnt.SetValue(rowData._getPathValue(el.dataset.name));
        });
    }

    //class TableView
    // заполняем значениями форму
    ShowFormData(data) {
        let fc = this.formcontrols;
        for (var cn in fc)
            fc[cn].SetValue(data._getPathValue(cn));
    }

    //class TableView
    AttachRowActions(row, data) {
        var $this = this;
        row.find("[data-action]").each(function () {
            var ac = this.dataset.action.split(':');
            if (ac.length == 1) {
                ac[1] = ac[0];
                ac[0] = "click";
            }
            var action = $this.Actions[ac[1]];
            action && $(this)[ac[0]](function () { action(row); });
        });
    }

    //class TableView
    InitActions() {
        this.Actions = {
            edit: row => {
                var data = row[0].rowData;
                var f = this.form;
                f.fadeOut(1000, () => {
                    f.insertAfter(row);
                    this.ShowFormData(data);
                    f.fadeIn(1000);
                });
            },
            del: row => {
                var data = row[0].rowData;
                if (this.BeforeDelete(data)) {
                    document.DelDialog.Action = () => {
                        $.getJSON(this.table.removeurl, { "Id": data.Id }, () => {
                            row.remove();
                        });
                        document.DelDialog.dialog("close");
                    };
                    document.DelDialog.dialog("option", "position", { my: "left top", at: "left bottom", of: row });
                    document.DelDialog.dialog("open");
                }
            },
            active: row => {
                let data = row[0].rowData;
                let ai = this.head.Active;
                var ch = row.find("[data-Name=Active]")[0];
                let ret = [data[0]];
                ret[ai] = ch.checked ? 1 : 0;
                this.Save(ret);
            },
            money: row => {
                var data = row[0].rowData;
                var f = this.formPm;
                this.tbvPayment.CurrentUserId = data.UserId;
                this.tbvPayment.CurrentProposeId = data.ProposeId;

                this.Database.Payment.where.params[0] = data.ProposeId;
                this.Database.Payment.where.params[1] = data.UserId;
                f.fadeOut(500, () => {
                    f.insertAfter(row);
                    $.when(this.Database.Payment.Load()).then(() => {
                        this.tbvPayment.CreateRows();
                        f.fadeIn(1000);
                    });
                });
            }
        };
    }

    //class TableView
    //инициализация формы ввода
    InitForm() {
        this.formcontrols = {};
        var f = $(".e1");
        f.find("#cansel").click(() => this.CancelForm());
        f.find("#submit").click(() => this.SubmitForm());

        f[0].querySelectorAll("[data-name]").forEach((el) => {
            let scnt = el.dataset.control || "Control";
            let cnt = eval("new " + scnt + "(el)");
            this.formcontrols[el.dataset.name] = cnt;
            if (el.dataset.source) {
                cnt.Init(this.Database);
            }
        });
        this.form = f;
    }


    //class TableView
    //инициализация формы ввода
    InitPayment() {
        var f = $(".f1");
        this.formPm = f;
    }

    SubmitForm() {
        let f = this.form;
        var row = f.prev();
        let data = row[0].rowData;
        this.ExtractData(f, data);
        const vf = () => {
            if (this.Verify(data)) {
                data._propose = undefined;
                data._user = undefined;
                if (data.UserId == 99999) {
                    this.CancelForm();
                    this.CreateAccruals(data);
                    return;
                }
                this.ShowRowData(row, data);
                data._save(
                    dataObj => {
                        this.ShowRowData(row, data);
                        f.fadeOut(1000);
                    }
                );
            }
        };
        if (typeof data.ProposeId === "string") {
            let objProp = new this.Database.Propose.rowProto({ "Name": data.ProposeId });
            objProp._save(() => {
                data.ProposeId = objProp.Id;
                data._propose = undefined;
                objProp.table.AddNew(objProp);
                vf();
            });
            ret;
        }
        vf();
    }

    CreateAccruals(data) {
        let protoaccr = data._getClean();
        let arg = this.Database.User.dataArray.map(
            (user) => {
                if (user.Id == 99999) return;
                protoaccr.UserId = user.Id;
                let accr = new this.Database.Accrual.rowProto(protoaccr);
                var ret = $.Deferred();
                accr._save((obj) => {
                    this.table.AddNew(obj);
                    ret.resolve(this);
                });
                return ret;
            }
        );
        $.when.apply(null, arg).then(() => this.CreateRows());
    }

    Verify(data) {
        let f = this.form;
        f.find(".error").hide();
        let ret = true;
        if (data.UserId <= 0) {
            f.find(".errorUser").show();
            ret = false;
        }
        if (data.ProposeId <= 0) {
            f.find(".errorPropose").show();
            ret = false;
        }
        if (data.AccSum <= 0) {
            f.find(".errorAccSum").show();
            ret = false;
        }
        return ret;
    }


    CancelForm() {
        let f = this.form;
        var row = f.prev();
        let data = row[0].rowData;
        if (!data._IsPersistent)
            row.remove();
        f.fadeOut(1000);
    }

    ExtractData(form, data) {
        let fc = this.formcontrols;
        for (var cn in fc)
            data[cn] = fc[cn].GetValue();
    }

    BeforeDelete(data) {
        if (data.Locked) {
            alert("Эту запись нельзя удалять !");
            return false;
        }
        return true;
    }

}

class DbZori {
    constructor() {
        this.tables = [];
        this.AddTable = function (tableName) {
            let tbl = new Table(tableName);
            this.tables.push(tbl);
            this[tableName] = tbl;
            tbl.Database = this;
        };

        this.removeTable = function (tableName) {
            delete this[tableName];
            delete this.tables[tableName];
        };

        this.AddTable("Accrual");
        this.AddTable("Payment");
        this.AddTable("User");
        this.AddTable("Propose");

        this.User.where = { name: "active" };
        this.User.fields = "Id,LandNumber,Fio";

        this.Accrual.where = { name: "period", params: [2022] };
        this.Payment.where = { name: "custom", params: [0, 0] };

        Object.defineProperty(this.User.rowProto.prototype, "LandFio", {
            get: function () {
                if (this.LandNumber && this.Fio)
                    return this.LandNumber + "(" + this.Fio + ")";
                return "-----";
            }
        });

        Object.defineProperty(this.User.rowProto.prototype, "Land", {
            get: function () {
                if (this.LandNumber)
                    return "Участок № "+this.LandNumber;
                return "-----";
            }
        });

        let propose = {
            get: function () {
                if (this._propose) return this._propose;
                return this._propose = this.table.Database.Propose.data[this.ProposeId];
            }
        };

        let user = {
            get: function () {
                if (this._user) return this._user;
                return this._user = this.table.Database.User.data[this.UserId];
            }
        };

        let paysum = {
            get: function () {
                if (this._paysum) return this._paysum;
                let self = this;
                this._paysum = this.table.Database.Payment.dataArray.reduce(function (sum, pm) {
                    if (self.UserId == pm.UserId && self.ProposeId == pm.ProposeId)
                        return sum + pm.PaySum;
                    return sum;
                }, 0);
                return this._paysum;
            }
        };

        let itog = {
            get: function () {
                return this.AccSum - this.PaySum;
            }
        };

        Object.defineProperty(this.Accrual.rowProto.prototype, "Propose", propose);
        Object.defineProperty(this.Accrual.rowProto.prototype, "User", user);
        Object.defineProperty(this.Accrual.rowProto.prototype, "PaySum", paysum);
        Object.defineProperty(this.Accrual.rowProto.prototype, "Itog", itog);
        Object.defineProperty(this.Payment.rowProto.prototype, "Propose", propose);
        Object.defineProperty(this.Payment.rowProto.prototype, "User", user);

        var $thisDb = this;

        this.Load = function (callback) {
            let arg = this.tables.map(function (el) { return el.Load(); });
            $.when.apply(null, arg).then(function (e, e1) {
                callback && callback();
            });
        };
    }
}

$(() => {

    let db = new DbZori();
    db.Load(
        () => {
            db.User.dataArray.unshift(new db.User.rowProto({ Id: 99999, LandNumber: 99999, Fio: "Для всех" }));
            let tbvAccrual = new TableView(db, "Accrual", "addAcc");
            db.User.dataArray.shift();

            let tbvPayment = new TableViewPm(db, "Payment", "addPay");
            tbvAccrual.tbvPayment = tbvPayment;
            let ff = new FilterForm(db, "filtrForm", tbvAccrual);
        }
    );

    document.DelDialog = $("#delete-confirm").dialog({
        autoOpen: false,
        height: "auto",
        width: 350,
        modal: true,
        buttons: {
            "Удалить строку": () => {
                document.DelDialog.Action();
            },
            "Отмена": function () {
                document.DelDialog.dialog("close");
            }
        }
    });

    document.DelPayDialog = $("#delete-confirm-pay").dialog({
        autoOpen: false,
        height: "auto",
        width: 350,
        modal: true,
        buttons: {
            "Удалить строку": () => {
                document.DelPayDialog.Action();
            },
            "Отмена": function () {
                document.DelPayDialog.dialog("close");
            }
        }
    });
});



