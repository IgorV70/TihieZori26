//Date.prototype.getTimezoneOffset = function () { return 0; };

$(function () {
    $.getJSON('head/Usluga.dbo', function (headline) {
        $.getJSON('data/Usluga.dbo', function (data) {
            var head= {};
            for (var i = 0; i < headline.length; i++)
                head[headline[i]] = i;
            window['uhead'] = head;
            window['udata'] = data;
            $(".sevicecenter").change(function () {
                sc = $(this).attr('data');
                var uslugList = "";
                for (var i = 0; i < data.length; i++)
                {
                    var obj = ArrToObj(head, data[i]);
                    if (sc[i] == '1')
                    {
                        uslugList += "<div class='col-md-4'><input type='checkbox' name='Usluga' value="+obj.Id+">"+obj.Name+"</div>";
                    }
                }
                $("#uslugi").html(uslugList);
                $("input[name='Usluga']").change(function () { $("#daterow").show(); });
            });       
            $("#showtime3").datepicker({ dateFormat: "dd.mm.yy" });
            var now = new Date();
            var today = new Date(now.getFullYear(), now.getMonth(), now.getDate());
            $("#showtime3").datepicker("setDate",today);

            var fun = function () {
                shift = 0;
                var dateday = $("#showtime3").datepicker("getDate");
                var usl = [];
                $("input:checked[name='Usluga']").val(function (i, val) {
                    usl.push(parseInt(val));
                    return val;
                });
                var obj = { sevicecenter: parseInt($(".sevicecenter:checked").val()), uslugi: usl, dateday: dateday.valueOf() / 1000, offset: -dateday.getTimezoneOffset() };
                window["orderReq"] = obj;
                if (VerifyTR(obj)) {
                    $.getJSON('cust/avatimes.dbo', { param: JSON.stringify(obj) }
                        , function (data) {
                            var timeList = "";
                            for (var i = 0; i < data.starttime.length; i++) {
                                var minut = data.starttime[i] % 60;
                                var formatTime = (data.starttime[i] - minut) / 60 + ":" + ((minut < 10) ? "0" : "") + minut;
                                timeList += "<div class='col-md-3'><input type='radio' name='Avtime' value=" + data.starttime[i] + ">" + formatTime + "</div>";
                            };
                            $("#avtime").html(timeList);
                            $("input[name='Avtime']").change(function () { $("#phonerow").show(); });
                        });
                }
            };
            $("#showtime4").click(fun);
            $("#btnOrder").click(function ()
            {
                var phone = $("#phone").val();
                if (VerifyPhone(phone))
                {
                    var obj = window.orderReq;
                    obj["phone"] = phone;
                    obj["Avtime"] = $("input:checked[name='Avtime']").val();
                    $.getJSON('cust/order.dbo', { param: JSON.stringify(obj) }
                        , function (data) {
                            if (data.orderid >0)
                                window.location.reload();
                        });
                }
            });
        });
    });
})

function VerifyPhone(phone)
{
    var phoneReg = /^((8|\+7)[\-]?)?(\(?\d{3}\)?[\-]?)?[\d\-]{7,10}$/g;
    if (phoneReg.test(phone)) return true;
    alert("Укажите пожалуйста правильный номер телефона.");
    return false;
}

function VerifyTR(obj)
{
    if (!obj.sevicecenter)
    {
        alert("Выберите, пожалуйтса, один из сервисных центров.");
        return false;
    };
    if (obj.uslugi.length == 0)
    {
        alert("Укажите 1 или несколько, требуемых услуг.");
        return false;
    };
    return true;
}

function ArrToObj(head, obj) {
    var ret = {};
    for (var name in head)
        ret[name] = obj[head[name]];
    return ret;
}

