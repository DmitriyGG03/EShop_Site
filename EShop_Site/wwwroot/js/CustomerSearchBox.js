function redirectToSearch() {
    var inputValue = document.getElementById("searchInput").value;
    var url = 'Customer/Products/ProductsListView';
    location.href = url + '?search=' + encodeURIComponent(inputValue);
}