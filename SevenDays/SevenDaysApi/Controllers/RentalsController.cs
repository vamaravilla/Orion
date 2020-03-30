using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SevenDays.Api.Models;

namespace SevenDaysApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly SevenDaysContext _context;

        public RentalsController(SevenDaysContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all user rentals
        /// </summary>
        /// <returns>List of rentals</returns>
        // GET: api/Rentals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rental>>> GetRental()
        {
            int? idUser = GetUserLogged();
            if (idUser == null)
            {
                return Unauthorized(new { message = "Not allowed" });
            }
            return await _context.Rental.Where(r => r.IdUser == idUser).ToListAsync();
        }


        /// <summary>
        /// Crea new rental
        /// </summary>
        /// <param name="rental">Rental Object</param>
        /// <returns>New rental details</returns>
        // POST: api/Rentals
        [HttpPost]
        public async Task<ActionResult<Rental>> PostRental(Rental rental)
        {
            // Only logged user can rent movies
            int? idUser = GetUserLogged();
            if (idUser == null)
            {
                return Unauthorized(new { message = "Not allowed" });
            }

            // Sale must be from current registered user
            rental.IdUser = (int)idUser;
            rental.RentalDate = DateTime.Now;
            // Return date is until 7 days
            rental.ReturnDate = DateTime.Now.AddDays(7);


            //Check Inventary, users can rent only old and available items
            var inventory = await _context.Inventory.FindAsync(rental.IdInventory);
            if (inventory == null || inventory.IsAvailable == false || inventory.IsNew == true)
            {
                return NotFound(new { message = "Inventory not found or not available for rent" });
            }

            // If the operation is successful the item will no longer be available
            inventory.IsAvailable = false;
            _context.Inventory.Update(inventory);

            _context.Rental.Add(rental);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRental", new { id = rental.IdRental }, rental);
        }

        /// <summary>
        /// Return movie from user
        /// </summary>
        ///  /// <param name="rental">Rental object</param>
        /// <returns>Rental Object</returns>
        // PUT: api/rentals/returnmovie
        [HttpPut("returnmovie")]
        public async Task<IActionResult> PutRental(Rental rental)
        {
            // Only Admin users are allowed to perform this action
            if (!IsUserAdminAutenticated())
            {
                return Unauthorized(new { message = "Not allowed" });
            }

            if(rental == null)
            {
                return BadRequest();
            }

            var rentalDb = await _context.Rental.FindAsync(rental.IdRental);
            if(rentalDb == null)
            {
                return NotFound(new { message = "Rent record not found" });
            }

            // Calculate penalty (if apply)
            DateTime today = DateTime.Now;
            // Difference in days, hours, and minutes.
            TimeSpan ts = today - (DateTime)rentalDb.RentalDate;
            // Difference in days.
            int differenceInDays = ts.Days;

            if(differenceInDays > 7)
            {
                // 5 per day of penalty
                rentalDb.Penalty = (decimal)(5.00m * ((decimal)differenceInDays - 7));

            }
            else
            {
                rentalDb.Penalty = 0.000m;
            }

            // Change movie item availability
            var inventory = await _context.Inventory.FindAsync(rentalDb.IdInventory);
            inventory.IsAvailable = true;
            // SAving changes
            try
            {
                _context.Inventory.Update(inventory);
                _context.Rental.Update(rentalDb);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return CreatedAtAction("GetRental", new { id = rental.IdRental }, rentalDb);
        }

        /// <summary>
        /// Remove one specific rental
        /// </summary>
        /// <param name="id">Id rental</param>
        /// <returns>rental deleted</returns>
            // DELETE: api/Rentals/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Rental>> DeleteRental(int id)
        {
            // Only Admin users are allowed to perform this action
            if (!IsUserAdminAutenticated())
            {
                return Unauthorized(new { message = "Not allowed" });
            }
            var rental = await _context.Rental.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }

            _context.Rental.Remove(rental);
            await _context.SaveChangesAsync();

            return rental;
        }

        /// <summary>
        /// Auxiliary method to validate one rental existence
        /// </summary>
        /// <param name="id">Id rental</param>
        /// <returns>Boolean</returns>
        private bool RentalExists(int id)
        {
            return _context.Rental.Any(e => e.IdRental == id);
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
