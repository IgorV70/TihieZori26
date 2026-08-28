class FilterForm {
    constructor(db, formId, tableView) {
        this.Database = db;
        this._tableView = tableView;
        this.el = document.getElementById(formId);
        this.InitForm();
        this.btn = document.getElementById("filter-action");
        this.btn.onclick = () => {
            let fd = this.ExtractData();
            this._tableView.Filter(data => {
                return (fd.UserId == 0 || fd.UserId == data.UserId)
                    && (fd.ProposeId == 0 || fd.ProposeId == data.ProposeId)
                    && (fd.NotPayment == 0 || data.Itog > 0 );
            });
        };
        this.formcontrols.Period.el.onchange = () => {
            this.Database.Accrual.where.params[0] = this.formcontrols.Period.GetValue();
            $.when(this.Database.Accrual.Load()).then(() => {
                this._tableView.CreateRows();
            });
        };
    }


    //class FilterForm
    //инициализация формы
    InitForm() {
        this.formcontrols = {};

        this.el.querySelectorAll("[data-name]").forEach((el) => {
            let scnt = el.dataset.control || "Control";
            let cnt = eval("new " + scnt + "(el)");
            this.formcontrols[el.dataset.name] = cnt;
            if (el.dataset.source) {
                cnt.Init(this.Database);
            }
        });
    }

    ExtractData() {
        let ret = {};
        let fc = this.formcontrols;
        for (var cn in fc)
            ret[cn] = fc[cn].GetValue();
        return ret;
    }

}




