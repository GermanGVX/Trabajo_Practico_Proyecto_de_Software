let currentEventId = null;
let currentSectorId = null;

document.addEventListener('DOMContentLoaded', async () => {
    // Verificar autenticación
    checkAuth();
    updateUserInfo();

    // Obtener parámetros de URL
    const urlParams = new URLSearchParams(window.location.search);
    currentEventId = urlParams.get('event');

    if (!currentEventId) {
        showMessage('No se especificó un evento', 'error');
        return;
    }

    try {
        await loadSectors();
    } catch (error) {
        showMessage('Error al cargar sectores: ' + error.message, 'error');
    }
});

async function loadSectors() {
    try {
        const sectors = await apiFetch(`/events/${currentEventId}/sectors`);

        if (sectors.length === 0) {
            showMessage('No hay sectores disponibles', 'error');
            return;
        }

        // Usamos el primer sector (para simplificar)
        currentSectorId = sectors[0].id;

        document.getElementById('sectorName').textContent = sectors[0].name;
        document.getElementById('sectorPrice').textContent = `Precio: $${sectors[0].price}`;
        document.getElementById('eventName').textContent = sessionStorage.getItem('currentEventName') || 'Evento';

        await loadSeats();
    } catch (error) {
        showMessage('Error al cargar sectores: ' + error.message, 'error');
    }
}

async function loadSeats() {
    const container = document.getElementById('seatsContainer');

    try {
        const seats = await apiFetch(`/sectors/${currentSectorId}/seats`);

        if (seats.length === 0) {
            container.innerHTML = '<p class="loading">No hay asientos disponibles</p>';
            return;
        }

        container.innerHTML = seats.map(seat => `
            <button 
                class="seat ${seat.status.toLowerCase()}" 
                data-seat-id="${seat.id}"
                data-seat-number="${seat.seatNumber}"
                onclick="reserveSeat('${seat.id}', ${seat.seatNumber})"
                ${seat.status !== 'Available' ? 'disabled' : ''}
            >
                ${seat.seatNumber}
            </button>
        `).join('');
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
            body: JSON.stringify({
                seatId: seatId,
                userId: parseInt(userId)
            })
        });

        // Éxito
        showMessage(
            `✅ ¡Reserva exitosa!<br>
             Asiento ${seatNumber}<br>
             ⏰ Tienes 5 minutos para completar el pago<br>
             Expira: ${new Date(reservation.expiresAt).toLocaleTimeString()}`,
            'success'
        );

        // Recargar mapa
        setTimeout(() => loadSeats(), 1000);

    } catch (error) {
        // Error 400 o 409
        showMessage(
            `❌ ${error.message}<br>
             El asiento ${seatNumber} ya no está disponible`,
            'error'
        );

        // Recargar mapa
        setTimeout(() => loadSeats(), 2000);
    }
}

function showMessage(text, type) {
    const box = document.getElementById('messageBox');
    box.innerHTML = text;
    box.className = `message ${type}`;

    // Auto-ocultar después de 5 segundos
    setTimeout(() => {
        box.className = 'message';
    }, 5000);
}