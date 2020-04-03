using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SevenDays.Api.Models
{
    public class MovieModel
    {
        [Required]
        public IFormFile Image { set; get; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        [Required]
        [Column(TypeName = "decimal(9, 3)")]
        public decimal RentalPrice { get; set; }
        [Required]
        [Column(TypeName = "decimal(9, 3)")]
        public decimal SalePrice { get; set; }

    }
}
