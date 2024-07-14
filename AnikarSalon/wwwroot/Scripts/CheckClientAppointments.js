(async () => {
    let response = await fetch('/system/check-client-appointments', {
        method: 'GET',
        credentials: 'include'
    });

    let appointmentsList = await response.json();

    document.getElementById('Name').innerHTML = appointmentsList.name;

    let appointments = document.getElementById('Appointments');
    appointmentsList.appointments.forEach(appointment => {
        let appointmentDiv = document.createElement('div');
        appointments.insertAdjacentElement('afterbegin', appointmentDiv);
        appointmentDiv.className = 'Appointment';
        appointmentDiv.id = appointment.appointmentId;
        if (appointment.status == 'Выполнен' || appointment.status == 'Отменен') appointmentDiv.style = 'background: #C3BCB0';

            let masterDiv = document.createElement('div');
            appointmentDiv.appendChild(masterDiv);
            masterDiv.className = 'masterCol';
            masterDiv.innerHTML = appointment.master;

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

            let statusDiv = document.createElement('div');
            appointmentDiv.appendChild(statusDiv);
            statusDiv.className = 'statusCol';
            statusDiv.innerHTML = appointment.status;
    });
})();
