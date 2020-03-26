using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SevenDays.Api.Models;

namespace SevenDays.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : ControllerBase
    {
        private static readonly string[] Titles = new[]
        {
            "MovieA", "MiveB", "MovieC"
        };

        private readonly ILogger<MovieController> _logger;

        public MovieController(ILogger<MovieController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Movie> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 3).Select(index => new Movie
            {
                Id = index,
                Title = Titles[rng.Next(Titles.Length)],
                Description = "Description",
                Stock = 10,
                RentalPrice = 5.00m,
                SalePrice = 20.00m,
                Availability = true
            })
            .ToArray();
        }
    }
}
