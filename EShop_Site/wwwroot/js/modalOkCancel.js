document.addEventListener("DOMContentLoaded", function () {
    let confirmButtons = document.getElementsByClassName("modal-confirm-button");
    let modal = document.getElementById("confirmModal");
    let span = document.getElementById("crossCloseConfirm");
    let acceptBtn = document.getElementById("accept-btn");
    let rejectBtn = document.getElementById("reject-btn");
    let link = document.getElementById("link").value;
    
    function openModal() {
        modal.style.display = "flex";
    }
    function closeModal() {
        modal.style.display = "none";
    }
    
    function redirect() {
        if (link && link.trim() !== "") {
            window.location.href = link;
        }
    }
    
    span.onclick = closeModal;
    acceptBtn.onclick = redirect;
    rejectBtn.onclick = closeModal;
    
    window.onclick = function (event) {
        if (event.target == modal) {
            closeModal();
        }
    }

    for (let i = 0; i < confirmButtons.length; i++) {
        confirmButtons[i].onclick = openModal;
    }
});