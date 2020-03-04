using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fmovies.ViewModels
{
    public class CartButtonViewModel
    {
        public bool movieAdded { get; set; }
        public int MovieId { get; set; }
        public int TotalCartCount { get; set; }
    }
}