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
    public class InventoriesController : ControllerBase
    {
        private readonly SevenDaysContext _context;

        public InventoriesController(SevenDaysContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all the inventory
        /// </summary>
        /// <returns>List of invetories</returns>
        // GET: api/Inventories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetInventory()
        {
            return await _context.Inventory.ToListAsync();
        }

        /// <summary>
        /// Get inventary of specific movie
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("movies/{id}")]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetMovieInventory(int id)
        {
            return await _context.Inventory.Where(inv => inv.IdMovie == id).ToListAsync();
        }

        /// <summary>
        /// Get specific inventory by Id
        /// </summary>
        /// <param name="id">id Inventory</param>
        /// <returns>Inventory</returns>
        // GET: api/Inventories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Inventory>> GetInventory(int id)
        {
            var inventory = await _context.Inventory.FindAsync(id);

            if (inventory == null)
            {
                return NotFound();
            }

            return inventory;
        }

        /// <summary>
        /// Edit complete Inventory
        /// </summary>
        /// <param name="id">Id Inventory</param>
        /// <param name="inventory">Inventory Object</param>
        /// <returns>No content</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInventory(int id, Inventory inventory)
        {
            // Only Admin users are allowed to perform this action
            if (!IsUserAdminAutenticated())
            {
                return Unauthorized(new { message = "Not allowed" });
            }

            if (id != inventory.IdInventory)
            {
                return BadRequest();
            }

            _context.Entry(inventory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventoryExists(id))
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
        /// Create one Inventory
        /// </summary>
        /// <param name="inventory">Inventory object</param>
        /// <returns>New inventory details</returns>
        [HttpPost]
        public async Task<ActionResult<Inventory>> PostInventory(Inventory inventory)
        {
            // Only Admin users are allowed to perform this action
            if (!IsUserAdminAutenticated())
            {
                return Unauthorized(new { message = "Not allowed" });
            }
            _context.Inventory.Add(inventory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInventory", new { id = inventory.IdInventory }, inventory);
        }

        /// <summary>
        /// Remove one specific Inventory
        /// </summary>
        /// <param name="id">Id Inventory</param>
        /// <returns>Deleted inventory</returns>
        // DELETE: api/Inventories/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Inventory>> DeleteInventory(int id)
        {
            // Only Admin users are allowed to perform this action
            if (!IsUserAdminAutenticated())
            {
                return Unauthorized(new { message = "Not allowed" });
            }

            var inventory = await _context.Inventory.FindAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }

            _context.Inventory.Remove(inventory);
            await _context.SaveChangesAsync();

            return inventory;
        }

        /// <summary>
        /// Auxiliary method to validate one movie existence
        /// </summary>
        /// <param name="id">Id Inventory</param>
        /// <returns>Boolean</returns>
        private bool InventoryExists(int id)
        {
            return _context.Inventory.Any(e => e.IdInventory == id);
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
