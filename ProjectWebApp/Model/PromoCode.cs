using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    [Index("Code", Name = "UQ__PromoCod__A25C5AA7CB34F399", IsUnique = true)]
    public partial class PromoCode
    {
        public PromoCode()
        {
            Bookings = new HashSet<Booking>();
        }

        [Key]
        public int PromoId { get; set; }
        [Required]
        [StringLength(50)]
        public string Code { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Value { get; set; }
        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }
        public int? UsageLimit { get; set; }
        [StringLength(255)]
        public string Notes { get; set; }
        public int StatusId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        public int? UserId { get; set; }

        [ForeignKey("StatusId")]
        [InverseProperty("PromoCodes")]
        public virtual PromoStatus Status { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("PromoCodes")]
        public virtual UserProfile User { get; set; }
        [InverseProperty("Promo")]
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
