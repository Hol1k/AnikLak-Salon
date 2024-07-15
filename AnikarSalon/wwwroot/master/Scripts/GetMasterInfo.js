(async () => {
    let response = await fetch('/system/get-master-info', {
        method: 'POST',
        body: new FormData(),
        credentials: 'include'
    });

    let info = await response.json();
    document.getElementById('Name').innerHTML = info.name + ' ' + info.lastname;
    document.getElementById('ExpVar').innerHTML = info.exp;
    document.getElementById('SerVar').innerHTML = info.service;
    document.getElementById('About').firstChild.innerHTML = info.about;

    let message = document.getElementById('About');

    message.addEventListener('input', function handleChange(event) {
        UpdateAbout(event.target);
    });
})();

async function UpdateAbout(target) {
    let form = new FormData();
    form.append('about', target.value);

    let response = await fetch('/system/update-master-info', {
        method: 'POST',
        body: form,
        credentials: 'include'
    });
}