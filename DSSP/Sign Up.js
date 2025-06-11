async function validateForm(event) {
    event.preventDefault();

    const username = document.getElementById('username').value.trim();
    const password = document.getElementById('password').value;
    const email = document.getElementById('email').value.trim();
    const nickname = document.getElementById('nickname').value.trim();
    const school = document.getElementById('school').value.trim();

    const usernameError = document.getElementById('usernameError');
    const passwordError = document.getElementById('passwordError');
    const emailError = document.getElementById('emailError');
    const nicknameError = document.getElementById('nicknameError');
    const schoolError = document.getElementById('schoolError');
    const successMessage = document.getElementById('successMessage');

    [usernameError, passwordError, emailError, nicknameError, schoolError].forEach(error => {
        error.style.display = 'none';
        error.textContent = '';
    });
    successMessage.innerHTML = '';

    // Client-side validation
    const usernameRegex = /^[a-zA-Z0-9]{3,20}$/;
    if (!usernameRegex.test(username)) {
        usernameError.textContent = 'Username must be 3-20 characters long and alphanumeric';
        usernameError.style.display = 'block';
        return false;
    }

    const passwordRegex = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$/;
    if (!passwordRegex.test(password)) {
        passwordError.textContent = 'Password must be at least 8 characters long and include a letter and a number';
        passwordError.style.display = 'block';
        return false;
    }

    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (!emailRegex.test(email)) {
        emailError.textContent = 'Please enter a valid email address';
        emailError.style.display = 'block';
        return false;
    }

    if (nickname.length < 2 || nickname.length > 30) {
        nicknameError.textContent = 'Nickname must be 2-30 characters long';
        nicknameError.style.display = 'block';
        return false;
    }

    if (school.length < 2 || school.length > 100) {
        schoolError.textContent = 'School name must be 2-100 characters long';
        schoolError.style.display = 'block';
        return false;
    }

    try {
        const response = await registerUser({ username, email, password });

        if (response.ok) {
            const responseText = await response.text();
            successMessage.innerHTML = `
                <div class="modern-success-message">
                    <button class="close-btn" onclick="this.parentElement.style.display='none'">×</button>
                    <div class="icon-wrapper">
                        <svg stroke-linejoin="round" stroke-linecap="round" stroke-width="2" stroke="currentColor" fill="none" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg" class="success-icon">
                            <path d="M9 12l2 2 4-4"></path>
                            <circle r="10" cy="12" cx="12"></circle>
                        </svg>
                    </div>
                    <div class="text-wrapper">
                        <div class="title">Success</div>
                        <div class="message">${responseText}</div>
                    </div>
                </div>
            `;
            return true;
        } else {
            const errorText = await response.text();
            let errorMessage = 'An unknown error occurred during sign-up.';
            if (response.status === 400) {
                if (errorText.toLowerCase().includes('email already exists')) {
                    errorMessage = 'An error occurred. Please try again.';
                    emailError.textContent = errorMessage;
                    emailError.style.display = 'block';
                } else if (errorText.toLowerCase().includes('username')) {
                    errorMessage = 'This username is already taken. Please choose another.';
                    usernameError.textContent = errorMessage;
                    usernameError.style.display = 'block';
                } else {
                    errorMessage = errorText || 'Invalid input. Please check your details.';
                    emailError.textContent = errorMessage;
                    emailError.style.display = 'block';
                }
            } else if (response.status === 429) {
                errorMessage = 'Too many requests. Please try again later.';
                emailError.textContent = errorMessage;
                emailError.style.display = 'block';
            } else if (response.status >= 500) {
                errorMessage = 'Server error. Please try again later or contact support.';
                emailError.textContent = errorMessage;
                emailError.style.display = 'block';
            } else {
                emailError.textContent = errorText || errorMessage;
                emailError.style.display = 'block';
            }
            return false;
        }
    } catch (error) {
        console.error('Registration error:', error);
        let errorMessage = 'An unexpected error occurred. Please try again.';
        if (error.name === 'TypeError' && error.message.includes('Failed to fetch')) {
            errorMessage = 'Unable to reach the server at https://ajudge.runasp.net. Please check your network or server status.';
        } else if (error.message.toLowerCase().includes('cors')) {
            errorMessage = 'CORS error. The server is blocking requests from this origin. Contact the server administrator.';
        } else if (error.name === 'AbortError') {
            errorMessage = 'Request timed out. The server took too long to respond.';
        }
        emailError.textContent = errorMessage;
        emailError.style.display = 'block';
        return false;
    }
}

async function registerUser(userData) {
    const apiUrl = 'https://ajudge.runasp.net/auth/register';

    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), 10000);

    try {
        const response = await fetch(apiUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(userData),
            signal: controller.signal
        });
        clearTimeout(timeoutId);
        return response;
    } catch (error) {
        clearTimeout(timeoutId);
        throw error;
    }
}