using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SevenDays.Api.Models
{
    public partial class Rental
    {
        [Key]
        public int IdRental { get; set; }
        public int IdInventory { get; set; }
        public int IdUser { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RentalDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ReturnDate { get; set; }
        [Column(TypeName = "decimal(9, 3)")]
        public decimal? RentalPrice { get; set; }
        [Column(TypeName = "decimal(9, 3)")]
        public decimal? Penalty { get; set; }

        [ForeignKey(nameof(IdInventory))]
        [InverseProperty(nameof(Inventory.Rental))]
        public virtual Inventory IdInventoryNavigation { get; set; }
        [ForeignKey(nameof(IdUser))]
        [InverseProperty(nameof(User.Rental))]
        public virtual User IdUserNavigation { get; set; }
    }
}
