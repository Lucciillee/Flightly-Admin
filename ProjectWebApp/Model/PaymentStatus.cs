using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    [Index("StatusName", Name = "UQ__PaymentS__05E7698A2C2727B3", IsUnique = true)]
    public partial class PaymentStatus
    {
        public PaymentStatus()
        {
            Payments = new HashSet<Payment>();
        }

        [Key]
        public int PaymentStatusId { get; set; }
        [Required]
        [StringLength(20)]
        public string StatusName { get; set; }

        [InverseProperty("PaymentStatus")]
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
