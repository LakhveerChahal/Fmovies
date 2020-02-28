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
        private List<int> selectedIds = new List<int>();
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
                    _context.AuthenticatedCarts.Add(authenticatedCart);
                    _context.UnauthenticatedCarts.Remove(p);
                    _context.SaveChanges();
                }
            }
        }
        private MoviesViewModel GetSelectedMovies(FormCollection form)
        {
            string[] checkboxes = form["movieCheckbox"].Split(',');
            List<int> selectedIds = new List<int>();
            foreach (string s in checkboxes)
            {
                if (s != "false")
                {
                    int temp = Convert.ToInt32(s);
                    selectedIds.Add(temp);
                }
            }
            List<Movie> getMovies = new List<Movie>();
            getMovies = _context.Movies.ToList();

            List<Genre> getGenres = new List<Genre>();
            getGenres = _context.Genres.ToList();

            MoviesViewModel moviesViewModel = new MoviesViewModel
            {
                movies = getMovies,
                genres = getGenres
            };
            moviesViewModel.selectedIds = selectedIds;
            moviesViewModel.userid = FetchUserId();
            return moviesViewModel;
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
        public ActionResult SelectedMovies()
        {
            var form = TempData["FormData"] as FormCollection;
            TempData.Remove("FormData");
            MoviesViewModel moviesViewModel = new MoviesViewModel();
            moviesViewModel = GetSelectedMovies(form);
            return View(moviesViewModel);
        }
        [HttpPost]
        public ActionResult SelectedMovies(FormCollection form)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["FormData"] = form;
                return new HttpUnauthorizedResult();
            }
            MoviesViewModel moviesViewModel = new MoviesViewModel();
            moviesViewModel = GetSelectedMovies(form);

            return View(moviesViewModel);
        }
        public PartialViewResult MoviesPartial(MoviesViewModel moviesViewModel)
        {
            return PartialView("_SelectedMoviesPartial", moviesViewModel);
        }
        [HttpPost]
        public PartialViewResult _SelectedMoviesPartial(int selectedId = -1)
        {
            MoviesViewModel moviesViewModel = new MoviesViewModel();
            moviesViewModel = (MoviesViewModel)ViewBag.movieViewModel;
            moviesViewModel = (MoviesViewModel)TempData["movieViewModel"];
            TempData.Keep("movieViewModel");
            if (selectedId != -1)
            {
                moviesViewModel.selectedIds.Remove(selectedId);
                if (TempData["removedIds"] == null)
                {
                    List<int> removedIds = new List<int>();
                    removedIds.Add(selectedId);
                    TempData["removedIds"] = removedIds;
                }
                else
                {
                    List<int> removedIds = (List<int>)TempData["removedIds"];
                    removedIds.Add(selectedId);
                    TempData["removedIds"] = removedIds;
                }
            }
            return PartialView(moviesViewModel);
        }

        [Authorize]
        public ActionResult SaveToCart()
        {
            MoviesViewModel moviesViewModel = new MoviesViewModel();
            moviesViewModel = (MoviesViewModel)TempData["movieViewModel"];
            TempData.Remove("movieViewModel");
            Bookings booking = new Bookings();
            booking.userId = moviesViewModel.userid;
            booking.bookingDate = DateTime.Now;
            booking.totalPrice = moviesViewModel.totalPrice;

            _context.Bookings.Add(booking);
            _context.SaveChanges();
            BookedMoviesList bookedMoviesList = new BookedMoviesList();
            bookedMoviesList.bookingId = booking.BookingId;
            foreach (int id in moviesViewModel.selectedIds)
            {
                bookedMoviesList.movieId = id;
                _context.bookedMoviesLists.Add(bookedMoviesList);
                _context.SaveChanges();
            }
            return RedirectToAction("DisplayCart");
        }
        [Authorize]
        public ActionResult DisplayCart()
        {
            List<Bookings> bookings = new List<Bookings>();
            bookings = _context.Bookings.ToList().FindAll(x => x.userId == FetchUserId());
            List<BookedMoviesList> bookedMoviesLists = new List<BookedMoviesList>();
            bookedMoviesLists = _context.bookedMoviesLists.ToList();
            BookingViewModel bookingViewModel = new BookingViewModel()
            {
                userid = FetchUserId(),
                bookings = bookings,
                bookedMoviesLists = bookedMoviesLists,
                movies = _context.Movies.ToList()
            };
            return View(bookingViewModel);
        }
    }
}