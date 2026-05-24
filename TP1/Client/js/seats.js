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

        const groupedSeats = await apiFetch(`/Seat/sector/${currentSectorId}`);

        console.log('✅ Grupos recibidos:', groupedSeats.length);


        if (groupedSeats.length === 0) {
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

        groupedSeats.forEach(group => {
            html += `

                <div class="seat-row">
                    <div class="row-label">Fila ${group.row}</div>
                    <div class="seats-grid">
                        ${group.seats.map(seat => {

                const statusLower = (seat.status || '').toLowerCase();
                const isAvailable = statusLower === 'available';

                let estadoTexto = 'Desconocido';
                if (statusLower === 'available') estadoTexto = 'Disponible';
                else if (statusLower === 'reserved' || statusLower === 'pending') estadoTexto = 'En proceso';
                else if (statusLower === 'sold' || statusLower === 'not available') estadoTexto = 'Vendido';

                const iconoSvg = `
                                <svg class="seat-icon" viewBox="0 0 24 24" fill="currentColor">
                                    <path d="M7 13c-1.1 0-2 .9-2 2v4h14v-4c0-1.1-.9-2-2-2H7zM17 10V7c0-1.66-1.34-3-3-3h-4c-1.66 0-3 1.34-3 3v3H5v4h14v-4h-2z"/>
                                </svg>
                            `;

                return `
                                <button type="button" class="seat ${statusLower}" 
                                        data-seat-id="${seat.id}" 
                                        data-seat-number="${seat.seatNumber}"
                                        title="Asiento ${seat.seatNumber} - ${estadoTexto}"
                                        aria-label="Asiento ${seat.seatNumber} - ${estadoTexto}"
                                        ${!isAvailable ? 'disabled' : ''}
                                        ${isAvailable ? `onclick="reserveSeat('${seat.id}', ${seat.seatNumber}, '${group.row}')"` : ''}>
                                    
                                    ${iconoSvg}
                                    <span class="seat-number">${seat.seatNumber}</span>
                                </button>
                            `;
            }).join('')}
                </div>
            </div>`;
        });

        container.innerHTML = html;
        console.log('✅ Mapa renderizado correctamente con datos agrupados');

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

    const numberSpan = button.querySelector('.seat-number');
    const originalNumber = numberSpan.textContent;

    // --- 1. ESTADO OPTIMISTA ---
    button.disabled = true;
    numberSpan.textContent = '⏳';

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
        numberSpan.textContent = originalNumber;

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