document.addEventListener("DOMContentLoaded", function () {
    const contactForm = document.getElementById("contactForm");
    const successMessage = document.getElementById("successMessage");

    contactForm.addEventListener("submit", function (event) {
        event.preventDefault(); // Prevent form from refreshing the page

        // Show alert box
        alert("Submitted successfully!");

        // Show success message below form
        successMessage.style.display = "block";

        // Clear form fields
        contactForm.reset();

        // Hide success message after 3 seconds
        setTimeout(function () {
            successMessage.style.display = "none";
        }, 3000);
    });
});

