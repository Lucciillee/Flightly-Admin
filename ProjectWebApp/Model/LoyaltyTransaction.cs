using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    public partial class LoyaltyTransaction
    {
        [Key]
        public int LoyaltyTransactionId { get; set; }
        public int UserId { get; set; }
        public int? BookingId { get; set; }
        public int Points { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime TransactionDate { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        public int TransactionTypeId { get; set; }

        [ForeignKey("BookingId")]
        [InverseProperty("LoyaltyTransactions")]
        public virtual Booking Booking { get; set; }
        [ForeignKey("TransactionTypeId")]
        [InverseProperty("LoyaltyTransactions")]
        public virtual TransactionType TransactionType { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("LoyaltyTransactions")]
        public virtual UserProfile User { get; set; }
    }
}
