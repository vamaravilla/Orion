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
    public class SalesController : ControllerBase
    {
        private readonly SevenDaysContext _context;

        public SalesController(SevenDaysContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all user purchases
        /// </summary>
        /// <returns>List of sales</returns>
        // GET: api/Sales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sale>>> GetSale()
        {
            int? idUser = GetUserLogged();
            if (idUser == null)
            {
                return Unauthorized(new { message = "Not allowed" });
            }

            return await _context.Sale.Where(s => s.IdUser == idUser).ToListAsync();
        }


        /// <summary>
        /// Create new sale
        /// </summary>
        /// <param name="sale">Sale object</param>
        /// <returns>New sale details</returns>
        // POST: api/Sales
        [HttpPost]
        public async Task<ActionResult<Sale>> PostSale(Sale sale)
        {
            // Only logged user can buy
            int? idUser = GetUserLogged();
            if(idUser == null)
            {
                return Unauthorized(new { message = "Not allowed" });
            }

            //Sale must be from current registered user
            sale.IdUser = (int)idUser;
            sale.SaleDate = DateTime.Now;

            //Check Inventary, users can buy only new and available items
            var inventory = await _context.Inventory.FindAsync(sale.IdInventory);
            if (inventory == null || inventory.IsAvailable == false || inventory.IsNew == false)
            {
                return NotFound(new { message = "Inventory not found or not available"});
            }

            // If the operation is successful the item will no longer be available
            inventory.IsAvailable = false;
            _context.Inventory.Update(inventory);

            _context.Sale.Add(sale);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SaleExists(sale.IdInventory))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSale", new { id = sale.IdInventory }, sale);
        }

        /// <summary>
        /// Remove one specific sale
        /// </summary>
        /// <param name="id">Id Sale</param>
        /// <returns>Sale deleted</returns>
        // DELETE: api/Sales/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Sale>> DeleteSale(int id)
        {
            // Only Admin users are allowed to perform this action
            if (!IsUserAdminAutenticated())
            {
                return Unauthorized(new { message = "Not allowed" });
            }
            var sale = await _context.Sale.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            _context.Sale.Remove(sale);
            await _context.SaveChangesAsync();

            return sale;
        }

        /// <summary>
        /// Auxiliary method to validate one movie existence
        /// </summary>
        /// <param name="id">Id Sale</param>
        /// <returns>Boolean</returns>
        private bool SaleExists(int id)
        {
            return _context.Sale.Any(e => e.IdInventory == id);
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
