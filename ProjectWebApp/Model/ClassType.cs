using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    public partial class ClassType
    {
        public ClassType()
        {
            Bookings = new HashSet<Booking>();
        }

        [Key]
        public int ClassTypeId { get; set; }
        [Required]
        [StringLength(50)]
        public string ClassName { get; set; }

        [InverseProperty("ClassType")]
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
