let currentEventId = null;
let currentSectorId = null;

document.addEventListener('DOMContentLoaded', async () => {
    checkAuth();
    updateUserInfo();

    const urlParams = new URLSearchParams(window.location.search);
    currentEventId = urlParams.get('event');
    currentSectorId = urlParams.get('sector');

    if (!currentEventId || !currentSectorId) {
        showToast('Datos incompletos. Redirigiendo...', 'error');
        setTimeout(() => window.location.href = 'index.html', 1500);
        return;
    }

    try {

        const events = await apiFetch('/events');
        const event = events.find(e => e.id == currentEventId);
        document.getElementById('eventName').textContent = event?.name || 'Evento';

        const sectors = await apiFetch(`/events/${currentEventId}/sectors`);
        const sector = sectors.find(s => s.id == currentSectorId);
        document.getElementById('sectorName').textContent = sector?.name || 'Sector';

        await loadSeats();
    } catch (error) {
        showToast('Error cargando información: ' + error.message, 'error');
    }
});

async function loadSeats() {
    const container = document.getElementById('seatsContainer');
    container.innerHTML = '<p class="loading">⏳ Cargando asientos...</p>';

    try {
        console.log('Intentando cargar sector:', currentSectorId);

        const seats = await apiFetch(`/sectors/${currentSectorId}/seats`);
        console.log('✅ Asientos recibidos:', seats.length);

        if (seats.length === 0) {
            container.innerHTML = '<p class="message info">No hay asientos en este sector</p>';
            return;
        }

        
        const rows = {};
        seats.forEach(seat => {
            console.log(`Asiento ${seat.seatNumber}: Status="${seat.status}", Row="${seat.rowIdentifier}"`);
            const row = seat.rowIdentifier || 'A';
            if (!rows[row]) rows[row] = [];
            rows[row].push(seat);
        });

        const sortedRows = Object.keys(rows).sort();
        let html = '';

        sortedRows.forEach(row => {
            html += `
            <div class="seat-row">
                <div class="row-label">Fila ${row}</div>
                <div class="seats-grid">
                    ${rows[row].map(seat => {
                const statusLower = (seat.status || '').toLowerCase();
                const isAvailable = statusLower === 'available';

                return `
                    <button class="seat ${statusLower}" 
                            data-seat-id="${seat.id}" 
                            data-seat-number="${seat.seatNumber}"
                            ${!isAvailable ? 'disabled' : ''}
                            ${isAvailable ? `onclick="reserveSeat('${seat.id}', ${seat.seatNumber}, '${row}')"` : ''}>
                        ${seat.seatNumber}
                    </button>
                `;
            }).join('')}
                </div>
            </div>`;
        });

        container.innerHTML = html;
        console.log('✅ Mapa renderizado correctamente');

    } catch (error) {
        console.error('❌ Error cargando asientos:', error);
        showToast(`⚠️ No se pudo actualizar el mapa de asientos`, 'error');

        const container = document.getElementById('seatsContainer');
        if (container.innerHTML.includes('Cargando asientos')) {
            container.innerHTML = `<p class="message error">No se pudo cargar el mapa. Verificá tu conexión.</p>`;
        }
    }
} 



async function reserveSeat(seatId, seatNumber, row) {
        const userId = checkAuth();
        if (!userId) return;

    const button = document.querySelector(`[data-seat-id="${seatId}"]`);
    const originalText = button.textContent;

    // --- 1. ESTADO OPTIMISTA ---
    button.disabled = true;
    button.textContent = '⏳';

    try {
        const reservation = await apiFetch('/reservations', {
            method: 'POST',
            body: JSON.stringify({ seatId: seatId, userId: parseInt(userId) })
        }, 0);

        const sectors = await apiFetch(`/events/${currentEventId}/sectors`);
        const currentSector = sectors.find(s => s.id == currentSectorId);

        const reservationData = {
            id: reservation.id,
            seatNumber: seatNumber,
            row: row,
            sectorName: document.getElementById('sectorName').textContent,
            eventName: document.getElementById('eventName').textContent,
            price: currentSector?.price || 0,
            expiresAt: new Date(Date.now() + 5 * 60 * 1000).toISOString()
        };

        sessionStorage.setItem(`reservation_${reservation.id}`, JSON.stringify(reservationData));

        // --- 2. NAVEGACIÓN A CHECKOUT ---
        window.location.href = `checkout.html?reservation=${reservation.id}`;

    } catch (error) {
        console.error("Reserva fallida:", error);

        // --- 3. ROLLBACK VISUAL ---
        button.textContent = originalText;

        const mensajeError = error.message.toLowerCase();

        if (mensajeError.includes('not available') || mensajeError.includes('400') || mensajeError.includes('reserv')) {
            button.classList.remove('available');
            button.classList.add('reserved');

            button.disabled = true;
            button.removeAttribute('onclick');
        } else {
            button.disabled = false;
        }

        // --- 4. FEEDBACK DE ÉXITO/ERROR ---
        showToast(`❌ Error: ${error.message}`, 'error');
    }
}