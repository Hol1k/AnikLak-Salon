async function isMasterLoginDataValid() {
    let numberElement = document.getElementById('userPhone');
    if (!/^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$/.test(numberElement.value)) {
        alert('Пароль или логин не верны');
        return false;
    }

    let password = document.getElementById('password').value;
    if (password.length < 8) {
        alert('Пароль или логин не верны');
        return false;
    }
    
    let dataForm = new FormData();
    dataForm.append("userPhone", numberElement.value);
    dataForm.append("password", password);
    let responce = await fetch('/system/check-master-login', {
        method: 'POST',
        body: dataForm
    });
    let text = await responce.text();

    if (text == 'false') {
        alert('Логин или пароль введены неверно');
        return false;
    }

    this.document.getElementById("form").submit();
}