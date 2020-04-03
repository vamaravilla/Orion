using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SevenDays.BusinessLogic;
using SevenDays.Entities;
using Microsoft.Extensions.Configuration;

namespace SevenDays.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        UserTransactionScript userTransactionScript;

        public UsersController(IConfiguration configuration)
        {
            userTransactionScript = new UserTransactionScript(configuration);
        }

        /// <summary>
        /// Operation to login user
        /// </summary>
        /// <param name="userParam">Login params</param>
        /// <returns>Result</returns>
        // GET: api/Users/authenticate
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public ActionResult<BLResult<SimpleUser>> Authenticate([FromBody]SimpleUser userParam)
        {
            BLResult<SimpleUser> result = userTransactionScript.Authenticate(userParam);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }




        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="user">User object</param>
        /// <returns>New movie details</returns>
        // POST: api/Users
        [AllowAnonymous]
        [HttpPost]
        public ActionResult<BLResult<User>> PostUser(User user)
        {
            BLResult<User> result = userTransactionScript.AddUser(user);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);

        }



        /// <summary>
        /// Give a like
        /// </summary>
        /// <param name="idUser">Id User</param>
        /// <param name="idMovie">Id Movie</param>
        /// <returns>Result</returns>
        /*
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

                // Add one like to Likes counter in Movie Object
                movie.LikesCounter++;

                _context.Movie.Update(movie);
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
                // Remove one like to Likes counter in Movie Object
                movie.LikesCounter--;

                _context.Movie.Update(movie);
                _context.Liked.Remove(liked);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = $"User: {user.IdUser} removed the LIKE to the Movie: {movie.IdMovie}" });
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
        */
    }
}
