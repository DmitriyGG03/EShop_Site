function redirectToSearch() {
    var inputValue = document.getElementById("searchInput").value;
    var url = 'Seller/Products/ProductsList';
    location.href = url + '?search=' + encodeURIComponent(inputValue);
}