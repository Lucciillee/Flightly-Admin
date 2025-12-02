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

        [ForeignKey("PreferredOriginAirportId")]
        [InverseProperty("UserPreferences")]
        public virtual Airport PreferredOriginAirport { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("UserPreferences")]
        public virtual UserProfile User { get; set; }
    }
}
