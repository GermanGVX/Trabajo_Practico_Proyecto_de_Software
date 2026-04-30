let currentEventId = null;
let currentSectorId = null;

document.addEventListener('DOMContentLoaded', async () => {
    checkAuth();
    updateUserInfo();

    const urlParams = new URLSearchParams(window.location.search);
    currentEventId = urlParams.get('event');
    currentSectorId = urlParams.get('sector');

    if (!currentEventId || !currentSectorId) {
        showMessage('Datos incompletos. Redirigiendo...', 'error');
        setTimeout(() => window.location.href = 'index.html', 1500);
        return;
    }

    try {
        // Cargar nombres dinámicamente (corrige el bug de "Concierto de Rock")
        const events = await apiFetch('/events');
        const event = events.find(e => e.id == currentEventId);
        document.getElementById('eventName').textContent = event?.name || 'Evento';

        const sectors = await apiFetch(`/events/${currentEventId}/sectors`);
        const sector = sectors.find(s => s.id == currentSectorId);
        document.getElementById('sectorName').textContent = sector?.name || 'Sector';

        await loadSeats();
    } catch (error) {
        showMessage('Error: ' + error.message, 'error');
    }
});

async function loadSeats() {
    const container = document.getElementById('seatsContainer');

    try {
        // ✅ URL corregida para coincidir con tu Controller
        const seats = await apiFetch(`/Seat/sector/${currentSectorId}`);

        if (seats.length === 0) {
            container.innerHTML = '<p class="loading">No hay asientos en este sector</p>';
            return;
        }

        // Agrupar por fila (se adapta a cualquier cantidad de asientos)
        const rows = {};
        seats.forEach(seat => {
            const row = seat.rowIdentifier || 'A';
            if (!rows[row]) rows[row] = [];
            rows[row].push(seat);
        });

        // Ordenar filas alfabéticamente (A, B, C...)
        const sortedRows = Object.keys(rows).sort();

        // Renderizar por filas
        let html = '';
        sortedRows.forEach(row => {
            html += `
                <div class="seat-row">
                    <div class="row-label">Fila ${row}</div>
                    <div class="seats-grid">
                        ${rows[row].map(seat => `
                            <button class="seat ${seat.status.toLowerCase()}" 
                                    data-seat-id="${seat.id}" 
                                    data-seat-number="${seat.seatNumber}"
                                    onclick="${seat.status === 'Available' ? `reserveSeat('${seat.id}', ${seat.seatNumber})` : ''}"
                                    ${seat.status !== 'Available' ? 'disabled' : ''}>
                                ${seat.seatNumber}
                            </button>
                        `).join('')}
                    </div>
                </div>
            `;
        });

        container.innerHTML = html;
    } catch (error) {
        container.innerHTML = `<p class="message error">Error: ${error.message}</p>`;
    }
}

async function reserveSeat(seatId, seatNumber) {
    const userId = checkAuth();
    if (!userId) return;

    const button = document.querySelector(`[data-seat-id="${seatId}"]`);
    button.disabled = true;
    button.textContent = '...';

    try {
        const reservation = await apiFetch('/reservations', {
            method: 'POST',
            body: JSON.stringify({ seatId, userId: parseInt(userId) })
        });

        showMessage(`✅ ¡Reserva exitosa!<br>Asiento ${seatNumber}<br> Expira: ${new Date(reservation.expiresAt).toLocaleTimeString()}`, 'success');
        setTimeout(() => loadSeats(), 1000);
    } catch (error) {
        showMessage(`❌ ${error.message}`, 'error');
        setTimeout(() => loadSeats(), 1500);
    }
}

function showMessage(text, type) {
    const box = document.getElementById('messageBox');
    box.innerHTML = text;
    box.className = `message ${type} show`;
    setTimeout(() => box.classList.remove('show'), 6000);
}