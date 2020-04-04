using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Configuration;
using SevenDays.Entities;
using SevenDays.Util;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SevenDays.DataAccess
{
    public class SaleDataAccess
    {

        private readonly IConfiguration Configuration;

        public SaleDataAccess(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        /// <summary>
        /// Get purchases from specific user
        /// </summary>
        /// <param name="idUser">ID User</param>
        /// <returns>Result object</returns>
        public DBResults<Sale> GetSalesByIdUser(int idUser)
        {
            DBResults<Sale> dbResult = new DBResults<Sale>();

            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    var sales = db.Sale.Where(i => i.IdUser == idUser).ToList();
                    if (sales == null)
                    {
                        dbResult.Message = "No sales for user";
                    }
                    else
                    {
                        dbResult.Success = true;
                        dbResult.Items = sales;
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
        /// Createing a new Sale
        /// </summary>
        /// <param name="sale">Sale object</param>
        /// <returns>Result object</returns>
        public DBResult<Sale> CreateSale(Sale sale)
        {
            DBResult<Sale> dbResult = new DBResult<Sale>();

            if (sale == null)
            {
                dbResult.Message = "Invalid sale";
                return dbResult;
            }
            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    db.Sale.Add(sale);
                    db.SaveChanges();
                    dbResult.Success = true;
                    dbResult.Item = sale;
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
