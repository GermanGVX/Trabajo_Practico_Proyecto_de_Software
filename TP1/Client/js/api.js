const API_BASE = window.API_BASE_URL || "https://localhost:7129/api";

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
async function apiFetch(endpoint, options = {}) {
    const url = `${API_BASE}${endpoint}`;
    const response = await fetch(url, {
        headers: { 'Content-Type': 'application/json', ...options.headers },
        ...options
    });

   
    const contentType = response.headers.get('content-type');
    if (contentType && contentType.includes('text/html')) {
        const html = await response.text();
        throw new Error(`El servidor devolvió HTML en lugar de JSON. Revisa la ruta: ${endpoint}`);
    }

    const data = await response.json();
    if (!response.ok) {
        throw new Error(data.error || `Error ${response.status}`);
    }
    return data;
}