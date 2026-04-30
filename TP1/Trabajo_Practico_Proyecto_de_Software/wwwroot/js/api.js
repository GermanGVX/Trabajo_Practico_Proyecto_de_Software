const API_BASE = 'https://localhost:7129/api';

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
    const defaultOptions = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    try {
        const response = await fetch(`${API_BASE}${endpoint}`, {
            ...defaultOptions,
            ...options,
            headers: {
                ...defaultOptions.headers,
                ...options.headers,
            },
        });

        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.error || `Error ${response.status}`);
        }

        return data;
    } catch (error) {
        console.error('API Error:', error);
        throw error;
    }
}