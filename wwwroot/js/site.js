document.addEventListener("DOMContentLoaded", function () {
    var backToTopBtn = document.getElementById("backToTopBtn");

    if (backToTopBtn) {
        window.onscroll = function () {
            scrollFunction();
        };

        function scrollFunction() {
            if (document.body.scrollTop > 300 || document.documentElement.scrollTop > 1800) {
                backToTopBtn.style.display = "block";
                backToTopBtn.classList.add("show");
            } else {
                backToTopBtn.classList.remove("show");
                backToTopBtn.style.display = "none";
            }
        }

        function backToTop() {
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }

        backToTopBtn.onclick = function () {
            backToTop();
        };
    }
});

document.addEventListener('DOMContentLoaded', function () {
    var images = [
        '/Pictures/Audi.png',
        '/Pictures/Subaru.png',
        '/Pictures/Nissan.png'
    ];
    var currentIndex = 0;

    var carImage1 = document.getElementById('carImage1');
    var carImage2 = document.getElementById('carImage2');
    var carImage3 = document.getElementById('carImage3');

    if (carImage1 && carImage2 && carImage3) {
        var carImages = [carImage1, carImage2, carImage3];

        carImages.forEach(function (carImage) {
            carImage.addEventListener('click', function () {
                fadeImages();
            });
        });

        function fadeImages() {
            var nextIndex = (currentIndex + 1) % images.length;
            var currentImage = carImages[currentIndex % 2];
            var nextImage = carImages[(currentIndex + 2) % 1];

            nextImage.src = images[nextIndex];
            currentImage.classList.add('hidden');
            nextImage.classList.remove('hidden');

            currentIndex = nextIndex;
        }
    }
});

// Obsługa koszyka
document.addEventListener('DOMContentLoaded', function () {
    var cartIcon = document.querySelector('.CartIcon');
    var cartSummary = document.getElementById('cart-summary');

    function toggleCartSummary() {
        cartSummary.classList.toggle('visible');
    }

    if (cartIcon) {
        cartIcon.addEventListener('click', toggleCartSummary);
    }

    var buttons = document.querySelectorAll('.AddToCartButton');
    buttons.forEach(function (button) {
        button.addEventListener('click', function () {
            var productId = this.getAttribute('data-product-id');
            var form = this.closest('form');
            var tokenValue = form.querySelector('input[name="__RequestVerificationToken"]').value;

            fetch(`/Cart/AddToCart?id=${productId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': tokenValue
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
                if (cartSummary) {
                    cartSummary.innerHTML = html;
                }
            });
    }
});
