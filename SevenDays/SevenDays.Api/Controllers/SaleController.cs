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
    public class SalesController : ControllerBase
    {

        SaleTransactionScript saleTransactionScript;

        public SalesController(IConfiguration configuration)
        {
            saleTransactionScript = new SaleTransactionScript(configuration);
        }


        /// <summary>
        /// Get sales from user
        /// </summary>
        /// <returns>Result</returns>
        [HttpGet]
        public ActionResult<IEnumerable<Sale>> GetSales()
        {
            int idUser = GetCurrentUser();
            if(idUser == -1)
            {
                return Unauthorized(new { message = "Not allowed. Login." });
            }

            BLResults<Sale> result = saleTransactionScript.GetSalesByUser(idUser);
            if (!result.Success)
            {
                return StatusCode(500, result);
            }

            return Ok(result.Items);
        }

        /// <summary>
        /// Create new sale
        /// </summary>
        /// <param name="sale">Sale object</param>
        /// <returns>New sale details</returns>
        // POST: api/Sales
        [HttpPost]
        public ActionResult<BLResult<Sale>> PostSale(Sale sale)
        {
            // Only logged users are allowed to perform this action
            int idUser = GetCurrentUser();
            if (idUser == -1)
            {
                return Unauthorized(new { message = "Not allowed. Login." });
            }

            sale.IdUser = idUser;
            BLResult<Sale> result = saleTransactionScript.AddSale(sale);

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
