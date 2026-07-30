document.addEventListener('DOMContentLoaded', async () => {
    checkAuth();
    updateUserInfo();

    const urlParams = new URLSearchParams(window.location.search);
    const eventId = urlParams.get('event');

    if (!eventId) {
        console.error('Evento no especificado');
        return;
    }

    try {
        // 1. Obtener nombre del evento desde la memoria
        const savedEventName = sessionStorage.getItem('currentEventName');
        if (savedEventName) {
            document.getElementById('eventName').textContent = savedEventName;
        }

        // 2. Cargar sectores
        const sectors = await apiFetch(`/events/${eventId}/sectors`);
        const container = document.getElementById('sectorsContainer');

        if (sectors.length === 0) {
            container.innerHTML = '<p class="loading">No hay sectores disponibles</p>';
            return;
        }

        // 3. Renderizar tarjetas de sectores
        container.innerHTML = sectors.map(sector => `
            <div class="sector-card" onclick="window.location.href='seats.html?event=${eventId}&sector=${sector.id}'">
                <h3>${sector.name}</h3>
                <p class="sector-price">$${sector.price}</p>
                <p class="sector-capacity">🪑 ${sector.capacity} butacas disponibles</p>
                <button class="btn btn-primary">Ver Mapa de Asientos</button>
            </div>

        `).join('');

    } catch (error) {
        console.error(error);
        const container = document.getElementById('sectorsContainer');
        if (container) {
            container.innerHTML = `<p class="message error">Error: ${error.message}</p>`;
        }
    }
});