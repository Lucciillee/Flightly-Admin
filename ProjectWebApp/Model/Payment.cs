using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    public partial class Payment
    {
        public Payment()
        {
            Bookings = new HashSet<Booking>();
        }

        [Key]
        public int PaymentId { get; set; }
        public int? UserId { get; set; }
        public int PaymentMethodId { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime PaymentDate { get; set; }
        public int PaymentStatusId { get; set; }
        public int? GuestId { get; set; }

        [ForeignKey("GuestId")]
        [InverseProperty("Payments")]
        public virtual Guest Guest { get; set; }
        [ForeignKey("PaymentMethodId")]
        [InverseProperty("Payments")]
        public virtual PaymentMethod PaymentMethod { get; set; }
        [ForeignKey("PaymentStatusId")]
        [InverseProperty("Payments")]
        public virtual PaymentStatus PaymentStatus { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("Payments")]
        public virtual UserProfile User { get; set; }
        [InverseProperty("Payment")]
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
