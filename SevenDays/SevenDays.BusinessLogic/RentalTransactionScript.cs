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
    public class RentalTransactionScript
    {
        private RentalDataAccess rentalDataAccess;
        private InventoryDataAccess inventoryDataAccess;
        private MovieDataAccess movieDataAccess;
        private UserDataAccess userDataAccess;
        public RentalTransactionScript(IConfiguration configuration)
        {

            inventoryDataAccess = new InventoryDataAccess(configuration);
            rentalDataAccess = new RentalDataAccess(configuration);
            movieDataAccess = new MovieDataAccess(configuration);
            userDataAccess = new UserDataAccess(configuration);
        }


        /// <summary>
        /// Add Rental
        /// </summary>
        /// <param name="sake">Rental object</param>
        /// <returns>Result object</returns>
        public BLResult<Rental> AddRental(Rental rental)
        {
            DBResult<Inventory> inventoryResult;
            DBResult<Rental> rentalResult;
            DBResult<Movie> movieResult;
            BLResult<Rental> result = new BLResult<Rental>();


            // Validating input data
            if (rental == null)
            {
                result.Message = "Invalid data";
                return result;
            }

            // Validating inventory
            inventoryResult =  inventoryDataAccess.GetInventoryById(rental.IdInventory);
            if (!inventoryResult.Success)
            {
                result.Message = "Invalid inventory";
                return result;
            }
            // Getting current rental price
            movieResult = movieDataAccess.GetMovieById(inventoryResult.Item.IdMovie);
            if (!movieResult.Success)
            {
                result.Message = $"Error getting price from Movie: {movieResult.Message}";
                return result;
            }

            if (inventoryResult.Item.IsNew == true || inventoryResult.Item.IsAvailable == false)
            {
                result.Message = "Inventory not found or not available for rent";
                return result;
            }

            rental.RentalPrice = movieResult.Item.RentalPrice;
            rental.RentalDate = DateTime.Now;
            rental.ReturnDate = DateTime.Now.AddDays(7);
            // Adding Rental
            rentalResult = rentalDataAccess.CreateRental(rental);
            if (rentalResult.Success)
            {
                result.Success = true;
                result.Item = rental;

                // If the operation is successful the item will no longer be available
                inventoryResult = inventoryDataAccess.SetAvailability(rental.IdInventory, false);
                if (!inventoryResult.Success)
                {
                    result.Message = inventoryResult.Message;
                }
                result.Success = true;

            }
            else
            {
                result.Message = rentalResult.Message;
            }
            
            return result;
        }


        /// <summary>
        /// Get rentals for specific user
        /// </summary>
        /// <param name="idUser">Id user</param>
        /// <returns>Result object</returns>
        public BLResults<Rental> GetRentalsByUser(int idUser)
        {
            DBResults<Rental> dbResult;
            BLResults<Rental> result = new BLResults<Rental>();

            // Search user
            dbResult = rentalDataAccess.GetRentalsByIdUser(idUser);
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

        /// <summary>
        /// Return rental
        /// </summary>
        /// <param name="rental">Rental Object</param>
        /// <returns>Result object</returns>
        public BLResult<Rental> ReturnRental(Rental rental,int currentuser)
        {
            DBResult<Inventory> inventoryResult;
            DBResult<Rental> dbResult;
            BLResult<Rental> result = new BLResult<Rental>();

            // Validating inventory
            inventoryResult = inventoryDataAccess.GetInventoryById(rental.IdInventory);
            if (!inventoryResult.Success)
            {
                result.Message = "Invalid inventory";
                return result;
            }
            // Getting rental
            dbResult = rentalDataAccess.GetRentalById(rental.IdRental);
            if (!dbResult.Success)
            {
                result.Message = "Invalid Rental";
                return result;
            }
            // validating user
            if(dbResult.Item.IdUser != currentuser)
            {
                result.Message = "Now allowed for corrent user.";
                return result;
            }

            // Updating rental
            dbResult = rentalDataAccess.ReturnRental(rental.IdRental);
            if (dbResult.Success)
            {
                result.Success = true;
                result.Item = dbResult.Item;

                // If the operation is successful the item will be available
                inventoryResult = inventoryDataAccess.SetAvailability(rental.IdInventory, true);
                if (!inventoryResult.Success)
                {
                    result.Message = inventoryResult.Message;
                }
            }
            else
            {
                result.Message = dbResult.Message;
            }

            return result;
        }


    }
}
