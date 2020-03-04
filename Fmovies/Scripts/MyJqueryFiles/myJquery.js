function UpdateCartCount() {
    var count = document.getElementById("movieCount").innerText;
    console.log("Hello");
    var cart = document.getElementById("cartElement");
    cart.innerHTML = '<a id="cartElement" href="~/Cart/CartSummary" class="nav-link"><span class="fa fa-shopping-cart"></span> Cart '+count+' </a>';
}
function UpdateCartIndex() {
    var cartCount = getCookie("CartCount");
    console.log(cartCount);
    var cart = document.getElementById("cartElement");
    cart.innerHTML = '<a id="cartElement" href="~/Cart/CartSummary" class="nav-link"><span class="fa fa-shopping-cart"></span> Cart ' + cartCount + ' </a>';

}
function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            var value = c.substring(name.length, c.length);
            return value.substring("Count=".length, value.length);
        }
    }
    return "";
}