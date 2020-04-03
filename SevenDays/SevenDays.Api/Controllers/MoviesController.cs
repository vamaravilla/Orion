using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SevenDays.BusinessLogic;
using SevenDays.Entities;
using Microsoft.Extensions.Configuration;
using SevenDays.Util;
using SevenDays.Api.Models;

namespace SevenDays.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {

        MovieTransactionScript movieTransactionScript;
        private readonly IConfiguration Configuration;
        public MoviesController(IConfiguration configuration)
        {
            Configuration = configuration;
            movieTransactionScript = new MovieTransactionScript(configuration);
        }


        /// <summary>
        /// Create new movie
        /// </summary>
        /// <param name="movie">Movie object model</param>
        /// <returns>New movie details</returns>
        // POST: api/Movies
        [HttpPost]
        public async Task<ActionResult> PostMovie([FromForm] MovieModel movie)
        {
            // Only Admin users are allowed to perform this action
            if (!IsUserAdminAutenticated())
            {
                return Unauthorized(new { message = "Not allowed" });
            }
            // Validating parameters
            if(movie == null)
            {
                return BadRequest();
            }

            // Uploading Image to Storage first
            StorageAccountService storageService = new StorageAccountService(Configuration);
            StorageResult storageResult = await storageService.UploadImageToStorage(movie.Image, movie.Title);

            if (storageResult.Success)
            {
                // Save movie in Database
                Movie movieDb = new Movie()
                {
                    Title = movie.Title,
                    Description = movie.Description,
                    Stock = 0,
                    SalePrice = movie.SalePrice,
                    RentalPrice = movie.RentalPrice,
                    LikesCounter = 0,
                    Image = storageResult.Uri,
                    IsAvailable = true
                };
                BLResult<Movie> blResult = movieTransactionScript.AddMovie(movieDb);

                if (blResult.Success)
                {
                    return CreatedAtAction("GetMovie", new { id = blResult.Item.IdMovie }, blResult.Item);
                }
                else
                {
                    return StatusCode(500, blResult);
                }
            }
            else
            {
                return StatusCode(500, storageResult);
            }

        }

        /// <summary>
        /// Get one specific movie detail
        /// </summary>
        /// <param name="id">Id Movie</param>
        /// <returns>Movie</returns>
        // GET: api/Movies/5
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            BLResult<Movie> result = await movieTransactionScript.GetMovie(id);

            if (!result.Success)
            {
                return NotFound();
            }

            // If user is admin can get any movie
            if (!IsUserAdminAutenticated())
            {
                if (result.Item.IsAvailable == false)
                {
                    return Forbid();
                }
            }

            return Ok(result.Item);
        }


        /// <summary>
        /// Validate if user is authenticated and its profile is admin
        /// </summary>
        /// <returns>Boolean result</returns>
        private bool IsUserAdminAutenticated()
        {
            bool isAdmin = false;
            // Get logged user if exists
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userCompositeId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;

            if (userCompositeId != null)
            {
                var splitUserId = userCompositeId.Split('.');
                // Validating if user is Admin
                if (splitUserId != null && splitUserId.Length == 2 && splitUserId[1] == SimpleUser.Admin)
                {
                    isAdmin = true;
                }
            }
            return isAdmin;
        }


    }
}
