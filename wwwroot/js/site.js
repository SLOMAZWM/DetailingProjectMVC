// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {
    var backToTopBtn = document.getElementById("backToTopBtn");

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
});

