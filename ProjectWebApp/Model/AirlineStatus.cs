using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    [Table("AirlineStatus")]
    public partial class AirlineStatus
    {
        public AirlineStatus()
        {
            Airlines = new HashSet<Airline>();
        }

        [Key]
        public int StatusId { get; set; }
        [Required]
        [StringLength(20)]
        public string StatusName { get; set; }

        [InverseProperty("Status")]
        public virtual ICollection<Airline> Airlines { get; set; }
    }
}
