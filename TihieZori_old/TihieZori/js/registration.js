$(function() {
    $("#regform").submit(function(f2) {

        if (!this.invoice.value) {
            alert('Введите инвойс!');
            return false;
        }

        if (!this.Login.value) {
            alert('Введите ваше имя !');
            return false;
        }
        if (!this.Password.value) {
            alert('Введите пароль!');
            return false;
        }
        if (this.Password.value !== this.password2.value) {
            alert('Пароль и подтверждение должны совпадать!');
            return false;
        }
        if (!this.Email.value) {
            alert('укажите Ваш почтовый адрес!');
            return false;
        }
        return (true);
    });
});


