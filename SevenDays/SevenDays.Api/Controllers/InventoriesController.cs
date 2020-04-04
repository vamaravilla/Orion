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
    public class InventoriesController : ControllerBase
    {

        InventoryTransactionScript inventoryTransactionScript;

        public InventoriesController(IConfiguration configuration)
        {
            inventoryTransactionScript = new InventoryTransactionScript(configuration);
        }


        /// <summary>
        /// Get inventary of specific movie
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("movies/{id}")]
        public ActionResult<IEnumerable<Inventory>> GetMovieInventory(int id)
        {
            BLResults<Inventory> result = inventoryTransactionScript.GetInventoryByIdMovie(id);

            if (!result.Success)
            {
                return StatusCode(500, result);
            }

            return Ok(result.Items);
        }

        /// <summary>
        /// Create new inventory
        /// </summary>
        /// <param name="inventory">Inventory object</param>
        /// <returns>New inventory details</returns>
        // POST: api/Users
        [HttpPost]
        public  ActionResult<BLResult<User>> PostInventory(Inventory inventory)
        {
            // Only Admin users are allowed to perform this action
            if (!IsUserAdminAutenticated())
            {
                return Unauthorized(new { message = "Not allowed" });
            }

            BLResult<Inventory> result = inventoryTransactionScript.AddInventory(inventory);

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
       
        
    }
}
