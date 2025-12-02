using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    [Index("Code", Name = "UQ__Airlines__A25C5AA73F5CDF1B", IsUnique = true)]
    public partial class Airline
    {
        public Airline()
        {
            Flights = new HashSet<Flight>();
        }

        [Key]
        public int AirlineId { get; set; }
        [Required]
        [StringLength(100)]
        public string AirlineName { get; set; }
        [Required]
        [StringLength(10)]
        public string Code { get; set; }
        public int CountryId { get; set; }
        public int StatusId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }
        [StringLength(500)]
        public string LogoUrl { get; set; }
        public int? UserId { get; set; }

        [ForeignKey("CountryId")]
        [InverseProperty("Airlines")]
        public virtual Country Country { get; set; }
        [ForeignKey("StatusId")]
        [InverseProperty("Airlines")]
        public virtual AirlineStatus Status { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("Airlines")]
        public virtual UserProfile User { get; set; }
        [InverseProperty("Airline")]
        public virtual ICollection<Flight> Flights { get; set; }
    }
}
