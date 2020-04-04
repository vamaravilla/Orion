using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Configuration;
using SevenDays.Entities;
using SevenDays.Util;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SevenDays.DataAccess
{
    public class RentalDataAccess
    {

        private readonly IConfiguration Configuration;

        public RentalDataAccess(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        /// <summary>
        /// Get rentals from specific user
        /// </summary>
        /// <param name="idUser">ID User</param>
        /// <returns>Result object</returns>
        public DBResults<Rental> GetRentalsByIdUser(int idUser)
        {
            DBResults<Rental> dbResult = new DBResults<Rental>();

            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    var sales = db.Rental.Where(i => i.IdUser == idUser).ToList();
                    if (sales == null)
                    {
                        dbResult.Message = "No rentals for user";
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
        /// Get rentalfrom specific ID
        /// </summary>
        /// <param name="idRental">ID Rental</param>
        /// <returns>Result object</returns>
        public DBResult<Rental> GetRentalById(int idRental)
        {
            DBResult<Rental> dbResult = new DBResult<Rental>();

            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    var rental = db.Rental.Find(idRental);
                    if (rental == null)
                    {
                        dbResult.Message = "No rental found";
                    }
                    else
                    {
                        dbResult.Success = true;
                        dbResult.Item = rental;
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
        /// Creating a new rental
        /// </summary>
        /// <param name="rental">Rental object</param>
        /// <returns>Result object</returns>
        public DBResult<Rental> CreateRental(Rental rental)
        {
            DBResult<Rental> dbResult = new DBResult<Rental>();

            if (rental == null)
            {
                dbResult.Message = "Invalid rental";
                return dbResult;
            }
            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    db.Rental.Add(rental);
                    db.SaveChanges();
                    dbResult.Success = true;
                    dbResult.Item = rental;
                }                
            }
            catch (Exception ex)
            {
                dbResult.Message = Common.GetMessageError(ex);
            }

            return dbResult;
        }


        /// <summary>
        /// Return rental
        /// </summary>
        /// <param name="id">Rental</param>
        /// <returns>Result object</returns>
        public DBResult<Rental> ReturnRental(int idRental)
        {
            DBResult<Rental> dbResult = new DBResult<Rental>();
            
            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    var rental = db.Rental.Find(idRental);
                    if(rental == null)
                    {
                        dbResult.Message = "Rental not found";
                    }
                    else
                    {
                        // Calculate penalty(if apply)
                         DateTime today = DateTime.Now;
                        // Difference in days, hours, and minutes.
                        TimeSpan ts = today - (DateTime)rental.RentalDate;
                        // Difference in days.
                        int differenceInDays = ts.Days;

                        if (differenceInDays > 7)
                        {
                            // 5 per day of penalty
                            rental.Penalty = (decimal)(5.00m * ((decimal)differenceInDays - 7));
                        }
                        else
                        {
                            rental.Penalty = 0.000m;
                        }


                        db.Rental.Update(rental);
                        db.SaveChanges();
                        dbResult.Success = true;
                        dbResult.Item = rental;
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
