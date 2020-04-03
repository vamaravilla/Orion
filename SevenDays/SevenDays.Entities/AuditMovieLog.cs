using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SevenDays.Entities
{
    public class AuditMovieLog
    {
        [Key]
        public int IdAuditMovieLog { get; set; }
        [Required]
        public int IdUser { get; set; }
        [Required]
        public int IdMovie { get; set; }
        [StringLength(100)]
        public string Title { get; set; }
        [Column(TypeName = "decimal(9, 3)")]
        public decimal? RentalPrice { get; set; }
        [Column(TypeName = "decimal(9, 3)")]
        public decimal? SalePrice { get; set; }
        [StringLength(20)]
        public string Action { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ActionDate { get; set; }
    }
}
