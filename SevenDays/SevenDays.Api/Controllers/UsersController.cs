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
using Microsoft.AspNetCore.JsonPatch;

namespace SevenDays.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        UserTransactionScript userTransactionScript;
        LikedTransactionScript likedTransactionScript;

        public UsersController(IConfiguration configuration)
        {
            userTransactionScript = new UserTransactionScript(configuration);
            likedTransactionScript = new LikedTransactionScript(configuration);
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
        /// Get user from token
        /// </summary>
        /// <param name="id">Id Movie</param>
        /// <returns>User</returns>
        // GET: api/Users/logged
        [HttpGet]
        public ActionResult<Movie> GetUser()
        {
            int idUser = GetCurrentUser();
            if(idUser == -1)
            {
                return NotFound(new { message = "Invalid JWT" });
            }

            BLResult<User> result = userTransactionScript.GetUserById(idUser);

            if (!result.Success)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(result.Item);
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
        /// Partial update of one user
        /// </summary>
        /// <param name="id">Id User</param>
        /// <param name="patchUser">Patch operations</param>
        /// <returns>Result</returns>
        // PATCH: api/Users/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchUser(int id, [FromBody]JsonPatchDocument<User> patchUser)
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

            BLResult<User> result = await userTransactionScript.PatchUser(patchUser,id);
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
        [HttpPut("{idUser}/like/movies/{idMovie}")]
        public ActionResult<User> PutLike(int idUser, int idMovie)
        {
            // Only the user is allowed to perform this action
            if (!IsAuthenticatedUser(idUser))
            {
                return Unauthorized(new { message = "Not allowed" });
            }

            Liked liked = new Liked()
            {
                IdMovie = idMovie,
                IdUser = idUser
            };

            BLResult<Liked> blResult = likedTransactionScript.PutLike(liked);
            if (!blResult.Success)
            {
                return StatusCode(500, blResult);
            }

            return Ok(new { message = $"User: {liked.IdUser} LIKED the Movie: {liked.IdMovie}" });
        }

        
        /// <summary>
        /// Remove a like
        /// </summary>
        /// <param name="idUser">Id User</param>
        /// <param name="idMovie">Id Movie</param>
        /// <returns>Result</returns>
        [HttpDelete("{idUser}/like/movies/{idMovie}")]
        public ActionResult<User> DeleteLike(int idUser, int idMovie)
        {
            // Only the user is allowed to perform this action
            if (!IsAuthenticatedUser(idUser))
            {
                return Unauthorized(new { message = "Not allowed" });
            }

            Liked liked = new Liked()
            {
                IdMovie = idMovie,
                IdUser = idUser
            };

            BLResult<Liked> blResult = likedTransactionScript.RemoveLike(liked);
            if (!blResult.Success)
            {
                return StatusCode(500, blResult);
            }

            return Ok(new { message = $"User: {liked.IdUser} removed the LIKE to the Movie: {liked.IdMovie}" });
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
