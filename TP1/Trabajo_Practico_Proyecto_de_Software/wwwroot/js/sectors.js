document.addEventListener('DOMContentLoaded', async () => {
    checkAuth();
    updateUserInfo();

    const urlParams = new URLSearchParams(window.location.search);
    const eventId = urlParams.get('event');

    if (!eventId) {
        showMessage('Evento no especificado', 'error');
        return;
    }

    try {
        // 1. Obtener nombre del evento dinámicamente
        const events = await apiFetch('/events');
        const event = events.find(e => e.id == eventId);
        document.getElementById('eventName').textContent = event?.name || 'Evento';

        // 2. Cargar sectores
        const sectors = await apiFetch(`/events/${eventId}/sectors`);
        const container = document.getElementById('sectorsContainer');

        if (sectors.length === 0) {
            container.innerHTML = '<p class="loading">No hay sectores disponibles para este evento</p>';
            return;
        }

        // 3. Renderizar tarjetas de sectores (adaptable a 1, 2, 10 o N sectores)
        container.innerHTML = sectors.map(sector => `
            <div class="sector-card" onclick="window.location.href='seats.html?event=${eventId}&sector=${sector.id}'">
                <h3>${sector.name}</h3>
                <p class="sector-price">$${sector.price}</p>
                <p class="sector-capacity">🪑 ${sector.capacity} butacas disponibles</p>
                <button class="btn btn-primary">Ver Mapa de Asientos</button>
            </div>
        `).join('');
    } catch (error) {
        showMessage('Error al cargar sectores: ' + error.message, 'error');
    }
});