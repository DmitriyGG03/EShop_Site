function redirectToSearch() {
    var inputValue = document.getElementById("searchInput").value;
    var url = 'Seller/Products/ProductsListView';
    location.href = url + '?search=' + encodeURIComponent(inputValue);
}