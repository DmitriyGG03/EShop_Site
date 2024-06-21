document.addEventListener("DOMContentLoaded", function () {
    let confirmButtons = document.getElementsByClassName("modal-confirm-button-2");
    
    for (let i = 0; i < confirmButtons.length; i++) {
        confirmButtons[i].onclick = function () {
            let confirmMessage = confirmButtons[i].getAttribute("data-confirm-message");
            let link = confirmButtons[i].getAttribute("data-link");
            let productId = confirmButtons[i].getAttribute("data-product-id");
            
            openModal(confirmMessage, link, productId);
        };
    }

    function openModal(confirmMessage, link, productId) {
        document.getElementById("message-field").innerText = confirmMessage;
        document.getElementById("accept-btn").onclick = function () {
            window.location.href = `${link}?id=${productId}`;
        };
        
        let modal = document.getElementById("confirmModal");
        modal.style.display = "flex";
    }
    
    let span = document.getElementById("crossCloseConfirm");
    let rejectBtn = document.getElementById("reject-btn");

    span.onclick = closeModal;
    rejectBtn.onclick = closeModal;

    window.onclick = function (event) {
        if (event.target == modal) {
            closeModal();
        }
    };

    function closeModal() {
        let modal = document.getElementById("confirmModal");
        modal.style.display = "none";
    }
});