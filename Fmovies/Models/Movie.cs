using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fmovies.Models
{
    public class Movie
    {
        public int movieId { get; set; }
        public string movieName { get; set; }
        public float moviePrice { get; set; }
        public int genreId { get; set; }
        public string URL { get; set; }

    }
}