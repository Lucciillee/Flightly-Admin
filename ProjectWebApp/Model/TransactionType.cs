using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    [Index("TypeName", Name = "UQ_TransactionTypes_TypeName", IsUnique = true)]
    public partial class TransactionType
    {
        public TransactionType()
        {
            LoyaltyTransactions = new HashSet<LoyaltyTransaction>();
        }

        [Key]
        public int TransactionTypeId { get; set; }
        [Required]
        [StringLength(50)]
        public string TypeName { get; set; }

        [InverseProperty("TransactionType")]
        public virtual ICollection<LoyaltyTransaction> LoyaltyTransactions { get; set; }
    }
}
