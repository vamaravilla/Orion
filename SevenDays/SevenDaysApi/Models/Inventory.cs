using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SevenDays.Api.Models
{
    public partial class Inventory
    {
        public Inventory()
        {
            Rental = new HashSet<Rental>();
            Sale = new HashSet<Sale>();
        }

        [Key]
        public int IdInventory { get; set; }
        public int IdMovie { get; set; }
        public bool? IsNew { get; set; }
        public bool? IsAvailable { get; set; }

        [ForeignKey(nameof(IdMovie))]
        [InverseProperty(nameof(Movie.Inventory))]
        public virtual Movie IdMovieNavigation { get; set; }
        [InverseProperty("IdInventoryNavigation")]
        public virtual ICollection<Rental> Rental { get; set; }
        [InverseProperty("IdInventoryNavigation")]
        public virtual ICollection<Sale> Sale { get; set; }
    }
}
