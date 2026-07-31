let currentPage = 1;
const pageSize = 10;

document.addEventListener('DOMContentLoaded', async () => {
    checkAuth();
    updateUserInfo();

    try {
        await loadEvents(currentPage);
    } catch (error) {
        showError('Error al cargar eventos: ' + error.message);
    }
});

async function loadEvents(page = 1) {
    const container = document.getElementById('eventsContainer');
    const paginationContainer = document.getElementById('paginationContainer');

    try {
        // Hacemos el fetch dinámico según la página que pasemos
        const response = await apiFetch(`/events?Page=${page}&PageSize=${pageSize}`);
        console.log(response);

        // En C# .NET por defecto los JSON devuelven propiedades en camelCase (data, total, page, pageSize)
        const eventsList = response.data;
        const totalItems = response.total;
        const pageNumber = response.page;
        const size = response.pageSize;

        if (!eventsList || eventsList.length === 0) {
            container.innerHTML = '<p class="loading">No hay eventos disponibles</p>';
            paginationContainer.innerHTML = '';
            return;
        }

        // 1. Renderizar Eventos
        container.innerHTML = eventsList.map(event => `
            <div class="event-card" onclick="goToSeats(${event.id}, '${event.name}')">
                <h3>${event.name}</h3>
                <p>${new Date(event.eventDate).toLocaleDateString()}</p>
                <p>${event.venue}</p>
            </div>
        `).join('');

        // 2. Renderizar Controles de Paginación
        renderPagination(totalItems, pageNumber, size);

    } catch (error) {
        container.innerHTML = `<p class="message error">Error: ${error.message}</p>`;
    }
}

function renderPagination(totalItems, currentPage, pageSize) {
    const paginationContainer = document.getElementById('paginationContainer');
    const totalPages = Math.ceil(totalItems / pageSize);

    // Si hay 1 sola página o no hay ítems, no mostramos la botonera
    if (totalPages <= 1) {
        paginationContainer.innerHTML = '';
        return;
    }

    paginationContainer.innerHTML = `
        <button class="btn-page" ${currentPage === 1 ? 'disabled' : ''} onclick="changePage(${currentPage - 1})">
            &laquo; Anterior
        </button>
        
        <span class="page-info">Página ${currentPage} de ${totalPages}</span>
        
        <button class="btn-page" ${currentPage === totalPages ? 'disabled' : ''} onclick="changePage(${currentPage + 1})">
            Siguiente &raquo;
        </button>
    `;
}

function changePage(newPage) {
    currentPage = newPage;
    loadEvents(currentPage);
    window.scrollTo({ top: 0, behavior: 'smooth' }); // Scroll arriba al cambiar de página
}

function goToSeats(eventId, eventName) {
    sessionStorage.setItem('currentEventId', eventId);
    sessionStorage.setItem('currentEventName', eventName);
    window.location.href = `sectors.html?event=${eventId}`;
}

function showError(message) {
    const container = document.getElementById('eventsContainer');
    container.innerHTML = `<p class="message error">${message}</p>`;
}