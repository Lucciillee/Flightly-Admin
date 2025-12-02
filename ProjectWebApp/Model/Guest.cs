using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    public partial class Guest
    {
        public Guest()
        {
            Bookings = new HashSet<Booking>();
            Payments = new HashSet<Payment>();
            Reviews = new HashSet<Review>();
        }

        [Key]
        public int GuestId { get; set; }
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(100)]
        public string LastName { get; set; }
        [StringLength(255)]
        public string Email { get; set; }
        [StringLength(20)]
        public string Phone { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }

        [InverseProperty("Guest")]
        public virtual ICollection<Booking> Bookings { get; set; }
        [InverseProperty("Guest")]
        public virtual ICollection<Payment> Payments { get; set; }
        [InverseProperty("Guest")]
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
