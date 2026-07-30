document.addEventListener('DOMContentLoaded', async () => {
    // Verificar autenticación
    checkAuth();
    updateUserInfo();

    try {
        await loadEvents();
    } catch (error) {
        showError('Error al cargar eventos: ' + error.message);
    }
});

async function loadEvents() {
    const container = document.getElementById('eventsContainer');

    try {
        // 1. Guardamos la respuesta del servidor (el objeto completo con la paginación)
        const response = await apiFetch('/events?Page=1&PageSize=10');
        console.log(response);

        // 2. Extraemos el arreglo de eventos apuntando a la propiedad "data"
        const eventsList = response.data;

        // 3. Ahora eventsList sí existe y tiene el arreglo adentro
        if (!eventsList || eventsList.length === 0) {
            container.innerHTML = '<p class="loading">No hay eventos disponibles</p>';
            return;
        }

        container.innerHTML = eventsList.map(event => `
            <div class="event-card" onclick="goToSeats(${event.id}, '${event.name}')">
                <h3>${event.name}</h3>
                <p> ${new Date(event.eventDate).toLocaleDateString()}</p>
                <p> ${event.venue}</p>
            </div>
        `).join('');
    } catch (error) {
        container.innerHTML = `<p class="message error">Error: ${error.message}</p>`;
    }
}

function goToSeats(eventId, eventName) {
    // Guardar info del evento 
    sessionStorage.setItem('currentEventId', eventId);
    sessionStorage.setItem('currentEventName', eventName);
    window.location.href = `sectors.html?event=${eventId}`;
}

function showError(message) {
    const container = document.getElementById('eventsContainer');
    container.innerHTML = `<p class="message error">${message}</p>`;
}