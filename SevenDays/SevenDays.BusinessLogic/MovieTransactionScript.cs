using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SevenDays.Api.Helpers;
using SevenDays.DataAccess;
using SevenDays.Entities;
using SevenDays.Util;
using System;
using System.Threading.Tasks;

namespace SevenDays.BusinessLogic
{
    public class MovieTransactionScript
    {
        private MovieDataAccess movieDataAccess;

        public MovieTransactionScript(IConfiguration configuration)
        {
            movieDataAccess = new MovieDataAccess(configuration);
        }

        
        /// <summary>
        /// Add new movie
        /// </summary>
        /// <param name="movie">Movie object</param>
        /// <returns>Result object</returns>
        public BLResult<Movie> AddMovie(Movie movie)
        {
            DBResult<Movie> dbResult;
            BLResult<Movie> result = new BLResult<Movie>();

            // Validating aditional input data
            if (movie == null || movie.SalePrice == null || movie.RentalPrice == null)
            {
                result.Message = "Invalid data";
                return result;
            }

            // Adding movie
            dbResult = movieDataAccess.CreateMovie(movie);
            if (dbResult.Success)
            {
                result.Success = true;
                result.Item = dbResult.Item;
            }
            else
            {
                result.Message = dbResult.Message;
            }

            return result;
        }

        /// <summary>
        /// Get one movie
        /// </summary>
        /// <param name="movie">Id movie</param>
        /// <returns>Result object</returns>
        public async Task<BLResult<Movie>> GetMovie(int idMovie)
        {
            DBResult<Movie> dbResult;
            BLResult<Movie> result = new BLResult<Movie>();

            // Try to get movie
            dbResult = await movieDataAccess.GetMovie(idMovie);
            if (dbResult.Success)
            {
                result.Success = true;
                result.Item = dbResult.Item;
            }
            else
            {
                result.Message = dbResult.Message;
            }

            return result;
        }

    }
}
