document.addEventListener("DOMContentLoaded", function () {
    // Back to top button
    var backToTopBtn = document.getElementById("backToTopBtn");

    if (backToTopBtn) {
        window.onscroll = function () {
            scrollFunction();
        };

        function scrollFunction() {
            if (document.body.scrollTop > 300 || document.documentElement.scrollTop > 3600) {
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

    // Image gallery
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
            var currentImage = carImages[currentIndex % carImages.length];
            var nextImage = carImages[nextIndex % carImages.length];

            nextImage.src = images[nextIndex];
            currentImage.classList.add('hidden');
            nextImage.classList.remove('hidden');

            currentIndex = nextIndex;
        }
    }

    // Cart handling
    var cartIcon = document.querySelector('.CartIcon');

    if (cartIcon) {
        cartIcon.addEventListener('click', function () {
            window.location.href = '/Cart/Summary';
        });
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
                var cartSummary = document.getElementById('cart-summary');
                if (cartSummary) {
                    cartSummary.innerHTML = html;
                }
            });
    }
});
