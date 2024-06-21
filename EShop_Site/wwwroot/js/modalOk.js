document.addEventListener("DOMContentLoaded", function () {
    let modal = document.getElementById("errorModal");
    let span = document.getElementById("crossClose");
    let closeBtn = document.getElementById("ok-close");
    let errorMessage = document.getElementById("errorMessage").value;
    
    function openModal() {
        modal.style.display = "flex";
    }
    function closeModal() {
        modal.style.display = "none";
    }
    
    span.onclick = closeModal;
    closeBtn.onclick = closeModal;
    
    window.onclick = function (event) {
        if (event.target == modal) {
            closeModal();
        }
    }

    if (errorMessage && errorMessage.trim() !== "") {
        openModal();
    }
});