using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    [Index("RoleName", Name = "UQ__Roles__8A2B6160A949F4DB", IsUnique = true)]
    public partial class Role
    {
        public Role()
        {
            UserProfiles = new HashSet<UserProfile>();
        }

        [Key]
        public int RoleId { get; set; }
        [Required]
        [StringLength(50)]
        public string RoleName { get; set; }

        [InverseProperty("Role")]
        public virtual ICollection<UserProfile> UserProfiles { get; set; }
    }
}
