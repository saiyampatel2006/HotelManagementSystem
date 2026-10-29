/**
 * The Aurelia Grand Hotel & Suites - Frontend Scripts
 */

document.addEventListener('DOMContentLoaded', function () {
    // 1. Initialize all Bootstrap Toasts
    const toastElList = [].slice.call(document.querySelectorAll('.toast'));
    toastElList.map(function (toastEl) {
        const toast = new bootstrap.Toast(toastEl, { delay: 5500 });
        toast.show();
        return toast;
    });

    // 2. Dynamic Booking Form Calculator
    const checkInInput = document.getElementById('bookingCheckIn');
    const checkOutInput = document.getElementById('bookingCheckOut');
    const pricePerNightEl = document.getElementById('bookingNightlyRate');
    const displayNightsEl = document.getElementById('displayNights');
    const displaySubtotalEl = document.getElementById('displaySubtotal');
    const displayTaxEl = document.getElementById('displayTax');
    const displayTotalEl = document.getElementById('displayTotal');

    if (checkInInput && checkOutInput) {
        // Enforce today as minimum check-in
        const todayStr = new Date().toISOString().split('T')[0];
        if (!checkInInput.min) {
            checkInInput.min = todayStr;
        }

        function recalculateBooking() {
            if (!checkInInput.value) return;

            const cin = new Date(checkInInput.value);
            // Minimum checkout is checkin + 1 day
            const minCout = new Date(cin);
            minCout.setDate(minCout.getDate() + 1);
            const minCoutStr = minCout.toISOString().split('T')[0];
            checkOutInput.min = minCoutStr;

            if (!checkOutInput.value || new Date(checkOutInput.value) <= cin) {
                checkOutInput.value = minCoutStr;
            }

            const cout = new Date(checkOutInput.value);
            const diffTime = Math.abs(cout - cin);
            const nights = Math.max(1, Math.ceil(diffTime / (1000 * 60 * 60 * 24)));

            if (pricePerNightEl && displayTotalEl) {
                const rate = parseFloat(pricePerNightEl.value || pricePerNightEl.dataset.rate || 0);
                const subtotal = rate * nights;
                const tax = subtotal * 0.12;
                const total = subtotal + tax;

                if (displayNightsEl) displayNightsEl.textContent = nights;
                if (displaySubtotalEl) displaySubtotalEl.textContent = '$' + subtotal.toFixed(2);
                if (displayTaxEl) displayTaxEl.textContent = '$' + tax.toFixed(2);
                if (displayTotalEl) displayTotalEl.textContent = '$' + total.toFixed(2);
            }
        }

        checkInInput.addEventListener('change', recalculateBooking);
        checkOutInput.addEventListener('change', recalculateBooking);
        recalculateBooking();
    }
});

// Helper: Quick-fill demo credentials on login page for effortless evaluation
function fillCredentials(email, password) {
    const emailField = document.getElementById('loginEmail');
    const passField = document.getElementById('loginPassword');
    if (emailField && passField) {
        emailField.value = email;
        passField.value = password;
        emailField.classList.add('is-valid');
        passField.classList.add('is-valid');
    }
}

