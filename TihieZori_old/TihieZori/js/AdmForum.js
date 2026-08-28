class Control {
    constructor(el) { this.el = $(el); }
    GetValue() { return this.el.val(); }
    SetValue(val) { this.el.val(val); }
}

class TextControl {
    constructor(el) { this.el = $(el); }
    GetValue() { return this.el.text(); }
    SetValue(val) { this.el.text(val); }
}

class HtmlCnt {
    constructor(el) { this.el = el; }
    GetValue() { return this.el.innerHTML; }
    SetValue(val) { this.el.innerHTML = val; }
}

class CheckControl {
    constructor(el) { this.el = el; }
    GetValue() { return this.el.checked ? 1 : 0; }
    SetValue(val) { this.el.checked = val > 0; }
}

class DateControl {
    constructor(el) { this.el = $(el); }
    SetValue(val) { this.el.text(new Date(val * 1000).toLocaleDateString()); }
}

class DpControl {
    constructor(el) {
        this.el = $(el).datepicker({ dateFormat: "dd.mm.yy" });
    }
    GetValue() { return this.el.datepicker("getDate").valueOf() / 1000; }
    SetValue(val) { this.el.datepicker("setDate", new Date(val * 1000)); }
}

class HtmlControl {
    constructor(el) {
        this.textA = $(el);
        this.nici = new nicEditor({ fullPanel: true, maxHeight: 200, maxWidth: 600 }).panelInstance(el).nicInstances[0];
    }
    GetValue() { return this.nici.getContent(); }
    SetValue(val) { this.nici.setContent(val); }
}

class CTable {
    constructor(init) {
        //this.head = init.head;
        this.saveurl = init.saveurl;
        this.head = {};
        this.InitActions();
    }

    Save(data, after) {
        //$.getJSON(this.saveurl, this.ToObj(data), after);
        $.ajax({
            method: "POST",
            url: this.saveurl,
            data: this.ToObj(data)
        }).always(after);
    }

    ShowRowData(row, data) {
        row.data(data);
        row.find("[data-name]").each((i, el) => {
            let scnt = el.dataset.control || "TextControl";
            let cnt = eval("new " + scnt + "(el)");
            cnt.SetValue(data[this.head[el.dataset.name]]);
        });
    }

    Load(after) {
        var param = this.where ? { "where": JSON.stringify(this.where) } : null;
        $.getJSON('dataf/' + this.Name + '.dbo', param, dataArray => {
            var hl = this.headline = dataArray.shift(); // первая строка
            this.head = hl.reduce(function (acc, val, i) { acc[val] = i; return acc; }, {});
            this.dataArray = dataArray;
            //if (hl[0] == "Id") {this.data = dataArray.reduce(function (acc, val, i) {dataArray[i] = acc[val[0]] = new this.toObj(this.head, val);return acc;}, {});}
        }).always(after);
    }

    Init() {
        $.getJSON('head/Feedbacks.dbo', headline => {
            $.getJSON('data/Feedbacks.dbo', data => {
                var s1 = $(".s1");
                s1.hide();
                var head = this.head;
                for (var i = 0; i < headline.length; i++)
                    head[headline[i]] = i;
                this.table = s1.parents("table")[0];
                this.body = $(this.table.tBodies[0]);
                //tbl.data(head);
                let oi = this.head.OrderId;
                data.sort((r1, r2) => r1[oi] - r2[oi]);
                for (var i = 0; i < data.length; i++)
                    this.AddRow(data[i]);
                this.InitForm();
                $("#add").click(event => {
                    $.getJSON('add/Feedbacks.dbo', { "DatM": new Date(), "Title": "Объявление", "Comment": "Текст объявления" }
                        , data => { this.AddRow(data); });
                });
            });
        });
    }

    //инициализация формы ввода
    InitForm() {
        this.formcontrols = {};
        var f = $(".e1");
        f.find("#cansel").click(() => f.fadeOut(1000));
        f.find("#submit").click(() => {
            var row = f.prev();
            var data = row.data();
            this.ExtractData(f, data);
            this.ShowRowData(row, data);
            f.fadeOut(1000);
            this.Save(data, dataObj => this.ShowRowData(row, data));
        });
        f.find("[data-name]").each((i, el) => {
            let scnt = el.dataset.control || "Control";
            this.formcontrols[el.dataset.name] = eval("new " + scnt + "(el)");
        });
        this.form = f;
    }

    ShowFormData(data) {
        data = this.ToObj(data);
        let fc = this.formcontrols;
        for (var cn in fc)
            fc[cn].SetValue(data[cn]);
    }

    ExtractData(form, data) {
        let fc = this.formcontrols;
        for (var cn in fc)
            data[this.head[cn]] = fc[cn].GetValue();
    }

    AddRow(data) {
        var s1 = $(".s1");
        var row = s1.clone(true);
        row.removeClass("s1");
        this.body.append(row);
        this.ShowRowData(row, data);
        this.AttachRowActions(row, data);
        row.show();
    }

    InitActions() {
        this.Actions = {
            edit: row => {
                var data = row.data();
                var f = this.form;
                f.fadeOut(1000, () => {
                    f.insertAfter(row);
                    this.ShowFormData(data);
                    f.fadeIn(1000);
                });
            },
            del: row => {
                var data = row.data();
                if (this.BeforeDelete(data)) {
                    $.getJSON('del/Feedbacks.dbo', { "Id": data[this.head["Id"]] }, () => {
                        row.remove();
                    });
                };
            },
            active: row => {
                let data = row.data();
                let ai = this.head.Active;
                var ch = row.find("[data-Name=Active]")[0];
                let ret = [data[0]];
                ret[ai] = ch.checked ? 1 : 0;
                this.Save(ret);
            }
        }
    }

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
        })
    }

    BeforeDelete(data) {
        if (data.Locked) {
            alert("Эту запись нельзя удалять !");
            return false;
        }
        $.di
        return true;
    }

    ToObj(row) {
        let ret = {};
        let h = this.head;
        for (var name in h)
            ret[name] = row[h[name]];
        return ret;
    }
}

var docTable = new CTable({ saveurl: 'edit/Feedbacks.dbo' });

$(() => {
    docTable.Init();
})



