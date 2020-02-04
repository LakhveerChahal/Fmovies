using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fmovies.Models;

namespace Fmovies.ViewModels
{
    public class MoviesViewModel
    {
        public List<Movie> movies { get; set; }
        public List<Genre> genres { get; set; }
        public string userid { get; set; }
        public float totalPrice { get; set; }
        public List<int> selectedIds { get; set; }

    }
}