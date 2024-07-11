(async () => {
    await UpdateMasters('none');
    document.getElementById('ChosenDate').valueAsDate = new Date();
})();

async function UpdateMasters(filter) {
    let response = await fetch('/system/get-masters-list');
    let mastersList = await response.json();

    let MastersList = document.getElementById('MastersList');
    MastersList.innerHTML = '';

    mastersList.masters.forEach((master) => {
        if (master.service == filter || filter == 'none') {
        let masterDiv = document.createElement('div');
        masterDiv.className = 'block';
        masterDiv.id = master.id;
        MastersList.appendChild(masterDiv);

            let avatarDiv = document.createElement('div');
            avatarDiv.className = 'block';
            avatarDiv.id = 'Avatar';
            avatarDiv.style.background = `url(${master.avatarUrl})`;
            avatarDiv.style.backgroundSize = 'auto 100%';
            avatarDiv.style.backgroundPositionX = '50%';
            masterDiv.appendChild(avatarDiv);

            let infoDiv = document.createElement('div');
            infoDiv.className = 'block';
            infoDiv.id = 'Info';
            masterDiv.appendChild(infoDiv);

                let nameDiv = document.createElement('div');
                nameDiv.id = 'Name';
                infoDiv.appendChild(nameDiv);

                    let nameA = document.createElement('a');
                    nameA.innerHTML = master.name;
                    nameDiv.appendChild(nameA);

                let specializationDiv = document.createElement('div');
                specializationDiv.id = 'Specialization';
                specializationDiv.innerHTML = master.service;
                infoDiv.appendChild(specializationDiv);

                let button = document.createElement('input');
                button.type = 'button';
                button.className = 'ChoseMasterButton';
                button.value = 'Выбрать';
                button.setAttribute('onclick', `ChoseMaster('${masterDiv.id}'); ChoseDate()`);
                infoDiv.appendChild(button);
        }
    });
};

let manicureSpecialisation = {
    specialization: 'Мастер маникюра',
    services: ['Маникюр с дизайном', 'Уходовый']
}

function ChoseMaster(id) {
    let chose = document.getElementById('ChosenMaster');
    let master = document.getElementById(id);
    let serviceSelection = document.getElementById('ServiceSelection');

    chose.setAttribute('value', master.id.toString());
    chose.innerHTML = master.childNodes[1].firstChild.firstChild.innerHTML;
    chose.setAttribute('onclick', `document.getElementById("${master.id.toString()}").scrollIntoView();`);

    let masterSpecialisation = master.childNodes[1].childNodes[1].innerHTML;
    if (masterSpecialisation == manicureSpecialisation.specialization) { //Услуги мастера маникюра
        serviceSelection.innerHTML = '';
        let defaultOption = document.createElement('option');
        defaultOption.setAttribute('selected', '');
        serviceSelection.appendChild(defaultOption);
        manicureSpecialisation.services.forEach(service => {
            let option = document.createElement('option');
            option.value = service.replace(/ /g, '+');
            serviceSelection.value = service.replace(/ /g, '+');
            option.innerHTML = service;
            serviceSelection.appendChild(option);
        });
    }
}

function ChoseTime(time) {
    let timeDiv = document.getElementById(time);
    let timeList = document.getElementById('ChoseTime');
    timeList.childNodes.forEach(node => {
        node.className = '';
    });

    timeDiv.className = 'SelectedTime';
    timeList.setAttribute('value', timeDiv.id);
}

async function ChoseDate() {
    let chosenMaster = document.getElementById('ChosenMaster');
    let dateNode = document.getElementById('ChosenDate');
    if (chosenMaster.getAttribute('value') == 'none' || dateNode.value == '') {
        return;
    }

    let dataForm = new FormData();
    dataForm.append("date", dateNode.value);
    dataForm.append("masterId", chosenMaster.getAttribute('value'));
    let responce = await fetch('/system/check-free-registration-times', {
        method: 'POST',
        body: dataForm
    });
    let text = await responce.text();

    let freeRegTimesList = text.split(',');
    let regTimeDivList = document.getElementById('ChoseTime');
    regTimeDivList.innerHTML = '';

    freeRegTimesList.forEach(freeTime => {
        let time = document.createElement('div');
        time.setAttribute('onclick', `ChoseTime('${freeTime}')`);
        time.setAttribute('id', `${freeTime}`);
        time.innerHTML = freeTime;
        regTimeDivList.appendChild(time);
    });
}

async function SubmitRegistration() {
    let chosenMaster = document.getElementById('ChosenMaster');
    let chosenService = document.getElementById('ServiceSelection');
    let chosenDate = document.getElementById('ChosenDate');
    let chosenTime = document.getElementById('ChoseTime');

    if (chosenMaster.getAttribute('value') == 'none') { alert('Вы не выбрали мастера'); return };
    if (chosenService.getAttribute('value') == '') { alert('Вы не выбрали услугу'); return };
    if (chosenTime.getAttribute('value') == '') { alert('Вы не выбрали время'); return };
}