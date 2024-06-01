function validateForm() {
    let isValid = true;
    const errorMessages = {
        required: 'To pole jest wymagane.',
        email: 'Nieprawidłowy format email.',
        phoneNumber: 'Numer telefonu musi być w formacie +00 000 000 000.',
        year: 'Rok jest wymagany.',
        mileage: 'Przebieg jest wymagany.',
        vin: 'VIN jest wymagany.',
        executionDate: 'Data realizacji jest wymagana.',
        selectedService: 'Wybór usługi jest wymagany.',
        materials: 'Materiały są wymagane.',
        clientRemarks: 'Uwagi dla klienta są wymagane.'
    };

    // Clear previous errors
    document.querySelectorAll('.text-danger').forEach(el => el.textContent = '');

    // Client validation rules
    const fields = [
        { id: 'FirstName', errorId: 'FirstNameError', rule: 'required' },
        { id: 'LastName', errorId: 'LastNameError', rule: 'required' },
        { id: 'Email', errorId: 'EmailError', rule: 'email' },
        { id: 'PhoneNumber', errorId: 'PhoneNumberError', rule: 'phoneNumber' },
        { id: 'Brand', errorId: 'BrandError', rule: 'required' },
        { id: 'Model', errorId: 'ModelError', rule: 'required' },
        { id: 'Year', errorId: 'YearError', rule: 'year' },
        { id: 'Color', errorId: 'ColorError', rule: 'required' },
        { id: 'VIN', errorId: 'VINError', rule: 'vin' },
        { id: 'Mileage', errorId: 'MileageError', rule: 'mileage' },
        { id: 'ExecutionDate', errorId: 'ExecutionDateError', rule: 'executionDate' },
        { id: 'SelectedService', errorId: 'SelectedServiceError', rule: 'selectedService' },
        { id: 'Materials', errorId: 'MaterialsError', rule: 'materials' },
        { id: 'ClientRemarks', errorId: 'ClientRemarksError', rule: 'clientRemarks' }
    ];

    fields.forEach(field => {
        const value = document.getElementById(field.id).value.trim();
        if (!value) {
            isValid = false;
            document.getElementById(field.errorId).textContent = errorMessages[field.rule];
        } else if (field.rule === 'email' && !validateEmail(value)) {
            isValid = false;
            document.getElementById(field.errorId).textContent = errorMessages.email;
        } else if (field.rule === 'phoneNumber' && !/^\+\d{1,3}\s?\d{1,3}\s?\d{3}\s?\d{3}\s?\d{3}$/.test(value)) {
            isValid = false;
            document.getElementById(field.errorId).textContent = errorMessages.phoneNumber;
        }
    });

    return isValid;
}

function validateEmail(email) {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
}
