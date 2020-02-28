using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Fmovies.Models
{
    public class UnauthenticatedCart
    {
        [Key]
        public int id { get; set; }
        public int cartId { get; set; }
        public int movieId { get; set; }
    }
}