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
        private MoviesViewModel GetViewModel()
        {
            MoviesViewModel moviesViewModel = new MoviesViewModel();
            moviesViewModel.totalPrice = 0;
            List<Movie> movies = new List<Movie>();
            List<Genre> genres = new List<Genre>();
            movies = _context.Movies.ToList();
            genres = _context.Genres.ToList();
            if (User.Identity.IsAuthenticated)
            {
                string userid = FetchUserId();
                List<AuthenticatedCart> authenticatedCarts = new List<AuthenticatedCart>();
                authenticatedCarts = _context.AuthenticatedCarts.Where(p => p.userId == userid).ToList();
                moviesViewModel.movies = movies.Where(m => authenticatedCarts.Any(a => a.movieId == m.movieId)).ToList();
                moviesViewModel.genres = genres.Where(g => moviesViewModel.movies.Any(m => m.genreId == g.genreId)).ToList();
                foreach (var m in moviesViewModel.movies)
                {
                    moviesViewModel.totalPrice += m.moviePrice;
                }
            }
            else
            {
                HttpCookie cookie = Request.Cookies["CartCookie"];
                if (cookie != null)
                {
                    int cartid = Convert.ToInt32(cookie["CartId"]);
                    List<UnauthenticatedCart> unauthenticatedCarts = new List<UnauthenticatedCart>();
                    unauthenticatedCarts = _context.UnauthenticatedCarts.Where(p => p.cartId == cartid).ToList();
                    moviesViewModel.movies = movies.Where(m => unauthenticatedCarts.Any(a => a.movieId == m.movieId)).ToList();
                    moviesViewModel.genres = genres.Where(g => moviesViewModel.movies.Any(m => m.genreId == g.genreId)).ToList();
                    foreach (var m in moviesViewModel.movies)
                    {
                        moviesViewModel.totalPrice += m.moviePrice;
                    }
                }
            }
            return moviesViewModel;
        }
        public ActionResult AddToCart(int selectedId = -1)
        {
            if (selectedId == -1)
                return new HttpNotFoundResult();
            bool mAdded = false;
            int CartCount = 0;
            if (User.Identity.IsAuthenticated)
            {
                AuthenticatedCart authenticatedCart = new AuthenticatedCart();
                string userid = FetchUserId();
                authenticatedCart = _context.AuthenticatedCarts.SingleOrDefault(p => p.userId == userid && p.movieId == selectedId);
                CartCount = _context.AuthenticatedCarts.Where(p => p.userId == userid).ToList().Count;
                if (authenticatedCart != null)
                {
                    _context.AuthenticatedCarts.Remove(authenticatedCart);
                    CartCount--;
                    mAdded = false;
                }
                else
                {
                    authenticatedCart = new AuthenticatedCart();
                    authenticatedCart.movieId = selectedId;
                    authenticatedCart.userId = userid;
                    _context.AuthenticatedCarts.Add(authenticatedCart);
                    CartCount++;
                    mAdded = true;
                }
            }
            else
            {
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
                UnauthenticatedCart unauthenticatedCart = new UnauthenticatedCart();
                unauthenticatedCart = _context.UnauthenticatedCarts.SingleOrDefault(p => p.cartId == cartid && p.movieId == selectedId);
                CartCount = _context.UnauthenticatedCarts.Where(p => p.cartId == cartid).ToList().Count;
                if (unauthenticatedCart != null)
                {
                    _context.UnauthenticatedCarts.Remove(unauthenticatedCart);
                    CartCount--;
                    mAdded = false;
                }
                else
                {
                    unauthenticatedCart = new UnauthenticatedCart();
                    unauthenticatedCart.cartId = cartid;
                    unauthenticatedCart.movieId = selectedId;
                    _context.UnauthenticatedCarts.Add(unauthenticatedCart);
                    CartCount++;
                    mAdded = true;
                }
            }
            _context.SaveChanges();
            CartButtonViewModel cartButtonViewModel = new CartButtonViewModel()
            {
                movieAdded = mAdded,
                MovieId = selectedId,
                TotalCartCount = CartCount
            };
            return PartialView("~/Views/Movie/_CartButtonPartial.cshtml", cartButtonViewModel);
        }
        public ActionResult CartSummary()
        {
            return View(GetViewModel());
        }
        public ActionResult RemoveSelected(int selectedId = -1)
        {
            if (selectedId == -1)
                return new HttpNotFoundResult(); 
            if (User.Identity.IsAuthenticated)
            {
                string userid = FetchUserId();
                _context.AuthenticatedCarts.Remove(_context.AuthenticatedCarts.First(p => p.userId == userid && p.movieId == selectedId));
            }
            else
            {
                HttpCookie cookie = Request.Cookies["CartCookie"];
                int cartid = Convert.ToInt32(cookie["CartId"]);
                _context.UnauthenticatedCarts.Remove(_context.UnauthenticatedCarts.First(p => p.cartId == cartid && p.movieId == selectedId));
            }
            _context.SaveChanges();
            return PartialView("_SelectedMoviesPartial", GetViewModel());
        }
        public ActionResult NavCart()
        {
            MoviesViewModel moviesViewModel = new MoviesViewModel();
            moviesViewModel = GetViewModel();
            if (moviesViewModel.movies != null)
                return PartialView("_NavCartPartial", moviesViewModel.movies.Count);
            else
                return PartialView("_NavCartPartial", 0);
        }
        [Authorize]
        public ActionResult SaveCart()
        {
            string userid = FetchUserId();
            HttpCookie cookie = Request.Cookies["CartCookie"];
            if (cookie == null)
                return new HttpNotFoundResult();
            int cartid = Convert.ToInt32(cookie["CartId"]);
            List<UnauthenticatedCart> unauthenticatedCarts = new List<UnauthenticatedCart>();
            unauthenticatedCarts = _context.UnauthenticatedCarts.Where(p => p.cartId == cartid).ToList();
            Bookings bookings = new Bookings();
            bookings.userId = userid;
            bookings.bookingDate = DateTime.Now;
            List<Movie> movies = new List<Movie>();
            List<Genre> genres = new List<Genre>();
            movies = _context.Movies.Where(p => unauthenticatedCarts.Any(q => q.movieId == p.movieId)).ToList();
            genres = _context.Genres.Where(p => movies.Any(q => q.genreId == p.genreId)).ToList();
            foreach(var m in unauthenticatedCarts)
            {
                _context.UnauthenticatedCarts.Remove(m);
                _context.SaveChanges();
            }
            float totalprice = 0;
            foreach(var m in movies)
            {
                totalprice += m.moviePrice;
            }
            bookings.totalPrice = totalprice;
            _context.Bookings.Add(bookings);
            _context.SaveChanges();
            BookedMoviesList bookedMoviesList = new BookedMoviesList();
            bookedMoviesList.bookingId = bookings.BookingId;
            foreach(var m in movies)
            {
                bookedMoviesList.movieId = m.movieId;
                _context.bookedMoviesLists.Add(bookedMoviesList);
                _context.SaveChanges();
            }
            CurrentBooking currentBooking = new CurrentBooking();
            currentBooking.userid = userid;
            currentBooking.totalprice = bookings.totalPrice;
            currentBooking.movies = movies;
            currentBooking.genres = genres;
            return View(currentBooking);
        }
        
    }
}