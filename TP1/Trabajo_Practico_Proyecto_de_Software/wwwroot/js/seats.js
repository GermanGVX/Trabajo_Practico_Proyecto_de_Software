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
    container.innerHTML = '<p class="loading">Cargando asientos...</p>';

    try {
        console.log('🔍 Intentando cargar sector:', currentSectorId);

        
        const response = await fetch(`${API_BASE}/Seat/sector/${currentSectorId}`);

        console.log('📡 Status:', response.status);
        console.log('📡 Content-Type:', response.headers.get('content-type'));

        if (!response.ok) {
            const errorText = await response.text();
            console.error('❌ Error HTTP:', errorText);
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }

        
        const contentType = response.headers.get('content-type');
        if (!contentType || !contentType.includes('application/json')) {
            const text = await response.text();
            console.error('❌ No es JSON:', text.substring(0, 200));
            throw new Error('El servidor no devolvió JSON válido');
        }

        const seats = await response.json();
        console.log('✅ Asientos recibidos:', seats.length);
        console.log('🪑 Primer asiento:', seats[0]);

        if (seats.length === 0) {
            container.innerHTML = '<p class="loading">No hay asientos en este sector</p>';
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
                </div>
            `;
        });

        container.innerHTML = html;
        console.log('✅ Mapa renderizado correctamente');
    } catch (error) {
        console.error('❌ Error cargando asientos:', error);
        container.innerHTML = `
            <p class="message error">
                ⚠️ Error: ${error.message}<br>
                <small>Revisá la consola (F12) para más detalles</small>
            </p>`;
    }
}




//async function reserveSeat(seatId, seatNumber, row) {
//    const userId = checkAuth();
//    if (!userId) return;

//    const button = document.querySelector(`[data-seat-id="${seatId}"]`);
//    const originalText = button.textContent;
//    button.disabled = true;
//    button.textContent = '...';

//    try {

//        const reservation = await apiFetch('/reservations', {
//            method: 'POST',
//            body: JSON.stringify({ seatId: seatId, userId: parseInt(userId) })
//        });

//        sessionStorage.setItem('currentReservationId', reservation.id);
//        sessionStorage.setItem('reservationExpiresAt', reservation.expiresAt);
//        sessionStorage.setItem('reservationSeatNumber', seatNumber);
//        sessionStorage.setItem('reservationRow', row);
//        sessionStorage.setItem('reservationSectorName', document.getElementById('sectorName').textContent);
//        sessionStorage.setItem('reservationEventName', document.getElementById('eventName').textContent);


//        window.location.href = `checkout.html?reservation=${reservation.id}`;

//    } catch (error) {

//        console.error("Reserva fallida:", error);
//        const errorMsg = error.message.includes("JSON")
//            ? "Error de conexión con el servidor. Intenta nuevamente."
//            : error.message;

//        showMessage(`❌ ${errorMsg}`, 'error');



//        setTimeout(() => {
//            button.disabled = false;
//            button.textContent = originalText;
//            loadSeats();
//        }, 1500);
//    }
//}

async function reserveSeat(seatId, seatNumber, row) {
    const userId = checkAuth();
    if (!userId) return;

    const button = document.querySelector(`[data-seat-id="${seatId}"]`);
    const originalText = button.textContent;
    button.disabled = true;
    button.textContent = '...';

    try {
        const reservation = await apiFetch('/reservations', {
            method: 'POST',
            body: JSON.stringify({ seatId: seatId, userId: parseInt(userId) })
        });
        const sectors = await apiFetch(`/events/${currentEventId}/sectors`);
        const currentSector = sectors.find(s => s.id == currentSectorId);
        const sectorPrice = currentSector?.price || 0;
        
        const reservationData = {
            id: reservation.id,
            seatNumber: seatNumber,
            row: row,
            sectorName: document.getElementById('sectorName').textContent,
            eventName: document.getElementById('eventName').textContent,
            price: sectorPrice, 
            expiresAt: new Date(Date.now() + 5 * 60 * 1000).toISOString()
        };

        
        sessionStorage.setItem(`reservation_${reservation.id}`, JSON.stringify(reservationData));

        console.log('✅ Datos guardados:', reservationData);
        console.log('🔑 Clave usada:', `reservation_${reservation.id}`);

        // Redirigir
        window.location.href = `checkout.html?reservation=${reservation.id}`;

    } catch (error) {
        console.error("Reserva fallida:", error);
        showMessage(`❌ ${error.message}<br>La butaca ya no está disponible.`, 'error');

        setTimeout(() => {
            button.disabled = false;
            button.textContent = originalText;
            loadSeats();
        }, 1500);
    }
}


function showMessage(text, type) {
    const box = document.getElementById('messageBox');
    box.innerHTML = text;
    box.className = `message ${type} show`;
    setTimeout(() => box.classList.remove('show'), 6000);
}