using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    public partial class Report
    {
        [Key]
        public int ReportId { get; set; }
        public int ReportTypeId { get; set; }
        [Column(TypeName = "date")]
        public DateTime DateFrom { get; set; }
        [Column(TypeName = "date")]
        public DateTime DateTo { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? GeneratedOn { get; set; }
        [StringLength(255)]
        public string FilePath { get; set; }
        [StringLength(500)]
        public string Notes { get; set; }
        public int? UserId { get; set; }

        [ForeignKey("ReportTypeId")]
        [InverseProperty("Reports")]
        public virtual ReportType ReportType { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("Reports")]
        public virtual UserProfile User { get; set; }
    }
}
