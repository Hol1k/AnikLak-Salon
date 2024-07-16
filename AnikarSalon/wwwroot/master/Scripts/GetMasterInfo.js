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

    let avatarDiv = document.getElementById('Avatar');
    avatarDiv.style.background = `url(${info.avatar})`;
    avatarDiv.style.backgroundSize = 'auto 100%';
    avatarDiv.style.backgroundPositionX = '50%';

    let message = document.getElementById('About');

    message.addEventListener('input', function handleChange(event) {
        UpdateAbout(event.target);
    });

    await GetComments();
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

async function GetComments() {
    let responce = await fetch('/system/get-comments', {
        method: 'GET'
    });

    let commentsList = await responce.json();

    let commentsDiv = document.getElementById('Comments');
    commentsList.comments.forEach(comment => {
        let commentDiv = document.createElement('div');
        commentsDiv.appendChild(commentDiv);
        commentDiv.className = 'Comment';
        commentDiv.setAttribute('id', comment.commentId);

        let authorDiv = document.createElement('div');
        commentDiv.appendChild(authorDiv);
        authorDiv.setAttribute('id', 'Author');
        authorDiv.innerHTML = comment.author;

        let dateDiv = document.createElement('div');
        commentDiv.appendChild(dateDiv);
        dateDiv.setAttribute('id', 'Date');
        dateDiv.innerHTML = comment.date;

        commentDiv.appendChild(document.createElement('br'));

        let textDiv = document.createElement('div');
        commentDiv.appendChild(textDiv);
        textDiv.setAttribute('id', 'CommentText');
        textDiv.innerHTML = comment.comment;
    });
}