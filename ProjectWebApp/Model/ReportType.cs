using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    [Index("Code", Name = "UQ_ReportTypes_Code", IsUnique = true)]
    [Index("TypeName", Name = "UQ__ReportTy__D4E7DFA8D6F2682F", IsUnique = true)]
    public partial class ReportType
    {
        public ReportType()
        {
            Reports = new HashSet<Report>();
        }

        [Key]
        public int ReportTypeId { get; set; }
        [Required]
        [StringLength(50)]
        public string TypeName { get; set; }
        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [InverseProperty("ReportType")]
        public virtual ICollection<Report> Reports { get; set; }
    }
}
