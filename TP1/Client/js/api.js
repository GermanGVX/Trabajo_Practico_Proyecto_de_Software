const API_BASE = window.API_BASE_URL || "https://localhost:7129/api";


//ux

window.showToast = function (mensaje, tipo = 'error') {
    const toast = document.createElement('div');
    toast.textContent = mensaje;

    let colorFondo = '#ef4444';
    if (tipo === 'success') colorFondo = '#22c55e';
    if (tipo === 'info') colorFondo = '#3b82f6';

    Object.assign(toast.style, {
        position: 'fixed',
        bottom: '20px',
        right: '20px',
        padding: '12px 24px',
        backgroundColor: colorFondo,
        color: 'white',
        borderRadius: '8px',
        boxShadow: '0 4px 6px rgba(0,0,0,0.1)',
        zIndex: '9999',
        transition: 'opacity 0.3s ease-in-out',
        fontWeight: 'bold'
    });

    document.body.appendChild(toast);

    setTimeout(() => {
        toast.style.opacity = '0';
        setTimeout(() => toast.remove(), 300);
    }, 3000);
};

// Manejo de estado Loading y prevención de doble submit
window.toggleButtonLoading = function (buttonId, isLoading, loadingText = '⏳ Procesando...') {
    const button = document.getElementById(buttonId);
    if (!button) return;

    if (isLoading) {

        if (!button.dataset.originalText) {
            button.dataset.originalText = button.innerHTML;
        }
        button.innerHTML = loadingText;
        button.disabled = true;
        button.style.opacity = '0.7';
        button.style.cursor = 'not-allowed';
    } else {
        button.innerHTML = button.dataset.originalText || 'Aceptar';
        button.disabled = false;
        button.style.opacity = '1';
        button.style.cursor = 'pointer';
    }
};



// Verificar autenticación
function checkAuth() {
    const userId = localStorage.getItem('userId');
    if (!userId) {
        window.location.href = 'auth.html';
        return null;
    }
    return userId;
}

// Obtener nombre del usuario (simplificado)
function getUserName() {
    return localStorage.getItem('userName') || 'Usuario';
}

// Cerrar sesión
function logout() {
    localStorage.removeItem('userId');
    localStorage.removeItem('userName');
    window.location.href = 'auth.html';
}

// Actualizar UI con datos del usuario
function updateUserInfo() {
    const userNameElement = document.getElementById('userName');
    if (userNameElement) {
        userNameElement.textContent = getUserName();
    }
}

// Fetch con manejo de errores
async function apiFetch(endpoint, options = {}, retries = 1) {
    const url = `${API_BASE}${endpoint}`;

    try {
        const response = await fetch(url, {
            headers: { 'Content-Type': 'application/json', ...options.headers },
            ...options
        });

        const contentType = response.headers.get('content-type');
        if (contentType && contentType.includes('text/html')) {
            throw new Error(`Error de ruta (HTML recibido): ${endpoint}`);
        }

        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.error || `Error ${response.status}`);
        }

        return data;

    } catch (error) {

        if (retries > 0) {
            console.warn(`Reintentando conexión a ${endpoint}... Intentos restantes: ${retries}`);
            await new Promise(resolve => setTimeout(resolve, 1000)); 
            return await apiFetch(endpoint, options, retries - 1);
        }

        throw error;
    }
}