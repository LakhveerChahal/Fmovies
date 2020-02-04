using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fmovies.Models
{
    public class Bookings
    {
        [Key]
        public int BookingId { get; set; }
        public string userId { get; set; }
        public DateTime bookingDate { get; set; }
        public float totalPrice { get; set; }
    }
}