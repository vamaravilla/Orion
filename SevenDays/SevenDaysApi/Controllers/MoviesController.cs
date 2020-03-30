using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SevenDays.Api.Helpers;
using SevenDays.Api.Models;

namespace SevenDays.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly SevenDaysContext _context;

        public MoviesController(SevenDaysContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get All movies
        /// </summary>
        /// <returns>List of movies</returns>
        // GET: api/Movies
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovie([FromQuery]bool? available, [FromQuery]string sort)
        {

            List<Movie> moviesListed;
            // Get logged user if exists
            if (IsUserAdminAutenticated()) 
            {
                // If user is Admin get all Movies (available or not)
                if(available == null)
                {
                    moviesListed = await _context.Movie.ToListAsync();
                }
                else
                {
                    // If available param is present, filter movies
                    moviesListed = await _context.Movie.Where(m => m.IsAvailable == available).ToListAsync();
                }
               
            }
            else
            {
                // If not admin user, available param is not allowd
                if(available != null)
                {
                    return Unauthorized(new { message = "Not allowed" });
                }
                // In other cases get only available movies
                moviesListed = await _context.Movie.Where(m => m.IsAvailable == true).ToListAsync();

            }

            //Sorting movie list
            switch (sort)
            {
                case "title_desc":
                    moviesListed = moviesListed.OrderByDescending(m => m.Title).ToList();
                    break;
                case "title":
                    moviesListed = moviesListed.OrderBy(m => m.Title).ToList();
                    break;
                case "popularity_desc":
                    moviesListed = moviesListed.OrderByDescending(m => m.LikesCounter).ToList();
                    break;
                case "popularity":
                    moviesListed = moviesListed.OrderBy(m => m.LikesCounter).ToList();
                    break;
                default:
                    moviesListed = moviesListed.OrderBy(m => m.Title).ToList();
                    break;
            }

            return moviesListed;
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
            if(filter != null && filter.IncludeInactive == true)
            {
                // Only Admin users are allowed to perform this action
                if (!IsUserAdminAutenticated())
                {
                    return Unauthorized(new { message = "Not allowed" });
                }
            }

            // Filtering logic  
            Func<FilterModel, IEnumerable<Movie>> filterData = (filterModel) =>
            {
                if(filterModel.Title != null && !string.IsNullOrEmpty(filterModel.Title))
                {
                    if (filterModel.IncludeInactive)
                    {
                        return _context.Movie.Where(m => m.Title.Contains(filterModel.Title, StringComparison.InvariantCultureIgnoreCase))
                              .Skip((filterModel.Page - 1) * filter.Limit)
                              .Take(filterModel.Limit);
                    }
                    else
                    {
                        return _context.Movie.Where(m => m.Title.Contains(filterModel.Title, StringComparison.InvariantCultureIgnoreCase) && m.IsAvailable == true)
                              .Skip((filterModel.Page - 1) * filter.Limit)
                              .Take(filterModel.Limit);
                    }
                  
                }
                else
                {
                    if (filterModel.IncludeInactive)
                    {
                        return _context.Movie.ToList()
                              .Skip((filterModel.Page - 1) * filter.Limit)
                              .Take(filterModel.Limit);
                    }
                    else
                    {
                        return _context.Movie.Where(m => m.IsAvailable == true)
                              .Skip((filterModel.Page - 1) * filter.Limit)
                              .Take(filterModel.Limit);
                    }
                }
              
            };

            // Get the data for the current page  
            var result = new PagedCollectionResponse<Movie>();
            result.Items = filterData(filter);

            // Get next page URL string  
            FilterModel nextFilter = filter.Clone() as FilterModel;
            nextFilter.Page += 1;
            String nextUrl = filterData(nextFilter).Count() <= 0 ? null : this.Url.Action("GetFilteredMovie", null, nextFilter, Request.Scheme);

            // Get previous page URL string  
            FilterModel previousFilter = filter.Clone() as FilterModel;
            previousFilter.Page -= 1;
            String previousUrl = previousFilter.Page <= 0 ? null : this.Url.Action("GetFilteredMovie", null, previousFilter, Request.Scheme);

            result.NextPage = !String.IsNullOrWhiteSpace(nextUrl) ? new Uri(nextUrl) : null;
            result.PreviousPage = !String.IsNullOrWhiteSpace(previousUrl) ? new Uri(previousUrl) : null;

            return  result;
        }

        /// <summary>
        /// Search movies by name
        /// </summary>
        /// <returns>List of movies</returns>
        // GET: api/Movies
        [AllowAnonymous]
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Movie>>> SearchMovie([FromQuery]string? name)
        {

            List<Movie> moviesListed;
            // Get logged user if exists
            if (IsUserAdminAutenticated())
            {
                // Only Admin users can search in all records
                moviesListed = await _context.Movie.Where(m => m.Title.Contains(name)).ToListAsync();
            }
            else
            {
                moviesListed = await _context.Movie.Where(m => m.Title.Contains(name) && m.IsAvailable == true).ToListAsync();
            }

            return moviesListed;
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
            var movie = await _context.Movie.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            // If user is admin can get any movie
            if (!IsUserAdminAutenticated())
            {
                if(movie.IsAvailable == false)
                {
                    return NotFound();
                }
            }
           
            return movie;
        }

        /// <summary>
        /// Update complete movie
        /// </summary>
        /// <param name="id">Id Movie</param>
        /// <param name="movie">Movie object</param>
        /// <returns>No content</returns>
        // PUT: api/Movies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, Movie movie)
        {
            // Only Admin users are allowed to perform this action
            if (!IsUserAdminAutenticated())
            {
                return Unauthorized(new { message = "Not allowed" });
            }

            if (id != movie.IdMovie)
            {
                return BadRequest();
            }

            _context.Entry(movie).State = EntityState.Modified;

            // Adding Audit Log
            AddAuditMovieLog(movie);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Partial update of one movie
        /// </summary>
        /// <param name="id">Id Movie</param>
        /// <param name="patchMovie">Patch operations</param>
        /// <returns>No content</returns>
        // PATCH: api/Movies/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchMovie(int id, [FromBody]JsonPatchDocument<Movie> patchMovie)
        {
            // Only Admin users are allowed to perform this action
            if (!IsUserAdminAutenticated())
            {
                return Unauthorized(new { message = "Not allowed" });
            }

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            // Get our original person object from the database.
            var movie = await _context.Movie.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            try
            {
                // Applying Path to DB Object
                patchMovie.ApplyTo(movie);
                _context.Movie.Update(movie);

                // Adding Audit Log
                AddAuditMovieLog(movie);
               
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Create one movie
        /// </summary>
        /// <param name="movie">Movie object</param>
        /// <returns>New Movie details</returns>
        // POST: api/Movies
        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie(Movie movie)
        {
            // Only Admin users are allowed to perform this action
            if (!IsUserAdminAutenticated())
            {
                return Unauthorized(new { message = "Not allowed" });
            }

            _context.Movie.Add(movie);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovie", new { id = movie.IdMovie }, movie);
        }

        /// <summary>
        /// Remove one specific movie
        /// </summary>
        /// <param name="id">Id Movie</param>
        /// <returns>Movie deleted</returns>
        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Movie>> DeleteMovie(int id)
        {
            // Only Admin users are allowed to perform this action
            if (!IsUserAdminAutenticated())
            {
                return Unauthorized(new { message = "Not allowed" });
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            _context.Movie.Remove(movie);
            await _context.SaveChangesAsync();

            return movie;
        }

        /// <summary>
        /// Set availability in false
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Movie edited</returns>
        [HttpDelete("{id}/availability")]
        public async Task<ActionResult<Movie>> DeleteAvailability(int id)
        {
            // Only Admin users are allowed to perform this action
            if (!IsUserAdminAutenticated())
            {
                return Unauthorized(new { message = "Not allowed" });
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            // Disable availability
            movie.IsAvailable = false;
            _context.Movie.Update(movie);
            await _context.SaveChangesAsync();

            return movie;
        }

        /// <summary>
        /// Set availability in true
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Movie Edited</returns>
        [HttpPut("{id}/availability")]
        public async Task<ActionResult<Movie>> PutAvailability(int id)
        {
            // Only Admin users are allowed to perform this action
            if (!IsUserAdminAutenticated())
            {
                return Unauthorized(new { message = "Not allowed" });
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            // Enable availability
            movie.IsAvailable = true;
            _context.Movie.Update(movie);
            await _context.SaveChangesAsync();

            return movie;
        }


        /// <summary>
        /// Auxiliary method to validate one movie existence
        /// </summary>
        /// <param name="id">Id Movie</param>
        /// <returns>Boolean</returns>
        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.IdMovie == id);
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
        /// Get current user logged
        /// </summary>
        /// <returns>Id User</returns>
        private int? GetUserLogged()
        {
            int? idUser = null;
            // Get logged user if exists
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userCompositeId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            if (userCompositeId != null)
            {
                var splitUserId = userCompositeId.Split('.');
                // Validating if user is Admin
                if (splitUserId != null && splitUserId.Length == 2)
                {
                    idUser = int.Parse(splitUserId[0]);
                }
            }
            return idUser;
        }

        /// <summary>
        /// Addinf Audit Log for Movies
        /// </summary>
        /// <param name="movieEdited">Movie edited</param>
        private void AddAuditMovieLog(Movie movieEdited)
        {
            int? idUser = GetUserLogged();
            if (idUser != null)
            {
                AuditMovieLog audit = new AuditMovieLog()
                {
                    IdMovie = movieEdited.IdMovie,
                    IdUser = (int)idUser,
                    Title = movieEdited.Title,
                    RentalPrice = movieEdited.RentalPrice,
                    SalePrice = movieEdited.SalePrice,
                    Action = "Update",
                };
                _context.AuditMovieLog.Add(audit);
            }
        }
    }
}
