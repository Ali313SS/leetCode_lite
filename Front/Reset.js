let credentialsStore = {
    email: 'user@example.com',
    password: 'Password123'
};

async function validateResetPassword(event) {
    event.preventDefault();
    console.log('Form submitted'); // Debug: Confirm form submission

    const urlParams = new URLSearchParams(window.location.search);
    const token = urlParams.get('token');
    const email = urlParams.get('email');
    const newPassword = document.getElementById('newPassword').value.trim();
    const confirmPassword = document.getElementById('confirmPassword').value.trim();
    const newPasswordError = document.getElementById('newPasswordError');
    const confirmPasswordError = document.getElementById('confirmPasswordError');
    const successMessage = document.getElementById('successMessage');

    console.log('Reset attempt:', { email, token, newPassword }); // Debug: Log inputs

    // Reset previous messages
    newPasswordError.style.display = 'none';
    confirmPasswordError.style.display = 'none';
    successMessage.style.display = 'none';
    newPasswordError.textContent = '';
    confirmPasswordError.textContent = '';
    successMessage.textContent = '';

    // Password validation: at least 8 chars, 1 letter, 1 number
    const passwordRegex = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$/;
    if (!newPassword) {
        newPasswordError.textContent = 'Please enter a new password';
        newPasswordError.style.display = 'block';
        console.log('Error: Empty new password'); // Debug
        return false;
    }
    if (!passwordRegex.test(newPassword)) {
        newPasswordError.textContent = 'Password must be at least 8 characters long and include a letter and a number';
        newPasswordError.style.display = 'block';
        console.log('Error: Invalid password format'); // Debug
        return false;
    }

    // Confirm password match
    if (!confirmPassword) {
        confirmPasswordError.textContent = 'Please confirm your password';
        confirmPasswordError.style.display = 'block';
        console.log('Error: Empty confirm password'); // Debug
        return false;
    }
    if (newPassword !== confirmPassword) {
        confirmPasswordError.textContent = 'Passwords do not match';
        confirmPasswordError.style.display = 'block';
        console.log('Error: Passwords do not match'); // Debug
        return false;
    }

    // Simulate backend API call for password reset
    try {
        const response = await mockResetPasswordCall({ email, token, newPassword });
        console.log('API response:', response); // Debug: Log API response
        if (response.status === 'success') {
            // Update stored credentials
            credentialsStore.password = newPassword;
            console.log('Updated credentials:', credentialsStore); // Debug
            successMessage.textContent = 'Password reset successful! Redirecting to sign-in...';
            successMessage.style.display = 'block';
            console.log('Success message displayed'); // Debug
            setTimeout(() => {
                window.location.href = 'Sign In.html';
            }, 3000);
            return true;
        } else {
            newPasswordError.textContent = response.message || 'Invalid or expired reset link';
            newPasswordError.style.display = 'block';
            console.log('Error: API returned failure'); // Debug
            return false;
        }
    } catch (error) {
        console.error('Reset error:', error);
        newPasswordError.textContent = 'An error occurred. Please try again.';
        newPasswordError.style.display = 'block';
        console.log('Error: Exception caught'); // Debug
        return false;
    }
}

function mockResetPasswordCall({ email, token, newPassword }) {
    return new Promise((resolve) => {
        setTimeout(() => {
            // Simplified for testing: Always return success
            resolve({ status: 'success', message: 'Password updated' });
        }, 1000);
    });
}