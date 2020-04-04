using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SevenDays.Api.Helpers;
using SevenDays.DataAccess;
using SevenDays.Entities;
using SevenDays.Util;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SevenDays.BusinessLogic
{
    public class InventoryTransactionScript
    {
        private InventoryDataAccess inventoryDataAccess;
        private MovieDataAccess movieDataAccess;
        public InventoryTransactionScript(IConfiguration configuration)
        {

            inventoryDataAccess = new InventoryDataAccess(configuration);
            movieDataAccess = new MovieDataAccess(configuration);
        }


        /// <summary>
        /// Add new inventory
        /// </summary>
        /// <param name="liked">Liked object</param>
        /// <returns>Result object</returns>
        public BLResult<Inventory> AddInventory(Inventory inventory)
        {
            DBResult<Inventory> dbResult;
            DBResult<Movie> movieResult;
            BLResult<Inventory> result = new BLResult<Inventory>();


            // Validating input data
            if (inventory == null)
            {
                result.Message = "Invalid data";
                return result;
            }

            // Validating movie
            movieResult =  movieDataAccess.GetMovieById(inventory.IdMovie);
            if (!movieResult.Success)
            {
                result.Message = "Invalid movie";
                return result;
            }

            // Adding Inventory
            dbResult = inventoryDataAccess.CreateInventory(inventory);
            if (dbResult.Success)
            {
                result.Success = true;
                result.Item = inventory;

                // Increment Stock counter
                movieResult = movieDataAccess.IncrementStockCounter(inventory.IdMovie, 1);
                if (!movieResult.Success)
                {
                    result.Success = false;
                    result.Message = movieResult.Message;
                }
            }
            else
            {
                result.Message = dbResult.Message;
            }
            
            return result;
        }


        /// <summary>
        /// Get invetories by id movie
        /// </summary>
        /// <param name="idMovie">Id Movie</param>
        /// <returns>Result object</returns>
        public BLResults<Inventory> GetInventoryByIdMovie(int idMovie)
        {
            DBResults<Inventory> dbResult;
            BLResults<Inventory> result = new BLResults<Inventory>();

            // Search user
            dbResult = inventoryDataAccess.GetInventoriesByIdMovie(idMovie);
            if (dbResult.Success)
            {
                result.Success = true;
                result.Items = dbResult.Items;
            }
            else
            {
                result.Message = dbResult.Message;
            }

            return result;
        }

        

    }
}
