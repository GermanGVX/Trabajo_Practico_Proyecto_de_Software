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
        const events = await apiFetch('/events');

        if (events.length === 0) {
            container.innerHTML = '<p class="loading">No hay eventos disponibles</p>';
            return;
        }

        container.innerHTML = events.map(event => `
            <div class="event-card" onclick="goToSeats(${event.id}, '${event.name}')">
                <h3>${event.name}</h3>
                <p>📅 ${new Date(event.eventDate).toLocaleDateString()}</p>
                <p>📍 ${event.venue}</p>
            </div>
        `).join('');
    } catch (error) {
        container.innerHTML = `<p class="message error">Error: ${error.message}</p>`;
    }
}

function goToSeats(eventId, eventName) {
    // Guardar info del evento en sessionStorage
    sessionStorage.setItem('currentEventId', eventId);
    sessionStorage.setItem('currentEventName', eventName);
    window.location.href = `sectors.html?event=${eventId}`;
}

function showError(message) {
    const container = document.getElementById('eventsContainer');
    container.innerHTML = `<p class="message error">${message}</p>`;
}