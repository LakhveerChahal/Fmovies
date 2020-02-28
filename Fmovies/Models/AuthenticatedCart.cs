using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fmovies.Models
{
    public class AuthenticatedCart
    {
        [Key]
        public int id { get; set; }
        public string userId { get; set; }
        public int movieId { get; set; }
    }
}