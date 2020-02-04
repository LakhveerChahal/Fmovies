using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fmovies.Models
{
    public class BookedMoviesList
    {
        [Key]
        public int id { get; set; }
        public int bookingId { get; set; }
        public int movieId { get; set; }
    }
}