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
    public class RentalsController : ControllerBase
    {

        RentalTransactionScript rentalTransactionScript;

        public RentalsController(IConfiguration configuration)
        {
            rentalTransactionScript = new RentalTransactionScript(configuration);
        }


        /// <summary>
        /// Get rentals from user
        /// </summary>
        /// <returns>Result</returns>
        [HttpGet]
        public ActionResult<IEnumerable<Rental>> GetRental()
        {
            int idUser = GetCurrentUser();
            if(idUser == -1)
            {
                return Unauthorized(new { message = "Not allowed. Login." });
            }

            BLResults<Rental> result = rentalTransactionScript.GetRentalsByUser(idUser);
            if (!result.Success)
            {
                return StatusCode(500, result);
            }

            return Ok(result.Items);
        }

        /// <summary>
        /// Create new rental
        /// </summary>
        /// <param name="rentla">Rental object</param>
        /// <returns>New rental details</returns>
        // POST: api/Rentals
        [HttpPost]
        public ActionResult<BLResult<Rental>> PostRental(Rental rental)
        {
            // Only logged users are allowed to perform this action
            int idUser = GetCurrentUser();
            if (idUser == -1)
            {
                return Unauthorized(new { message = "Not allowed. Login." });
            }

            rental.IdUser = idUser;
            BLResult<Rental> result = rentalTransactionScript.AddRental(rental);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);

        }

        /// <summary>
        /// Return movie from user
        /// </summary>
        ///  /// <param name="rental">Rental object</param>
        /// <returns>Rental Object</returns>
        // PUT: api/rentals/returnmovie
        [HttpPut("returnmovie")]
        public IActionResult PutRental(Rental rental)
        {
            // Only logged users are allowed to perform this action
            int idUser = GetCurrentUser();
            if (idUser == -1)
            {
                return Unauthorized(new { message = "Not allowed. Login." });
            }

            rental.IdUser = idUser;
            BLResult<Rental> result = rentalTransactionScript.ReturnRental(rental,idUser);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
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
