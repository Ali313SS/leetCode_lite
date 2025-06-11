async function validateForm(event) {
    event.preventDefault();

    const email = document.getElementById('email').value.trim();
    const password = document.getElementById('password').value.trim();
    const emailError = document.getElementById('emailError');
    const passwordError = document.getElementById('passwordError');
    const successMessage = document.getElementById('successMessage');

    emailError.style.display = 'none';
    passwordError.style.display = 'none';
    successMessage.style.display = 'none';
    emailError.textContent = '';
    passwordError.textContent = '';
    successMessage.textContent = '';

    console.log('Email:', email);
    console.log('Password:', password);

    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (!emailRegex.test(email)) {
        emailError.textContent = 'Please enter a valid email address';
        emailError.style.display = 'block';
        return false;
    }

    const passwordRegex = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$/;
    if (!passwordRegex.test(password)) {
        passwordError.textContent = 'Password must be at least 8 characters long and include a letter and a number';
        passwordError.style.display = 'block';
        return false;
    }

    console.log('Request payload:', JSON.stringify({ email, password }));

    try {
        const response = await fetch('https://ajudge.runasp.net/auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ email, password }),
        });

        console.log('Response status:', response.status);
        console.log('Response headers:', [...response.headers.entries()]);
        const textResponse = await response.text();
        console.log('Raw response body:', textResponse);

        let data;
        const isJwt = textResponse.includes('.') && /^[A-Za-z0-9-_=]+\.[A-Za-z0-9-_=]+\.?[A-Za-z0-9-_.+/=]*$/.test(textResponse);
        if (response.ok && isJwt) {
            console.log('Detected raw JWT response');
            data = { token: textResponse };
        } else {
            try {
                data = JSON.parse(textResponse);
            } catch (jsonError) {
                console.error('Failed to parse JSON:', jsonError);
                emailError.textContent = 'Server returned an invalid response: ' + textResponse.slice(0, 100);
                emailError.style.display = 'block';
                return false;
            }
        }

        console.log('API response:', data);

        if (response.ok) {
            successMessage.textContent = 'Sign-in successful!';
            successMessage.style.display = 'block';

            // --- IMPORTANT: Clear ALL old authentication and user-specific data ---
            localStorage.removeItem('jwtToken');
            localStorage.removeItem('loggedInUsername');
            localStorage.removeItem('loggedInUserEmail');
            localStorage.removeItem('loggedInUserId'); // Clear userId to avoid stale data

            // Store new token if present
            if (data.token) {
                localStorage.setItem('jwtToken', data.token);
                console.log('Stored JWT:', data.token);

                // Decode JWT to get username, email, and userId and store them
                try {
                    const payload = JSON.parse(atob(data.token.split('.')[1]));
                    // Prioritize a specific 'username' claim, then 'name', then 'email', then 'nameid' or 'sub'
                    const usernameForDisplay = payload.username || payload.name || payload.email || payload.nameid || payload.sub;
                    const userEmail = payload.email;
                    const userId = payload.nameid || payload.userId || payload.id || payload.sub; // Get userId from JWT

                    if (usernameForDisplay) {
                        localStorage.setItem('loggedInUsername', usernameForDisplay);
                        console.log('Stored username:', usernameForDisplay);
                    }
                    if (userEmail) {
                        localStorage.setItem('loggedInUserEmail', userEmail);
                        console.log('Stored user email:', userEmail);
                    }
                    if (userId) {
                        localStorage.setItem('loggedInUserId', userId);
                        console.log('Stored userId:', userId);
                    }
                } catch (jwtError) {
                    console.error('Error decoding JWT for user details:', jwtError);
                }
            }
            window.location.href = 'profile.html';
            return true;
        } else {
            emailError.textContent = data.message || 'Invalid email or password.';
            emailError.style.display = 'block';
            return false;
        }
    } catch (error) {
        console.error('Error during sign-in:', error);
        emailError.textContent = 'An error occurred. Please try again.';
        emailError.style.display = 'block';
        return false;
    }
}

function showForgotPasswordModal() {
    const modal = document.getElementById('forgotPasswordModal');
    modal.style.display = 'flex';
    const emailInput = document.getElementById('resetEmail');
    emailInput.value = '';
    emailInput.focus();
    document.getElementById('resetEmailError').style.display = 'none';
    document.getElementById('resetSuccessMessage').style.display = 'none';
}

function closeForgotPasswordModal() {
    const modal = document.getElementById('forgotPasswordModal');
    modal.style.display = 'none';
    document.getElementById('resetPasswordForm').reset();
    document.getElementById('resetEmailError').style.display = 'none';
    document.getElementById('resetSuccessMessage').style.display = 'none';
}

async function validateResetForm(event) {
    event.preventDefault();

    const email = document.getElementById('resetEmail').value.trim();
    const emailError = document.getElementById('resetEmailError');
    const successMessage = document.getElementById('resetSuccessMessage');

    console.log('Email entered:', email);

    emailError.style.display = 'none';
    successMessage.style.display = 'none';
    emailError.textContent = '';
    successMessage.textContent = '';

    if (!email) {
        emailError.textContent = 'Please enter an email address';
        emailError.style.display = 'block';
        return false;
    }

    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (!emailRegex.test(email)) {
        emailError.textContent = 'Please enter a valid email address';
        emailError.style.display = 'block';
        return false;
    }

    try {
        const response = await mockPasswordResetCall(email);

        if (response.status === 'success') {
            successMessage.textContent = 'Password reset link sent to your email!';
            successMessage.style.display = 'block';
            setTimeout(closeForgotPasswordModal, 3000);
            return true;
        } else {
            emailError.textContent = 'Email not found';
            emailError.style.display = 'block';
            return false;
        }
    } catch (error) {
        emailError.textContent = 'An error occurred. Please try again.';
        emailError.style.display = 'block';
        return false;
    }
}

function mockPasswordResetCall(email) {
    return new Promise((resolve, reject) => {
        setTimeout(() => {
            const validEmail = 'user@example.com';
            if (email === validEmail) {
                resolve({ status: 'success', message: 'Reset link sent' });
            } else {
                resolve({ status: 'error', message: 'Email not found' });
            }
        }, 1000);
    });
}