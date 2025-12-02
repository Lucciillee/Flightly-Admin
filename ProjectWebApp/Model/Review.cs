using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    [Index("BookingId", Name = "UQ_Reviews_Booking", IsUnique = true)]
    public partial class Review
    {
        [Key]
        public int ReviewId { get; set; }
        public int BookingId { get; set; }
        public int? UserId { get; set; }
        public int Rating { get; set; }
        [StringLength(500)]
        public string Comment { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        public int? GuestId { get; set; }

        [ForeignKey("BookingId")]
        [InverseProperty("Review")]
        public virtual Booking Booking { get; set; }
        [ForeignKey("GuestId")]
        [InverseProperty("Reviews")]
        public virtual Guest Guest { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("Reviews")]
        public virtual UserProfile User { get; set; }
    }
}
