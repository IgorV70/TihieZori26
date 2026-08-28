class CTable {
    constructor(init) {
        //this.head = init.head;
        this.saveurl = init.saveurl;
        this.head = {};
        this.InitActions();
    }

    Save(data, after) {
        $.getJSON(this.saveurl, ArrToObj(this.head, data), after);
    }

    ExtractData(form, data) {
        for (var name in this.head) {
            var tdEl = form.find("[data-Name=" + name + "]");
            if (tdEl.length) {
                let td = tdEl[0];
                data[this.head[name]] = td.dataset.getter ? eval(td.dataset.getter)(td) : $(td).val();
            }
        }

    }

    ShowFormData(form, data) {
        for (var name in this.head) {
            var tdEl = form.find("[data-Name=" + name + "]");
            if (tdEl.length) {
                let td = tdEl[0];
                let dataEl = data[this.head[name]];
                if (td.dataset.setter)
                    eval(td.dataset.setter)(td, dataEl);
                else
                    $(td).val(dataEl);
            }
        }
    }

    ShowRowData(row, data) {
        for (var name in this.head) {
            var tdEl = row.find("[data-Name=" + name + "]");
            if (tdEl.length) {
                let td = tdEl[0];
                let dataEl = data[this.head[name]];
                if (td.dataset.setter)
                    eval(td.dataset.setter)(td, dataEl);
                else
                    tdEl.text(dataEl);
            }
        }
        row.data(data);
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
        $.getJSON('head/FinDocuments.dbo', headline => {
            $.getJSON('data/FinDocuments.dbo', data => {
                var s1 = $(".s1");
                s1.hide();
                var head = this.head;
                for (var i = 0; i < headline.length; i++)
                    head[headline[i]] = i;
                this.table = s1.parents("table")[0];
                this.body = $(this.table.tBodies[0]);
                //tbl.data(head);
                let oi = this.head.OrderId;
                data.sort((r1,r2)=>r1[oi]-r2[oi]);
                for (var i = 0; i < data.length; i++)
                    this.AddRow(data[i]);
                var e1 = $(".e1").hide();
                e1.find("#cansel").click(() => e1.fadeOut(1000));
                e1.find("#submit").click(() => {
                    var row = e1.prev();
                    var data = row.data();
                    this.ExtractData(e1, data);
                    this.ShowRowData(row, data);
                    e1.fadeOut(1000);
                    this.Save(data, dataObj => this.ShowRowData(row, data));
                });
                $("#add").click(event => {
                    event.preventDefault();
                    var input = document.createElement('input');
                    input.type = 'file';
                    input.addEventListener("change", () => {
                        let file = input.files[0];
                        var formData = new FormData();
                        formData.append("fileInput", file);
                        //$.getJSON('docload',formData, data => AddRow( data));
                        $.ajax({
                            url: 'docload',
                            type: "POST",
                            data: formData,
                            contentType: false,
                            processData: false,
                            success: data => this.AddRow(data)
                        });
                    }, false);
                    input.click();
                });
            });
        });
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
                var e1 = $(".e1");
                e1.fadeOut(1000, () => {
                    this.ShowFormData(e1, data);
                    e1.insertAfter(row);
                    e1.fadeIn(1000);
                });
            },
            del: row => {
                var data = row.data();
                if (this.BeforeDelete(data)) {
                    $.getJSON('del/FinDocuments.dbo', { "Id": data[this.head["Id"]] }, () => {
                        row.remove();
                    });
                };
            },
            up: row => {
                let urow = row.prev();
                if (urow.length) {
                    urow.insertAfter(row);
                    let data1 = row.data();
                    let data2 = urow.data();
                    let oi = this.head.OrderId;
                    [data1[oi], data2[oi]] = [data2[oi], data1[oi]];
                    this.Save(data1);
                    this.Save(data2);
                }
            },
            down: row => {
                let urow = row.next();
                if (urow.length) {
                    row.insertAfter(urow);
                    let data1 = row.data();
                    let data2 = urow.data();
                    let oi = this.head.OrderId;
                    [data1[oi], data2[oi]] = [data2[oi], data1[oi]];
                    this.Save(data1);
                    this.Save(data2);
                }
            },
            active: row => {
                let data = row.data();
                let ai = this.head.Active;
                var ch = row.find("[data-Name=Active]")[0];
                data[ai] = ch.checked ? 1 : 0;
                this.Save(data);
            }
        }
    }

    AttachRowActions(row, data) {
        var $this = this;
        row.find("[data-action]").each(function () {
            var ac= this.dataset.action.split(':');
            if (ac.length == 1)
            {
                ac[1] = ac[0];
                ac[0] = "click";
            }
            var action = $this.Actions[ac[1]];
            action && $(this)[ac[0]](function () { action(row); });
        })
    }

    BeforeDelete(data) {
        var obj = ArrToObj(this.head, data);
        if (obj.Locked) {
            alert("Эту запись нельзя удалять !");
            return false;
        }
        return true;
    }

}

var docTable = new CTable({ saveurl: 'edit/FinDocuments.dbo' });

$(() => {
    docTable.Init();
})

function checkboxSetter(cb, val) {
    cb.checked = val > 0;
}

function checkboxGetter(cb) {
    return cb.checked ? 1 : 0;
}


function ArrToObj(head, obj) {
    var ret = {};
    for (var name in head)
        ret[name] = obj[head[name]];
    return ret;
}

