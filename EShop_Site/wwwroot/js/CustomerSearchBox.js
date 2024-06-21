function redirectToSearch() {
    var inputValue = document.getElementById("searchInput").value;
    var url = 'Customer/Products/ProductsList';
    location.href = url + '?search=' + encodeURIComponent(inputValue);
}