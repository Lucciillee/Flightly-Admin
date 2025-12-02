using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    public partial class PaymentMethod
    {
        public PaymentMethod()
        {
            Payments = new HashSet<Payment>();
        }

        [Key]
        public int PaymentMethodId { get; set; }
        [Required]
        [StringLength(50)]
        public string MethodName { get; set; }
        [StringLength(50)]
        public string Provider { get; set; }

        [InverseProperty("PaymentMethod")]
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
