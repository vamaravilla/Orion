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

        private SevenDaysContext _context;

        public MovieController(SevenDaysContext context)
        {
            _context = context;
        }

      

        [HttpGet]
        public IEnumerable<Movie> Get()
        {
            return _context.Movie.ToArray();
        }
    }
}
