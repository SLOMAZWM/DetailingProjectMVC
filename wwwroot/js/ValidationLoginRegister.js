document.addEventListener('DOMContentLoaded', function () {
    function validateEmail(email) {
        const re = /\S+@\S+\.\S+/;
        return re.test(email);
    }

    function validatePhoneNumber(phone) {
        const re = /^\+\d{1,3}\s?\d{1,3}\s?\d{3}\s?\d{3}\s?\d{3}$/;
        return re.test(phone);
    }

    function validatePassword(password) {
        const re = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d\s:]).*$/;
        return re.test(password) && password.length >= 8 && password.length <= 256;
    }

    document.getElementById('registerForm').addEventListener('submit', function (event) {
        let isValid = true;
        const errors = [];

        const firstName = document.getElementById('FirstName').value.trim();
        const lastName = document.getElementById('LastName').value.trim();
        const email = document.getElementById('Email').value.trim();
        const password = document.getElementById('Password').value.trim();
        const phoneNumber = document.getElementById('PhoneNumber').value.trim();

        if (!firstName) {
            errors.push('Imię jest wymagane.');
            isValid = false;
        }

        if (!lastName) {
            errors.push('Nazwisko jest wymagane.');
            isValid = false;
        }

        if (!email || !validateEmail(email)) {
            errors.push('Nieprawidłowy format email.');
            isValid = false;
        }

        if (!password || !validatePassword(password)) {
            errors.push('Hasło musi mieć od 8 do 256 znaków i zawierać co najmniej jedną dużą literę, jedną małą literę, jedną cyfrę i jeden znak specjalny.');
            isValid = false;
        }

        if (!phoneNumber || !validatePhoneNumber(phoneNumber)) {
            errors.push('Numer telefonu musi być w formacie +00 000 000 000.');
            isValid = false;
        }

        if (!isValid) {
            event.preventDefault();
            const errorContainer = document.getElementById('errorMessages');
            errorContainer.innerHTML = '';
            errors.forEach(error => {
                const errorElement = document.createElement('p');
                errorElement.className = 'text-danger';
                errorElement.textContent = error;
                errorContainer.appendChild(errorElement);
            });
        }
    });

    document.getElementById('loginForm').addEventListener('submit', function (event) {
        let isValid = true;
        const errors = [];

        const email = document.getElementById('LoginEmail').value.trim();
        const password = document.getElementById('LoginPassword').value.trim();

        if (!email || !validateEmail(email)) {
            errors.push('Nieprawidłowy format email.');
            isValid = false;
        }

        if (!password) {
            errors.push('Hasło jest wymagane.');
            isValid = false;
        }

        if (!isValid) {
            event.preventDefault();
            const errorContainer = document.getElementById('errorMessages');
            errorContainer.innerHTML = '';
            errors.forEach(error => {
                const errorElement = document.createElement('p');
                errorElement.className = 'text-danger';
                errorElement.textContent = error;
                errorContainer.appendChild(errorElement);
            });
        }
    });
});
