async function isSignupDataValid() {
    let numberElement = document.getElementById('userPhone');
    if (!/^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$/.test(numberElement.value)) {
        alert('Неверно введен номер');
        return false;
    }

    let password = document.getElementById('password').value;
    if (password.length < 8) {
        alert('Пароль должен содержать минимум 8 символов');
        return false;
    }
    
    let dataForm = new FormData();
    dataForm.append("userPhone", numberElement.value);
    let responce = await fetch('/system/is-number-occupied', {
        method: 'POST',
        body: dataForm
    });
    let text = await responce.text();
    
    if (text == 'true') {
        alert('Пользователь под этим номером уже зарегестрирован');
        return false;
    }

    this.document.getElementById("form").submit();
}