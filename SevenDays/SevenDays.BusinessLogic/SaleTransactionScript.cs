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
    public class SaleTransactionScript
    {
        private SaleDataAccess saleDataAccess;
        private InventoryDataAccess inventoryDataAccess;
        private MovieDataAccess movieDataAccess;
        public SaleTransactionScript(IConfiguration configuration)
        {

            inventoryDataAccess = new InventoryDataAccess(configuration);
            saleDataAccess = new SaleDataAccess(configuration);
            movieDataAccess = new MovieDataAccess(configuration);
        }


        /// <summary>
        /// Add Sale
        /// </summary>
        /// <param name="sake">Sale object</param>
        /// <returns>Result object</returns>
        public BLResult<Sale> AddSale(Sale sale)
        {
            DBResult<Inventory> inventoryResult;
            DBResult<Sale> saleResult;
            DBResult<Movie> movieResult;
            BLResult<Sale> result = new BLResult<Sale>();


            // Validating input data
            if (sale == null)
            {
                result.Message = "Invalid data";
                return result;
            }

            // Validating inventory
            inventoryResult =  inventoryDataAccess.GetInventoryById(sale.IdInventory);
            if (!inventoryResult.Success)
            {
                result.Message = "Invalid inventory";
                return result;
            }
            // Getting current sale price
            movieResult = movieDataAccess.GetMovieById(inventoryResult.Item.IdMovie);
            if (!movieResult.Success)
            {
                result.Message = $"Error getting price from Movie: {movieResult.Message}";
                return result;
            }

            if (inventoryResult.Item.IsNew == false || inventoryResult.Item.IsAvailable == false)
            {
                result.Message = "Inventory not found or not available";
                return result;
            }

            sale.SalePrice = movieResult.Item.SalePrice;
            sale.SaleDate = DateTime.Now;
            // Adding sale
            saleResult = saleDataAccess.CreateSale(sale);
            if (saleResult.Success)
            {
                result.Success = true;
                result.Item = sale;

                // If the operation is successful the item will no longer be available
                inventoryResult = inventoryDataAccess.SetAvailability(sale.IdInventory, false);
                if (!inventoryResult.Success)
                {
                    result.Message = inventoryResult.Message;
                }
                result.Success = true;

            }
            else
            {
                result.Message = saleResult.Message;
            }
            
            return result;
        }


        /// <summary>
        /// Get sales for specific user
        /// </summary>
        /// <param name="idUser">Id user</param>
        /// <returns>Result object</returns>
        public BLResults<Sale> GetSalesByUser(int idUser)
        {
            DBResults<Sale> dbResult;
            BLResults<Sale> result = new BLResults<Sale>();

            // Search user
            dbResult = saleDataAccess.GetSalesByIdUser(idUser);
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
