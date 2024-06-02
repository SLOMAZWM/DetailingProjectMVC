document.addEventListener('DOMContentLoaded', function () {
    var cartIcon = document.querySelector('.CartIcon');
    var cartSummary = document.getElementById('cart-summary');
    var closeCartButton = document.getElementById('close-cart');

    cartIcon.addEventListener('click', function () {
        cartSummary.classList.toggle('visible');
    });

    closeCartButton.addEventListener('click', function () {
        cartSummary.classList.remove('visible');
    });
});
