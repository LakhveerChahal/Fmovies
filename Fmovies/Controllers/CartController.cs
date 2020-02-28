using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Mvc;
using Fmovies.Models;
using Fmovies.ViewModels;
using System.Security.Claims;

namespace Fmovies.Controllers
{
    public class CartController : Controller
    {
        private int CartIdGen()
        {
            Random random = new Random();
            int num = random.Next(1000, int.MaxValue);
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
        public ActionResult AddToCart(int selectedId = -1)
        {
            if (selectedId == -1)
                return new HttpNotFoundResult();
            bool mAdded = false;
            HttpCookie cookie = new HttpCookie("CartCookie");
            cookie = Request.Cookies["CartCookie"];
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
                cartid = Convert.ToInt32(cookie["CartId"]);
            }
            if (User.Identity.IsAuthenticated)
            {
                AuthenticatedCart authenticatedCart = new AuthenticatedCart();
                string userid = FetchUserId();
                authenticatedCart = _context.AuthenticatedCarts.SingleOrDefault(p => p.userId == userid && p.movieId == selectedId);
                if (authenticatedCart != null)
                {
                    _context.AuthenticatedCarts.Remove(authenticatedCart);
                    mAdded = false;
                }
                else
                {
                    authenticatedCart = new AuthenticatedCart();
                    authenticatedCart.movieId = selectedId;
                    authenticatedCart.userId = userid;
                    _context.AuthenticatedCarts.Add(authenticatedCart);
                    mAdded = true;
                }
            }
            else
            {
                UnauthenticatedCart unauthenticatedCart = new UnauthenticatedCart();
                unauthenticatedCart = _context.UnauthenticatedCarts.SingleOrDefault(p => p.cartId == cartid && p.movieId == selectedId);
                if (unauthenticatedCart != null)
                {
                    _context.UnauthenticatedCarts.Remove(unauthenticatedCart);
                    mAdded = false;
                }
                else
                {
                    unauthenticatedCart = new UnauthenticatedCart();
                    unauthenticatedCart.cartId = cartid;
                    unauthenticatedCart.movieId = selectedId;
                    _context.UnauthenticatedCarts.Add(unauthenticatedCart);
                    mAdded = true;
                }
            }
            _context.SaveChanges();
            CartButtonViewModel cartButtonViewModel = new CartButtonViewModel()
            {
                movieAdded = mAdded,
                MovieId = selectedId
            };
            return PartialView("~/Views/Movie/_CartButtonPartial.cshtml", cartButtonViewModel);
        }
    }
}