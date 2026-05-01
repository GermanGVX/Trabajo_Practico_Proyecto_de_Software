let reservationId = null;
let expiresAt = null;
let timerInterval = null;

document.addEventListener('DOMContentLoaded', async () => {
    checkAuth();
    updateUserInfo();

    const params = new URLSearchParams(window.location.search);
    reservationId = params.get('reservation');

    if (!reservationId) {
        showMessage('ID de reserva inválido', 'error');
        return;
    }

    await loadReservationDetails();
});

//async function loadReservationDetails() {
//    try {

//        const data = JSON.parse(sessionStorage.getItem(`reservation_${reservationId}`));
//        if (!data) throw new Error('Datos de reserva no encontrados. Inicia el proceso desde el mapa.');

//        document.getElementById('eventTitle').textContent = data.eventName || 'Evento';
//        document.getElementById('seatInfo').textContent = `N° ${data.seatNumber} (Fila ${data.row})`;
//        document.getElementById('sectorInfo').textContent = data.sectorName;
//        document.getElementById('priceInfo').textContent = `$${data.price}`;


//        let dateStr = data.expiresAt;
//        if (!dateStr.endsWith('Z') && !dateStr.includes('+') && !dateStr.endsWith('00:00')) {
//            dateStr = dateStr + 'Z';
//        }
//        expiresAt = new Date(dateStr);


//        if (isNaN(expiresAt.getTime()) || (expiresAt - Date.now()) < 0) {
//            console.warn(' Fecha inválida o ya expirada, recalculando desde ahora');
//            expiresAt = new Date(Date.now() + 5 * 60 * 1000);
//        }

//        console.log(' Timer iniciado:', expiresAt.toLocaleTimeString(), '- Diferencia:', Math.round((expiresAt - Date.now()) / 1000), 'segundos');
//        startTimer();
//    } catch (error) {
//        showMessage(error.message, 'error');
//        setTimeout(() => window.location.href = 'seats.html', 3000);
//    }
//}

async function loadReservationDetails() {
    try {
        console.log('🔍 Buscando datos de reserva...');
        console.log('🔑 reservationId:', reservationId);

        // Listar todas las claves en sessionStorage (DEBUG)
        for (let i = 0; i < sessionStorage.length; i++) {
            const key = sessionStorage.key(i);
            console.log(`📦 SessionStorage key ${i}: ${key}`);
        }

        // Intentar obtener datos con la clave exacta
        const storageKey = `reservation_${reservationId}`;
        const storedData = sessionStorage.getItem(storageKey);

        console.log('📥 Datos brutos de sessionStorage:', storedData);

        if (!storedData) {
            console.error('❌ No se encontraron datos con la clave:', storageKey);
            throw new Error('Datos de reserva no encontrados. Inicia el proceso desde el mapa.');
        }

        const data = JSON.parse(storedData);
        console.log('✅ Datos parseados:', data);

        // Verificar que existan todos los campos
        if (!data.eventName || !data.seatNumber || !data.sectorName) {
            console.warn('⚠️ Datos incompletos en sessionStorage');
        }

        document.getElementById('eventTitle').textContent = data.eventName || 'Evento';
        document.getElementById('seatInfo').textContent = `N° ${data.seatNumber} (Fila ${data.row || 'A'})`;
        document.getElementById('sectorInfo').textContent = data.sectorName;
        document.getElementById('priceInfo').textContent = `$${data.price || '5000'}`;

        // Calcular expiración
        if (data.expiresAt) {
            expiresAt = new Date(data.expiresAt);
            console.log('🕐 Expira:', expiresAt);
        } else {
            // Fallback: 5 minutos desde ahora
            expiresAt = new Date(Date.now() + 5 * 60 * 1000);
            console.warn('⚠️ No hay expiresAt, usando +5min desde ahora');
        }

        startTimer();
    } catch (error) {
        console.error('❌ Error en loadReservationDetails:', error);
        showMessage(error.message, 'error');
        setTimeout(() => window.location.href = 'seats.html', 3000);
    }
}

function startTimer() {
    const timerEl = document.getElementById('timerValue');
    const timerBox = document.getElementById('timerBox');

    timerInterval = setInterval(() => {
        const now = new Date();
        const diff = expiresAt - now;

        if (diff <= 0) {
            clearInterval(timerInterval);
            timerEl.textContent = "00:00";
            timerBox.classList.add('expired');
            showMessage('⏰ El tiempo de reserva ha expirado. Redirigiendo...', 'error');
            setTimeout(() => window.location.href = 'seats.html', 2000);
            return;
        }

        const minutes = Math.floor(diff / 60000);
        const seconds = Math.floor((diff % 60000) / 1000);
        timerEl.textContent = `${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;

        // Advertencia visual cuando queda menos de 1 minuto
        if (diff < 60000) {
            timerBox.classList.add('warning');
        }
    }, 1000);
}

async function confirmPayment() {
    const btn = document.getElementById('btnConfirm');
    btn.disabled = true;
    btn.textContent = 'Procesando...';

    try {
        await apiFetch(`/reservations/${reservationId}/confirm`, { method: 'POST' });
        showMessage('✅ ¡Pago confirmado! Butaca vendida exitosamente.', 'success');
        setTimeout(() => window.location.href = 'index.html', 2000);
    } catch (error) {
        showMessage(`❌ ${error.message}`, 'error');
        btn.disabled = false;
        btn.textContent = '✅ Confirmar Pago';
    }
}

async function cancelReservation() {
    if (!confirm('¿Seguro que querés cancelar? La butaca volverá a estar disponible inmediatamente.')) {
        return;
    }

    const btn = document.getElementById('btnCancel');
    const originalText = btn.textContent;
    btn.disabled = true;
    btn.textContent = 'Cancelando...';

    try {
        // Llamar al backend para liberar la butaca
        await apiFetch(`/reservations/${reservationId}/cancel`, { method: 'POST' });

        // Limpiar datos de sesión para evitar inconsistencias
        sessionStorage.removeItem(`reservation_${reservationId}`);

        showMessage('✅ Reserva cancelada. Butaca liberada.', 'success');

        // Redirigir al mapa después de 1.5s
        setTimeout(() => {
            window.location.href = 'index.html';
        }, 1500);

    } catch (error) {
        console.error('❌ Error cancelando:', error);
        showMessage(`❌ ${error.message}`, 'error');

        // Restaurar botón si falla
        btn.disabled = false;
        btn.textContent = originalText;
    }
}

function showMessage(text, type) {
    const box = document.getElementById('messageBox');
    box.innerHTML = text;
    box.className = `message ${type} show`;
}