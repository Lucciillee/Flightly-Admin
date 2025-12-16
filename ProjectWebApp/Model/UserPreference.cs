using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    public partial class UserPreference
    {
        [Key]
        public int PreferenceId { get; set; }
        public int UserId { get; set; }
        [StringLength(10)]
        public string PreferredCurrency { get; set; }
        public int? PreferredOriginAirportId { get; set; }
        public bool IsDarkMode { get; set; }
        [StringLength(10)]
        public string PreferredLanguage { get; set; }
        [StringLength(4)]
        public string PassportLast4 { get; set; }
        [Column(TypeName = "date")]
        public DateTime? PassportExpiryDate { get; set; }
        public bool IsSubscribedToNewsletter { get; set; }

        [ForeignKey("PreferredOriginAirportId")]
        [InverseProperty("UserPreferences")]
        public virtual Airport PreferredOriginAirport { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("UserPreferences")]
        public virtual UserProfile User { get; set; }
    }
}
