using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SevenDays.Api.Models
{
    public partial class Movie
    {
        public Movie()
        {
            Inventory = new HashSet<Inventory>();
            Liked = new HashSet<Liked>();
        }

        [Key]
        public int IdMovie { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        [StringLength(50)]
        public string Image { get; set; }
        public int? Stock { get; set; }
        [Column(TypeName = "decimal(9, 3)")]
        public decimal? RentalPrice { get; set; }
        [Column(TypeName = "decimal(9, 3)")]
        public decimal? SalePrice { get; set; }
        public bool? IsAvailable { get; set; }

        [InverseProperty("IdMovieNavigation")]
        public virtual ICollection<Inventory> Inventory { get; set; }
        [InverseProperty("IdMovieNavigation")]
        public virtual ICollection<Liked> Liked { get; set; }
    }
}
