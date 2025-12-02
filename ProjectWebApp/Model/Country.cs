using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    public partial class Country
    {
        public Country()
        {
            Airlines = new HashSet<Airline>();
            Airports = new HashSet<Airport>();
        }

        [Key]
        public int CountryId { get; set; }
        [Required]
        [StringLength(100)]
        public string CountryName { get; set; }
        [StringLength(5)]
        public string CountryCode { get; set; }

        [InverseProperty("Country")]
        public virtual ICollection<Airline> Airlines { get; set; }
        [InverseProperty("Country")]
        public virtual ICollection<Airport> Airports { get; set; }
    }
}
