using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SevenDays.BusinessLogic;
using SevenDays.Entities;
using Microsoft.Extensions.Configuration;
using SevenDays.Util;
using SevenDays.Api.Models;
using Microsoft.AspNetCore.JsonPatch;
using SevenDays.Api.Helpers;
using System;

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
        public ActionResult<Movie> GetMovie(int id)
        {
            BLResult<Movie> result = movieTransactionScript.GetMovieById(id);

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
        /// Get all available movies
        /// </summary>
        /// <returns>Movies</returns>
        // GET: api/Movies
        [AllowAnonymous]
        [HttpGet]
        public ActionResult<Movie> GetMovies()
        {
            BLResults<Movie> result = movieTransactionScript.GetMovies();

            if (!result.Success)
            {
                return StatusCode(500,result);
            }

            return Ok(result.Items);
        }

        /// <summary>
        /// Get All movies filtered with paginng
        /// </summary>
        /// <returns>List of movies</returns>
        // GET: api/Movies
        [AllowAnonymous]
        [HttpGet("filter")]
        public ActionResult<PagedCollectionResponse<Movie>> GetFilteredMovie([FromQuery] FilterModel filter)
        {
            if (filter != null && filter.IncludeInactive == true)
            {
                // Only Admin users are allowed to perform this action
                if (!IsUserAdminAutenticated())
                {
                    return Unauthorized(new { message = "Not allowed" });
                }
            }

            BLFResults<Movie> dbResult = movieTransactionScript.GetMoviesFiltered(filter);
            if (!dbResult.Success)
            {
                return StatusCode(500, dbResult);
            }

            // Get the data for the current page  
            var result = new PagedCollectionResponse<Movie>();
            result.Items = dbResult.Items;

            // Get next page URL string  
            FilterModel nextFilter = filter.Clone() as FilterModel;
            nextFilter.Page += 1;
            string nextUrl = dbResult.CountNextFilter <= 0 ? null : this.Url.Action("GetFilteredMovie", null, nextFilter, Request.Scheme);

            // Get previous page URL string  
            FilterModel previousFilter = filter.Clone() as FilterModel;
            previousFilter.Page -= 1;
            string previousUrl = previousFilter.Page <= 0 ? null : this.Url.Action("GetFilteredMovie", null, previousFilter, Request.Scheme);

            result.NextPage = !string.IsNullOrWhiteSpace(nextUrl) ? new Uri(nextUrl) : null;
            result.PreviousPage = !string.IsNullOrWhiteSpace(previousUrl) ? new Uri(previousUrl) : null;

            return Ok(result);
        }


        /// <summary>
        /// Partial update of one movie
        /// </summary>
        /// <param name="id">Id Movie</param>
        /// <param name="patchMovie">Patch operations</param>
        /// <returns>Result</returns>
        // PATCH: api/Movies/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchMovie(int id, [FromBody]JsonPatchDocument<Movie> patchMovie)
        {
            // Only Admin users are allowed to perform this action
            if (!IsUserAdminAutenticated())
            {
                return Unauthorized(new { message = "Not allowed" });
            }

            int idUser = GetCurrentUser();

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            BLResult<Movie> result = await movieTransactionScript.PatchMovie(patchMovie, id,idUser);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
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

        /// <summary>
        /// Get Id current user
        /// </summary>
        /// <returns>Id User/returns>
        private int GetCurrentUser()
        {
            int idUser = -1;
            // Get logged user if exists
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userCompositeId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;

            if (userCompositeId != null)
            {
                var splitUserId = userCompositeId.Split('.');
                // Validating if user is the same
                if (splitUserId != null && splitUserId.Length == 2)
                {
                    idUser = int.Parse(splitUserId[0]);
                }
            }
            return idUser;
        }
    }
}
