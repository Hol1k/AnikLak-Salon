(async () => {
    let response = await fetch('/system/username', {
        method: 'GET',
        credentials: 'include'
    });

    if (response.ok) {
        let username = document.getElementById('username');
        let text = await response.text();
        if (text.length > 0) {
            username.innerHTML = text;
            username.href = '/';
        }
    }
})();
