document.addEventListener("DOMContentLoaded", function () {
    var modal = document.getElementById("errorModal");
    var span = document.getElementsByClassName("modal-close-cross")[0];
    var closeBtn = document.getElementsByClassName("modal-close-btn")[0];

    // Function to open the modal
    function openModal() {
        modal.style.display = "flex";
    }

    // Function to close the modal
    function closeModal() {
        modal.style.display = "none";
    }

    // When the user clicks on <span> (x) or the close button, close the modal
    span.onclick = closeModal;
    closeBtn.onclick = closeModal;

    // When the user clicks anywhere outside of the modal, close it
    window.onclick = function(event) {
        if (event.target == modal) {
            closeModal();
        }
    }

    // If there are errors in the ModelState, open the modal
    var errorMessages = document.getElementById("errorMessages").children.length;
    if (errorMessages > 0) {
        openModal();
    }
});