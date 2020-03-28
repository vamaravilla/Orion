using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SevenDays.Api.Models
{
    public partial class Sale
    {
        [Key]
        public int IdInventory { get; set; }
        [Key]
        public int IdUser { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SaleDate { get; set; }
        [Column(TypeName = "decimal(9, 3)")]
        public decimal? SalePrice { get; set; }

        [ForeignKey(nameof(IdInventory))]
        [InverseProperty(nameof(Inventory.Sale))]
        public virtual Inventory IdInventoryNavigation { get; set; }
        [ForeignKey(nameof(IdUser))]
        [InverseProperty(nameof(User.Sale))]
        public virtual User IdUserNavigation { get; set; }
    }
}
