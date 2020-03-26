using System;


namespace SevenDays.Api.Models
{
    public class Movie
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public int Stock { get; set; }

        public decimal RentalPrice { get; set; }

        public decimal SalePrice { get; set; }

        public bool Availability { get; set; }
    }
}