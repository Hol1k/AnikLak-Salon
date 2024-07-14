(async () => {
    let response = await fetch('/system/check-master-appointments', {
        method: 'GET',
        credentials: 'include'
    });

    let appointmentsList = await response.json();

    let appointments = document.getElementById('Appointments');
    appointmentsList.appointments.forEach(appointment => {
        let appointmentDiv = document.createElement('div');
        appointments.insertAdjacentElement('afterbegin', appointmentDiv);
        appointmentDiv.className = 'Appointment';
        appointmentDiv.id = appointment.appointmentId;
        if (appointment.status == 'Выполнен' || appointment.status == 'Отменен') appointmentDiv.style = 'background: #C3BCB0';

            let clientDiv = document.createElement('div');
            appointmentDiv.appendChild(clientDiv);
            clientDiv.className = 'clientCol';
            clientDiv.innerHTML = appointment.client;

            let numberDiv = document.createElement('div');
            appointmentDiv.appendChild(numberDiv);
            numberDiv.className = 'numberCol';
            numberDiv.innerHTML = appointment.number;

            let serviceDiv = document.createElement('div');
            appointmentDiv.appendChild(serviceDiv);
            serviceDiv.className = 'serviceCol';
            serviceDiv.innerHTML = appointment.service;

            let priceDiv = document.createElement('div');
            appointmentDiv.appendChild(priceDiv);
            priceDiv.className = 'priceCol';
            priceDiv.innerHTML = appointment.price + ' ₽';

            let dateDiv = document.createElement('div');
            appointmentDiv.appendChild(dateDiv);
            dateDiv.className = 'dateCol';
            dateDiv.innerHTML = appointment.date;

            let statusDiv = document.createElement('select');
            appointmentDiv.appendChild(statusDiv);
            statusDiv.className = 'statusCol';
            statusDiv.name = 'status';
            statusDiv.setAttribute('onchange', 'updateStatus(this.parentElement)');

                let statuses = ['Обрабатывается', 'Принят', 'Выполнен', 'Отменен'];
                statuses.forEach(status =>{
                    let option = document.createElement('option');
                    statusDiv.appendChild(option);
                    option.value = status;
                    option.innerHTML = status;
                    if (status == appointment.status) option.setAttribute('selected', '');
                });
    });
})();

async function updateStatus(appontDiv) {
    let form = document.createElement('form');

    document.getElementById('Appointments').appendChild(form);

    let idInput = document.createElement('input');
    form.appendChild(idInput);
    idInput.name = 'appointId';
    idInput.value = appontDiv.id;

    form.appendChild(appontDiv.lastChild);

    form.submit();

    document.getElementById('Appointments').removeChild(form);
}
