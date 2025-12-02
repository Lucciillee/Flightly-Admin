using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    [Table("FlightStatus")]
    public partial class FlightStatus
    {
        public FlightStatus()
        {
            Flights = new HashSet<Flight>();
        }

        [Key]
        public int StatusId { get; set; }
        [Required]
        [StringLength(50)]
        public string StatusName { get; set; }

        [InverseProperty("Status")]
        public virtual ICollection<Flight> Flights { get; set; }
    }
}
