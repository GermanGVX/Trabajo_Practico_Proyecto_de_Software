const API_URL = 'https://localhost:7129/api'; 

function switchTab(tab) {
    document.querySelectorAll('.tab').forEach(t => t.classList.remove('active'));
    document.querySelectorAll('.tab')[tab === 'login' ? 0 : 1].classList.add('active');

    document.getElementById('loginForm').classList.toggle('hidden', tab !== 'login');
    document.getElementById('registerForm').classList.toggle('hidden', tab !== 'register');
    hideMessage();
}

function showMessage(text, type) {
    const box = document.getElementById('messageBox');
    box.textContent = text;
    box.className = `message ${type}`;
}

function hideMessage() {
    document.getElementById('messageBox').className = 'message';
}

// --- LOGIN ---
document.getElementById('loginForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const btn = document.getElementById('loginBtn');
    btn.disabled = true;
    btn.textContent = 'Verificando...';

    try {
        const res = await fetch(`${API_URL}/users/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                email: document.getElementById('loginEmail').value,
                password: document.getElementById('loginPassword').value
            })
        });

        const data = await res.json();
        if (res.ok) {
            
            localStorage.setItem('userId', data.userId);
            showMessage('¡Bienvenido! Redirigiendo...', 'success');
            setTimeout(() => window.location.href = 'index.html', 1000);
        } else {
            showMessage(data.error || 'Error al iniciar sesión', 'error');
        }
    } catch (err) {
        showMessage('Error de conexión con el servidor', 'error');
    } finally {
        btn.disabled = false;
        btn.textContent = 'Entrar';
    }
});



// --- REGISTRO ---
document.getElementById('registerForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const btn = document.getElementById('regBtn');
    btn.disabled = true;
    btn.textContent = 'Creando cuenta...';

    try {
        const res = await fetch(`${API_URL}/users`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                name: document.getElementById('regName').value,
                email: document.getElementById('regEmail').value,
                password: document.getElementById('regPassword').value
            })
        });

        if (res.ok) {
            showMessage('¡Cuenta creada! Ahora podés iniciar sesión.', 'success');
            setTimeout(() => switchTab('login'), 1500);
        } else {
            const err = await res.json();
            showMessage(err.error || 'Error al registrar', 'error');
        }
    } catch (err) {
        showMessage('Error de conexión con el servidor', 'error');
    } finally {
        btn.disabled = false;
        btn.textContent = 'Crear cuenta';
    }
});

