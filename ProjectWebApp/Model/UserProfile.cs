using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    [Table("UserProfile")]
    [Index("Email", Name = "UQ__UserProf__A9D10534D36BBF36", IsUnique = true)]
    public partial class UserProfile
    {
        public UserProfile()
        {
            Airlines = new HashSet<Airline>();
            Announcements = new HashSet<Announcement>();
            BackupHistories = new HashSet<BackupHistory>();
            Bookings = new HashSet<Booking>();
            FeaturedDestinations = new HashSet<FeaturedDestination>();
            Flights = new HashSet<Flight>();
            Logs = new HashSet<Log>();
            LoyaltyTransactions = new HashSet<LoyaltyTransaction>();
            Payments = new HashSet<Payment>();
            PromoCodes = new HashSet<PromoCode>();
            Reports = new HashSet<Report>();
            Reviews = new HashSet<Review>();
            TourismPopups = new HashSet<TourismPopup>();
            UserPreferences = new HashSet<UserPreference>();
        }

        [Key]
        public int UserId { get; set; }
        [StringLength(100)]
        public string FirstName { get; set; }
        [StringLength(100)]
        public string LastName { get; set; }
        [Required]
        [StringLength(150)]
        public string Email { get; set; }
        [StringLength(20)]
        public string PhoneNumber { get; set; }
        public int RoleId { get; set; }
        public int? LoyaltyPoints { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [StringLength(450)]
        public string IdentityUserId { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("RoleId")]
        [InverseProperty("UserProfiles")]
        public virtual Role Role { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Airline> Airlines { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Announcement> Announcements { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<BackupHistory> BackupHistories { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Booking> Bookings { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<FeaturedDestination> FeaturedDestinations { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Flight> Flights { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Log> Logs { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<LoyaltyTransaction> LoyaltyTransactions { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Payment> Payments { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<PromoCode> PromoCodes { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Report> Reports { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Review> Reviews { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<TourismPopup> TourismPopups { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<UserPreference> UserPreferences { get; set; }
    }
}
