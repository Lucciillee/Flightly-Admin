using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    [Table("PromoStatus")]
    [Index("StatusName", Name = "UQ__PromoSta__05E7698A442B3F54", IsUnique = true)]
    public partial class PromoStatus
    {
        public PromoStatus()
        {
            PromoCodes = new HashSet<PromoCode>();
        }

        [Key]
        public int StatusId { get; set; }
        [Required]
        [StringLength(50)]
        public string StatusName { get; set; }

        [InverseProperty("Status")]
        public virtual ICollection<PromoCode> PromoCodes { get; set; }
    }
}
