document.addEventListener('DOMContentLoaded', function () {
    var cartIcon = document.querySelector('.CardShop');
    var cartSummary = document.getElementById('cart-summary');

    cartIcon.addEventListener('click', function () {
        cartSummary.classList.toggle('visible');
    });

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
                document.getElementById('cart-summary').innerHTML = html;
            });
    }
});
