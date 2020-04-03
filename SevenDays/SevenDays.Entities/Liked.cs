using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SevenDays.Entities
{
    public partial class Liked
    {
        [Key]
        public int IdMovie { get; set; }
        [Key]
        public int IdUser { get; set; }

        [ForeignKey(nameof(IdMovie))]
        [InverseProperty(nameof(Movie.Liked))]
        public virtual Movie IdMovieNavigation { get; set; }
        [ForeignKey(nameof(IdUser))]
        [InverseProperty(nameof(User.Liked))]
        public virtual User IdUserNavigation { get; set; }
    }
}
