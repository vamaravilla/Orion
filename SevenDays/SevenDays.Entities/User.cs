using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SevenDays.Entities
{
    public partial class User
    {
        public User()
        {
            Liked = new HashSet<Liked>();
            Rental = new HashSet<Rental>();
            Sale = new HashSet<Sale>();
        }

        [Key]
        public int IdUser { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(100)]
        public string Email { get; set; }
        [StringLength(100)]
        public string Password { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }
        public int? Profile { get; set; }
        public bool? IsActive { get; set; }

        [InverseProperty("IdUserNavigation")]
        public virtual ICollection<Liked> Liked { get; set; }
        [InverseProperty("IdUserNavigation")]
        public virtual ICollection<Rental> Rental { get; set; }
        [InverseProperty("IdUserNavigation")]
        public virtual ICollection<Sale> Sale { get; set; }
    }
}
