using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    public partial class BookingStatus
    {
        public BookingStatus()
        {
            Bookings = new HashSet<Booking>();
        }

        [Key]
        public int StatusId { get; set; }
        [Required]
        [StringLength(50)]
        public string StatusName { get; set; }

        [InverseProperty("BookingStatus")]
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
