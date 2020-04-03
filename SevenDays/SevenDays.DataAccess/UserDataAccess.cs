using Microsoft.Extensions.Configuration;
using SevenDays.Entities;
using System;
using System.Linq;

namespace SevenDays.DataAccess
{
    public class UserDataAccess
    {

        private readonly IConfiguration Configuration;

        public UserDataAccess(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        /// <summary>
        /// Get the user with a specific email
        /// </summary>
        /// <param name="idUser">ID User</param>
        /// <returns>Result object</returns>
        public DBResult<User> GetUser(string email)
        {
            DBResult<User> dbResult = new DBResult<User>()
            {
                Success = false
            };

            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    var user = db.User.Where(u => u.Email == email).FirstOrDefault();
                    if (user == null)
                    {
                        dbResult.Message = "User not found";
                    }
                    else
                    {
                        dbResult.Success = true;
                        dbResult.Item = user;
                    }
                }
            }
            catch(Exception ex)
            {
                dbResult.Message = GetMessageError(ex);
            }
           

            return dbResult;
        }

        /// <summary>
        /// Createing a new user
        /// </summary>
        /// <param name="user">User object</param>
        /// <returns>Result object</returns>
        public DBResult<User> CreateUser(User user)
        {
            DBResult<User> dbResult = new DBResult<User>();
            dbResult.Success = false;

            if (user == null)
            {
                dbResult.Message = "Invalid user";
                return dbResult;
            }
            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    db.User.Add(user);
                    db.SaveChanges();
                    dbResult.Success = true;
                    dbResult.Item = user;
                    // Get user with ID
                    user = db.User.Where(u => u.Email == user.Email).FirstOrDefault();
                }                
            }
            catch (Exception ex)
            {
                dbResult.Message = GetMessageError(ex);
            }


            return dbResult;
        }

        /// <summary>
        /// Build the error detail
        /// </summary>
        /// <param name="ex">Exfeption</param>
        /// <returns>Error detail string</returns>
        private string GetMessageError(Exception ex)
        {
            if (ex == null) return String.Empty;
            if(ex.InnerException != null)
            {
                return $"Data Access error: {ex.Message}, Inner: {ex.InnerException.Message}";
            }
            else
            {
                return $"Data Access error: {ex.Message}";
            }
        }
    }
}
