using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fmovies.Models;
namespace Fmovies.ViewModels
{
    public class CurrentBooking
    {
        public string userid { get; set; }
        public List<Movie> movies { get; set; }
        public List<Genre> genres { get; set; }
        public float totalprice { get; set; }
    }
}