
class Event {
    constructor(sender) {
        this._sender = sender;
        this._listeners = [];
    }

    attach(listener) {
        this._listeners.push(listener);
    }
    notify(args) {
        for (var i = 0; i < this._listeners.length; i++) {
            this._listeners[i](this._sender, args);
        }
    }
}

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

class FloatControl {
    constructor(el) { this.el = $(el); }
    GetValue() { return parseFloat(this.el.value) || 0; }
    SetValue(val) { this.el.text(val.toFixed(2)); }
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
    SetValue(val) {
        if (!(val instanceof Date))
            val = new Date(val * 1000);
        this.el.text(val.toLocaleDateString());
    }
}

class DpControl {
    constructor(el) {
        this.el = $(el).datepicker({ dateFormat: "dd.mm.yy" });
    }
    GetValue() { return this.el.datepicker("getDate").valueOf() / 1000; }
    SetValue(val) {
        if (!(val instanceof Date))
            val = new Date(val * 1000);
        this.el.datepicker("setDate", val);
    }
}

class HtmlControl {
    constructor(el) {
        this.textA = $(el);
        this.nici = new nicEditor({ fullPanel: true, maxHeight: 200, maxWidth: 600 }).panelInstance(el).nicInstances[0];
    }
    GetValue() { return this.nici.getContent(); }
    SetValue(val) { this.nici.setContent(val); }
}

class SelectControl {
    constructor(el) { this.el = el; }
    GetValue() {
        let val = $(this.el).val();
        if (!val) val = -1;
        return val - 0;
    }
    SetValue(val) { $(this.el).val(val); }
    Init(db) {
        let el = this.el;
        let source = db[el.dataset.source];
        let valName = el.dataset.value;
        let txtName = el.dataset.text;
        source.dataArray.forEach((obj) => {
            let opEl = document.createElement("option");
            opEl.text = obj[txtName];
            opEl.value = obj[valName];
            el.add(opEl);
        });
    }
}

class SelectControl2 {
    constructor(el) { this.el = $(el); }
    GetValue() {
        let val = this.el.val();
        if (!val) val = -1;
        return val - 0;
    }
    SetValue(val) { this.el.val(val); }
    Init(db) {
        let el = this.el[0];
        let source = db[el.dataset.source];
        let valName = el.dataset.value;
        let txtName = el.dataset.text;
        let opEmpty = document.createElement("option");
        opEmpty.text = "---";
        opEmpty.value = 0;
        el.add(opEmpty);
        source.dataArray.forEach((obj) => {
            let opEl = document.createElement("option");
            opEl.text = obj[txtName];
            opEl.value = obj[valName];
            el.add(opEl);
        });
    }
}

class InputListControl {
    constructor(el) {
        this.el = $(el);
        this.emptyText = "Взносы за ...";
    }
    GetValue() {
        let el = this.el[0];
        let val = el.value;
        if (val === this.emptyText)
            return -1;
        let obj = this.source.dataArray.find(obj => { return obj[this.txtName] === val; });
        if (obj)
            return obj[this.valName];
        return val;
    }
    SetValue(val) {
        let obj = this.source.data[val];
        this.el.val(obj ? obj[this.txtName] : this.emptyText);
    }
    Init(db) {
        let el = this.el[0];
        this.source = db[el.dataset.source];
        this.valName = el.dataset.value;
        this.txtName = el.dataset.text;
        let dl = el.list;
        this.source.dataArray.forEach((obj) => {
            let opEl = document.createElement("option");
            opEl.value = obj[this.txtName];
            dl.append(opEl);
        });
        this.source.onAddNew.attach((sender, obj) => {
            let opEl = document.createElement("option");
            opEl.value = obj[this.txtName];
            dl.append(opEl);
        });
    }
}
