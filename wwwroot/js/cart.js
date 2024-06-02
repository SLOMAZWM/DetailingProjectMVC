document.addEventListener('DOMContentLoaded', function () {
    var cartIcon = document.querySelector('.CartIcon');
    var cartSummary = document.getElementById('cart-summary');

    function toggleCartSummary() {
        cartSummary.classList.toggle('visible');
    }

    cartIcon.addEventListener('click', toggleCartSummary);

    var buttons = document.querySelectorAll('.AddToCartButton');
    buttons.forEach(function (button) {
        button.addEventListener('click', function () {
            var productId = this.getAttribute('data-product-id');
            fetch(`/Cart/AddToCart?id=${productId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                },
                body: JSON.stringify({ id: productId })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        alert('Produkt został dodany do koszyka.');
                        updateCartSummary();
                    } else {
                        alert('Wystąpił błąd przy dodawaniu produktu do koszyka.');
                    }
                });
        });
    });

    function updateCartSummary() {
        fetch('/Cart/Summary')
            .then(response => response.text())
            .then(html => {
                cartSummary.innerHTML = html;
                // Reassign event listener for the cart icon to ensure it still works after updating the summary
                cartIcon.addEventListener('click', toggleCartSummary);
            });
    }
});
