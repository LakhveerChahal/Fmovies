using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Mvc;
using Fmovies.Models;
using System.Security.Claims;

namespace Fmovies.Controllers
{
    public class CartController : Controller
    {
        private int CartIdGen()
        {
            Random random = new Random();
            int num = random.Next();
            return num;
        }
        private ApplicationDbContext _context;
        public CartController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        private string FetchUserId()
        {
            //Getting user ID of the logged in user
            var claimsIdentity = User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                // the principal identity is a claims identity.
                // now we need to find the NameIdentifier claim
                var userIdClaim = claimsIdentity.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

                if (userIdClaim != null)
                {
                    var userIdValue = userIdClaim.Value;
                    return userIdValue;
                }
            }
            return null;
        }
        public void UserCartCheck()
        {
            if (User.Identity.IsAuthenticated)
            {
                HttpCookie cookie = Request.Cookies["CartCookie"];
                if (cookie != null)
                {
                    int cartid = Convert.ToInt32(cookie["CartId"]);
                    List<UnauthenticatedCart> unauthenticatedCarts = new List<UnauthenticatedCart>();
                    unauthenticatedCarts = _context.UnauthenticatedCarts.Where(p => p.cartId == cartid).ToList();
                    AuthenticatedCart authenticatedCart = new AuthenticatedCart();
                    if(unauthenticatedCarts.Count == 0)
                    {
                        return;
                    }
                    authenticatedCart.userId = FetchUserId();
                    foreach(var p in unauthenticatedCarts)
                    {
                        authenticatedCart.movieId = p.movieId;
                        _context.AuthenticatedCarts.Add(authenticatedCart);
                        _context.UnauthenticatedCarts.Remove(p);
                    }
                    _context.SaveChanges();
                }
            }
        }
        public void AddToCart(int selectedId=-1)
        {
            if (selectedId == -1)
                return;
            HttpCookie cookie = Request.Cookies["CartCookie"];
            int cartid;
            if (cookie == null)
            {
                cookie = new HttpCookie("CartCookie");
                cartid = CartIdGen();
                cookie["CartId"] = Convert.ToString(cartid);
                cookie.Expires = DateTime.Now.AddMonths(3);
                Response.Cookies.Add(cookie);
            }
            else
            {
                cartid = Convert.ToInt32(cookie["CartCookie"]);
            }
            if (User.Identity.IsAuthenticated)
            {
                AuthenticatedCart authenticatedCart = new AuthenticatedCart();
                authenticatedCart.movieId = selectedId;
                authenticatedCart.userId = FetchUserId();
                _context.AuthenticatedCarts.Add(authenticatedCart);
            }
            else
            {
                UnauthenticatedCart unauthenticatedCart = new UnauthenticatedCart();
                unauthenticatedCart.cartId = cartid;
                unauthenticatedCart.movieId = selectedId;
                _context.UnauthenticatedCarts.Add(unauthenticatedCart);
            }
            _context.SaveChanges();
        }
    }
}