using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SevenDays.Api.Models;
using SevenDays.Api.Services;
using System.Security.Claims;

namespace SevenDays.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SevenDaysContext _context;
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
            _context = userService.GetDBContext();
        }

        /// <summary>
        /// Operation to login user
        /// </summary>
        /// <param name="userParam">Login params</param>
        /// <returns>Result</returns>
        // GET: api/Users/authenticate
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]SimpleUser userParam)
        {
            var user = _userService.Authenticate(userParam.Email, userParam.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of users</returns>
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            if (!IsUserAdminAutenticated())
            {
                return Unauthorized(new { message = "Not allowed" });
            }
            return await _context.User.ToListAsync();
        }

        /// <summary>
        /// Get specific user by id
        /// </summary>
        /// <param name="id">Id User</param>
        /// <returns>User</returns>
        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            if (!IsUserAdminAutenticated())
            {
                return Unauthorized(new { message = "Not allowed" });
            }
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        /// <summary>
        /// Update complete user
        /// </summary>
        /// <param name="id">Id User</param>
        /// <param name="movie">DTO User</param>
        /// <returns>No content</returns>
        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (!IsUserAdminAutenticated())
            {
                return Unauthorized(new { message = "Not allowed" });
            }

            if (id != user.IdUser)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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
        /// Create new user
        /// </summary>
        /// <param name="user">User object</param>
        /// <returns>New movie details</returns>
        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (!IsUserAdminAutenticated())
            {
                return Unauthorized(new { message = "Not allowed" });
            }

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.IdUser }, user);
        }

        /// <summary>
        /// Delete specific user
        /// </summary>
        /// <param name="id">Id User</param>
        /// <returns>User deleted</returns>
        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            if (!IsUserAdminAutenticated())
            {
                return Unauthorized(new { message = "Not allowed" });
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// Give a like
        /// </summary>
        /// <param name="idUser">Id User</param>
        /// <param name="idMovie">Id Movie</param>
        /// <returns>Result</returns>
        [HttpPut("{idUser}/like/movies/{idMovie}")]
        public async Task<ActionResult<User>> PutLike(int idUser, int IdMovie)
        {
            // Only the user is allowed to perform this action
            if (!IsAuthenticatedUser(idUser))
            {
                return Unauthorized(new { message = "Not allowed" });
            }

            var user = await _context.User.FindAsync(idUser);
            var movie = await _context.Movie.FindAsync(IdMovie);
            if (movie == null || user == null)
            {
                return NotFound();
            }

            // Check that it has not been liked before
            var liked = _context.Liked.Where(lk => lk.IdMovie == movie.IdMovie && lk.IdUser == user.IdUser).FirstOrDefault();
            if(liked == null)
            {
                // Give a like
                liked = new Liked()
                {
                    IdUser = user.IdUser,
                    IdMovie = movie.IdMovie
                };

                _context.Liked.Add(liked);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = $"User: {user.IdUser} LIKED the Movie: {movie.IdMovie}" });
        }

        /// <summary>
        /// Remove a like
        /// </summary>
        /// <param name="idUser">Id User</param>
        /// <param name="idMovie">Id Movie</param>
        /// <returns>Result</returns>
        [HttpDelete("{idUser}/like/movies/{idMovie}")]
        public async Task<ActionResult<User>> DeleteLike(int idUser, int IdMovie)
        {
            // Only the user is allowed to perform this action
            if (!IsAuthenticatedUser(idUser))
            {
                return Unauthorized(new { message = "Not allowed" });
            }

            var user = await _context.User.FindAsync(idUser);
            var movie = await _context.Movie.FindAsync(IdMovie);
            if (movie == null || user == null)
            {
                return NotFound();
            }

            // Check that it has not been liked before
            var liked = _context.Liked.Where(lk => lk.IdMovie == movie.IdMovie && lk.IdUser == user.IdUser).FirstOrDefault();
            if (liked != null)
            {
                _context.Liked.Remove(liked);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = $"User: {user.IdUser} removed the LIKE to the Movie: {movie.IdMovie}" });
        }

        /// <summary>
        /// Auxiliary method to validate one movie existence
        /// </summary>
        /// <param name="id">Id User</param>
        /// <returns>Boolean</returns>
        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.IdUser == id);
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
        /// Validate if user is the same
        /// </summary>
        ///  /// <param name="idUser">Id User</param>
        /// <returns>Boolean result</returns>
        private bool IsAuthenticatedUser(int idUser)
        {
            bool isUserAuthenticated = false;
            // Get logged user if exists
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userCompositeId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;

            if (userCompositeId != null)
            {
                var splitUserId = userCompositeId.Split('.');
                // Validating if user is the same
                if (splitUserId != null && splitUserId.Length == 2 && splitUserId[0] == idUser.ToString())
                {
                    isUserAuthenticated = true;
                }
            }
            return isUserAuthenticated;
        }
    }
}
