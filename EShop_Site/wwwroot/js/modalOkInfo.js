document.addEventListener("DOMContentLoaded", function () {
    let modal = document.getElementById("infoModal");
    let span = document.getElementById("crossCloseInfo");
    let closeBtn = document.getElementById("ok-close-info");
    let errorMessage = document.getElementById("infoMessage").value;
    
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