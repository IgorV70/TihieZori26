class TableViewPm {
    constructor(db, tableElementId, addButtonElementId) {
        this.Database = db;
        this.el = $("#" + tableElementId)[0];
        this.table = db[this.el.dataset.name];

        this.InitActions();

        this.rowLayout = $(this.el).find(".fs1");
        this.rowLayout.hide();

        this.body = $(this.el.tBodies[0]);
        //tbl.data(head);
        //let oi = this.head.OrderId;
        //data.sort((r1, r2) => r1[oi] - r2[oi]);
        this.rows = [];
        this.CreateRows();
        this.InitForm();
        $("#" + addButtonElementId).click(() => {
            var row = this.AddRow(new db.Payment.rowProto({
                "PayDate": new Date(), "ProposeId": this.CurrentProposeId, "UserId": this.CurrentUserId, "PaySum": 0.0
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
        row.removeClass("fs1");
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
                    document.DelPayDialog.Action = () => {
                        $.getJSON(this.table.removeurl, { "Id": data.Id }, () => {
                            row.remove();
                        });
                        document.DelPayDialog.dialog("close");
                    };
                    document.DelPayDialog.dialog("option", "position", { my: "left top", at: "left bottom", of: row });
                    document.DelPayDialog.dialog("open");
                }
            },
            active: row => {
                let data = row[0].rowData;
                let ai = this.head.Active;
                var ch = row.find("[data-Name=Active]")[0];
                let ret = [data[0]];
                ret[ai] = ch.checked ? 1 : 0;
                this.Save(ret);
            }
        };
    }

    //class TableView
    //инициализация формы ввода
    InitForm() {
        this.formcontrols = {};
        var f = $(".fe1");
        f.find("#cansel1").click(() => this.CancelForm());
        f.find("#submit1").click(() => this.SubmitForm());

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

    SubmitForm() {
        let f = this.form;
        var row = f.prev();
        let data = row[0].rowData;
        this.ExtractData(f, data);
        const vf = () => {
            if (this.Verify(data)) {
                data._propose = undefined;
                data._user = undefined;
                if (data.UserId == 99999)
                {
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

    CreateAccruals(data)
    {
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




