using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    public partial class Booking
    {
        public Booking()
        {
            LoyaltyTransactions = new HashSet<LoyaltyTransaction>();
        }

        [Key]
        public int BookingId { get; set; }
        public int? UserId { get; set; }
        public int FlightId { get; set; }
        public int PaymentId { get; set; }
        public int BookingStatusId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime BookingDate { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalAmount { get; set; }
        public int? LoyaltyPointsEarned { get; set; }
        public int? PromoId { get; set; }
        public int? GuestId { get; set; }
        public int ClassTypeId { get; set; }

        [ForeignKey("BookingStatusId")]
        [InverseProperty("Bookings")]
        public virtual BookingStatus BookingStatus { get; set; }
        [ForeignKey("ClassTypeId")]
        [InverseProperty("Bookings")]
        public virtual ClassType ClassType { get; set; }
        [ForeignKey("FlightId")]
        [InverseProperty("Bookings")]
        public virtual Flight Flight { get; set; }
        [ForeignKey("GuestId")]
        [InverseProperty("Bookings")]
        public virtual Guest Guest { get; set; }
        [ForeignKey("PaymentId")]
        [InverseProperty("Bookings")]
        public virtual Payment Payment { get; set; }
        [ForeignKey("PromoId")]
        [InverseProperty("Bookings")]
        public virtual PromoCode Promo { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("Bookings")]
        public virtual UserProfile User { get; set; }
        [InverseProperty("Booking")]
        public virtual Review Review { get; set; }
        [InverseProperty("Booking")]
        public virtual ICollection<LoyaltyTransaction> LoyaltyTransactions { get; set; }
    }
}
