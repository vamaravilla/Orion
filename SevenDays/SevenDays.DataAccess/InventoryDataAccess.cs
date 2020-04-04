using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Configuration;
using SevenDays.Entities;
using SevenDays.Util;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SevenDays.DataAccess
{
    public class InventoryDataAccess
    {

        private readonly IConfiguration Configuration;

        public InventoryDataAccess(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        /// <summary>
        /// Get inventory from specific movie
        /// </summary>
        /// <param name="idMovie">ID Movie</param>
        /// <returns>Result object</returns>
        public DBResults<Inventory> GetInventoriesByIdMovie(int idMovie)
        {
            DBResults<Inventory> dbResult = new DBResults<Inventory>();

            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    var inventories = db.Inventory.Where(i => i.IdMovie == idMovie).ToList();
                    if (inventories == null)
                    {
                        dbResult.Message = "No inventory for movie";
                    }
                    else
                    {
                        dbResult.Success = true;
                        dbResult.Items = inventories;
                    }
                }
            }
            catch (Exception ex)
            {
                dbResult.Message = Common.GetMessageError(ex);
            }

            return dbResult;
        }


        /// <summary>
        /// Get inventory by id
        /// </summary>
        /// <param name="idMovie">ID Inventory</param>
        /// <returns>Result object</returns>
        public DBResult<Inventory> GetInventoryById(int idInventory)
        {
            DBResult<Inventory> dbResult = new DBResult<Inventory>();

            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    var inventory = db.Inventory.Find(idInventory);
                    if (inventory == null)
                    {
                        dbResult.Message = "Inventory not found";
                    }
                    else
                    {
                        dbResult.Success = true;
                        dbResult.Item = inventory;
                    }
                }
            }
            catch (Exception ex)
            {
                dbResult.Message = Common.GetMessageError(ex);
            }

            return dbResult;
        }


        /// <summary>
        /// Createing a new Inventory
        /// </summary>
        /// <param name="inventory">Inventory</param>
        /// <returns>Result object</returns>
        public DBResult<Inventory> CreateInventory(Inventory inventory)
        {
            DBResult<Inventory> dbResult = new DBResult<Inventory>();

            if (inventory == null)
            {
                dbResult.Message = "Invalid inventory";
                return dbResult;
            }
            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    db.Inventory.Add(inventory);
                    db.SaveChanges();
                    dbResult.Success = true;
                    dbResult.Item = inventory;
                }                
            }
            catch (Exception ex)
            {
                dbResult.Message = Common.GetMessageError(ex);
            }


            return dbResult;
        }

        /// <summary>
        ///  Disable/Enable Inventory (for sale or rental) 
        /// </summary>
        /// <param name="inventory">id Inventory</param>
        /// <returns>Result object</returns>
        public DBResult<Inventory> SetAvailability(int idInventory, bool status)
        {
            DBResult<Inventory> dbResult = new DBResult<Inventory>();

            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    var inventory = db.Inventory.Find(idInventory);
                    if(inventory != null)
                    {
                        inventory.IsAvailable = status;
                        db.Update(inventory);
                        db.SaveChanges();
                        dbResult.Success = true;
                        dbResult.Item = inventory;
                    }
                    
                }
            }
            catch (Exception ex)
            {
                dbResult.Message = Common.GetMessageError(ex);
            }

            return dbResult;
        }



    }
}
