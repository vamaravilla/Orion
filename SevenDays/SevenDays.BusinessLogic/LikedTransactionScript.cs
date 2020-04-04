using Microsoft.Extensions.Configuration;
using SevenDays.DataAccess;
using SevenDays.Entities;
using System.Threading.Tasks;

namespace SevenDays.BusinessLogic
{
    public class LikedTransactionScript
    {
        private LikedDataAccess likedDataAccess;
        private MovieDataAccess movieDataAccess;
        private UserDataAccess userDataAccess;

        public LikedTransactionScript(IConfiguration configuration)
        {
            likedDataAccess = new LikedDataAccess(configuration);
            movieDataAccess = new MovieDataAccess(configuration);
            userDataAccess = new UserDataAccess(configuration);
        }

       
        /// <summary>
        /// Put a like
        /// </summary>
        /// <param name="liked">Liked object</param>
        /// <returns>Result object</returns>
        public BLResult<Liked> PutLike(Liked liked)
        {
            DBResult<Liked> dbResult;
            DBResult<Movie> movieResult;
            DBResult<User> userResult;
            BLResult<Liked> result = new BLResult<Liked>();


            // Validating input data
            if (liked == null)
            {
                result.Message = "Invalid data";
                return result;
            }

            // Validating user
            userResult = userDataAccess.GetUserById(liked.IdUser);
            if (!userResult.Success)
            {
                result.Message = "Invalid user";
                return result;
            }

            // Check that it has not been liked before
            if (!likedDataAccess.HasALike(liked))
            {
                // Adding like
                dbResult = likedDataAccess.CreateLiked(liked);
                if (dbResult.Success)
                {
                    result.Success = true;
                    result.Item = liked;

                    // Increment like counter
                    movieResult = movieDataAccess.IncrementLikesCounter(liked.IdMovie, 1);
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
            }
            else
            {
                // If it exists continue
                result.Success = true;
                result.Item = liked;
            }

            return result;
        }

        /// <summary>
        /// Remove a like
        /// </summary>
        /// <param name="liked">Liked object</param>
        /// <returns>Result object</returns>
        public BLResult<Liked> RemoveLike(Liked liked)
        {
            DBResult<Liked> dbResult;
            DBResult<Movie> movieResult;
            BLResult<Liked> result = new BLResult<Liked>();

            // Validating input data
            if (liked == null)
            {
                result.Message = "Invalid data";
                return result;
            }

            // Removing like
            dbResult = likedDataAccess.DeleteLiked(liked);
            if (dbResult.Success)
            {
                result.Success = true;
                result.Item = liked;

                if (dbResult.Message.Equals("YES"))
                {

                    // Decrement like counter
                    movieResult = movieDataAccess.IncrementLikesCounter(liked.IdMovie, -1);
                    if (!movieResult.Success)
                    {
                        result.Success = false;
                        result.Message = movieResult.Message;
                    }
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
