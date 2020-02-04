using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fmovies.Models;

namespace Fmovies.ViewModels
{
    public class BookingViewModel
    {
        public string userid { get; set; }
        public List<Bookings> bookings { get; set; }
        public List<BookedMoviesList> bookedMoviesLists { get; set; }
        public List<Movie> movies { get; set; }
    }
}