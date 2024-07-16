(async () => {
    let response = await fetch('/system/get-master-info?masterId=' + new URLSearchParams(window.location.search).get('masterId'), {
        method: 'POST',
        body: new FormData(),
        credentials: 'include'
    });

    let info = await response.json();
    document.getElementById('Name').innerHTML = info.name + ' ' + info.lastname;
    document.getElementById('ExpVar').innerHTML = info.exp;
    document.getElementById('SerVar').innerHTML = info.service;
    document.getElementById('About').innerHTML = info.about;

    let avatarDiv = document.getElementById('Avatar');
    avatarDiv.style.background = `url(${info.avatar})`;
    avatarDiv.style.backgroundSize = 'auto 100%';
    avatarDiv.style.backgroundPositionX = '50%';

    await GetComments();
    if (document.getElementById('username').innerHTML != 'Войти') await WriteComment();
})();

function Registrate() {
    setCookie('chosenMaster', new URLSearchParams(window.location.search).get('masterId'))

    location.href = '/registration';
}

async function GetComments() {
    let responce = await fetch('/system/get-comments?masterId=' + new URLSearchParams(window.location.search).get('masterId'), {
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

    if (commentsList.comments.length === 0) {
        let emptyComment = document.createElement('div');
        commentsDiv.appendChild(emptyComment);
        emptyComment.setAttribute('id', 'NoComments');
        emptyComment.setAttribute('lang', 'ru');
        emptyComment.innerHTML = 'У этого мастера еще нет отзывов.';
    }
    else if (document.getElementById('username').innerHTML == 'Войти') {
        let emptyComment = document.createElement('div');
        commentsDiv.appendChild(emptyComment);
        emptyComment.setAttribute('id', 'NoComments');
        emptyComment.setAttribute('lang', 'ru');
        emptyComment.innerHTML = 'Авторизуйтесь, чтобы оставить отзыв.';
    }
    else {
        let emptyComment = document.createElement('div');
        commentsDiv.appendChild(emptyComment);
        emptyComment.setAttribute('id', 'NoComments');
        emptyComment.setAttribute('lang', 'ru');
    }
}

async function WriteComment() {
    let response = await fetch('/system/is-client-was-at-master?masterId=' + new URLSearchParams(window.location.search).get('masterId'), {
        method: 'GET',
        credentials: 'include'
    });
    let answer = await response.text();

    if (document.getElementById('Comments').lastChild.id == 'NoComments') {
        if (answer == 'false') document.getElementById('NoComments').innerHTML += ' Сходите к этому мастеру, чтобы оставить отзыв.';
        if (answer == 'true') {
            if (document.getElementById('NoComments').innerHTML == 'У этого мастера еще нет отзывов.') document.getElementById('NoComments').innerHTML += ' Напишите его первым!';
            else document.getElementById('NoComments').remove();

            let commentsDiv = document.getElementById('Comments');
            let writeCommentDiv = document.createElement('div');
            commentsDiv.insertAdjacentElement('afterbegin', writeCommentDiv);
            writeCommentDiv.setAttribute('id', 'WriteComment');
            writeCommentDiv.innerHTML = '<textarea id="text_area" rows="2" onkeyup="textarea_resize(event, 20, 2);"></textarea><div id="text_area_div"></div><button id="SendComment" onclick="SendComment()">Отправить</button>'
        }
    }
}

function SendComment() {
    if (document.getElementById('text_area_div').innerHTML.length >= 5) {
        location.href = '/system/write-comment?masterId=' + new URLSearchParams(window.location.search).get('masterId') + '&commentText=' + document.getElementById('text_area_div').innerHTML;
    }
    else alert('Слишком короткий отзыв.');
}

function setCookie(name, value, options = {}) {

    options = {
        path: '/',
    };

    if (options.expires instanceof Date) {
        options.expires = options.expires.toUTCString();
    }

    let updatedCookie = encodeURIComponent(name) + "=" + encodeURIComponent(value);

    for (let optionKey in options) {
        updatedCookie += "; " + optionKey;
        let optionValue = options[optionKey];
        if (optionValue !== true) {
            updatedCookie += "=" + optionValue;
        }
    }

    document.cookie = updatedCookie;
}

function textarea_resize(event, line_height, min_line_count) {
    var min_line_height = min_line_count * line_height;
    var obj = event.target;
    var div = document.getElementById(obj.id + '_div');
    div.innerHTML = obj.value;
    var obj_height = div.offsetHeight;
    if (event.keyCode == 13)
        obj_height += line_height;
    else if (obj_height < min_line_height)
        obj_height = min_line_height;
    obj.style.height = obj_height + 50 + 'px';
}