using System;

namespace SevenDaysFront.Data
{
    public class Movie
    {
        public int IdMovie { get; set; }
        public string Title { get; set; }
        public decimal? RentalPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public int? LikesCounter { get; set; }
        public int? Stock { get; set; }
    }
}
