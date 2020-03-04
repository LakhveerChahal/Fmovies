using Fmovies.Models;
using Fmovies.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Claims;
using System.Web.Mvc;

namespace Fmovies.Controllers
{
    public class MovieController : Controller
    {
        // GET: Movie
        private ApplicationDbContext _context;
        public MovieController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public ViewResult Index()
        {
            List<Movie> getMovies = new List<Movie>();
            getMovies = _context.Movies.ToList();

            List<Genre> getGenres = new List<Genre>();
            getGenres = _context.Genres.ToList();

            MoviesViewModel moviesViewModel = new MoviesViewModel
            {
                movies = getMovies,
                genres = getGenres,
                totalPrice = 0
            };
            if (User.Identity.IsAuthenticated)
            {
                UserCartCheck();
                string userid = FetchUserId();
                List<AuthenticatedCart> authenticatedCarts = new List<AuthenticatedCart>();
                authenticatedCarts = _context.AuthenticatedCarts.Where(p => p.userId == userid).ToList();
                List<int> movieIds = new List<int>();
                foreach(var x in authenticatedCarts)
                {
                    movieIds.Add(x.movieId);
                }
                ViewBag.movieIds = movieIds;
            }
            else
            {
                HttpCookie cookie = Request.Cookies["CartCookie"];
                if(cookie != null)
                {
                    int cartid = Convert.ToInt32(cookie["CartId"]);
                    List<UnauthenticatedCart> unauthenticatedCarts = new List<UnauthenticatedCart>();
                    unauthenticatedCarts = _context.UnauthenticatedCarts.Where(p => p.cartId == cartid).ToList();
                    List<int> movieIds = new List<int>();
                    foreach(var x in unauthenticatedCarts)
                    {
                        movieIds.Add(x.movieId);
                    }
                    ViewBag.movieIds = movieIds;
                }
            }
            return View(moviesViewModel);
        }

        // Helper Method
        public void UserCartCheck()
        {
            HttpCookie cookie = Request.Cookies["CartCookie"];
            if (cookie != null)
            {
                int cartid = Convert.ToInt32(cookie["CartId"]);
                List<UnauthenticatedCart> unauthenticatedCarts = new List<UnauthenticatedCart>();
                unauthenticatedCarts = _context.UnauthenticatedCarts.Where(p => p.cartId == cartid).ToList();
                AuthenticatedCart authenticatedCart = new AuthenticatedCart();
                if (unauthenticatedCarts.Count == 0)
                {
                    return;
                }
                authenticatedCart.userId = FetchUserId();
                foreach (var p in unauthenticatedCarts)
                {
                    authenticatedCart.movieId = p.movieId;
                    if (!_context.AuthenticatedCarts.Any(m => (m.userId == authenticatedCart.userId && m.movieId == authenticatedCart.movieId)))
                    {
                        _context.AuthenticatedCarts.Add(authenticatedCart);
                    }
                    _context.UnauthenticatedCarts.Remove(p);
                    _context.SaveChanges();
                }
            }
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
    }
}