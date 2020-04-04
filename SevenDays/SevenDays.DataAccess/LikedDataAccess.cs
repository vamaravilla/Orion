using Microsoft.Extensions.Configuration;
using SevenDays.Entities;
using SevenDays.Util;
using System;
using System.Linq;

namespace SevenDays.DataAccess
{
    public class LikedDataAccess
    {

        private readonly IConfiguration Configuration;

        public LikedDataAccess(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Check if a like exists
        /// </summary>
        /// <param name="liked"></param>
        /// <returns></returns>
        public bool HasALike(Liked liked)
        {
            using (SevenDaysContext db = new SevenDaysContext(Configuration))
            {
                var likedDb = db.Liked.Where(lk => lk.IdMovie == liked.IdMovie && lk.IdUser == liked.IdUser).FirstOrDefault();
                if(likedDb == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        /// <summary>
        /// Createing a new like
        /// </summary>
        /// <param name="liked">Liked object</param>
        /// <returns>Result object</returns>
        public DBResult<Liked> CreateLiked(Liked liked)
        {
            DBResult<Liked> dbResult = new DBResult<Liked>();

            if (liked == null)
            {
                dbResult.Message = "Invalid Like";
                return dbResult;
            }
            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    db.Liked.Add(liked);
                    db.SaveChanges();
                    dbResult.Success = true;
                    dbResult.Item = liked;
                }                
            }
            catch (Exception ex)
            {
                dbResult.Message = Common.GetMessageError(ex);
            }
            return dbResult;
        }

        /// <summary>
        /// Remove like
        /// </summary>
        /// <param name="liked">Liked object</param>
        /// <returns>Result object</returns>
        public DBResult<Liked> DeleteLiked(Liked liked)
        {
            DBResult<Liked> dbResult = new DBResult<Liked>();
            // Validating input object
            if (liked == null)
            {
                dbResult.Message = "Invalid Like";
                return dbResult;
            }
            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    liked = db.Liked.Where(l => l.IdUser == liked.IdUser && l.IdMovie == liked.IdMovie).FirstOrDefault();
                    // Remove like from database
                    if(liked != null)
                    {
                        db.Liked.Remove(liked);
                        db.SaveChanges();
                        dbResult.Message = "YES"; //Mark to decrement likes counter
                    }
                    dbResult.Success = true;
                    dbResult.Item = liked;
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
