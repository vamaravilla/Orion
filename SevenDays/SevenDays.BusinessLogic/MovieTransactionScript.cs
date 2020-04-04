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
        public BLResult<Movie> GetMovieById(int idMovie)
        {
            DBResult<Movie> dbResult;
            BLResult<Movie> result = new BLResult<Movie>();

            // Try to get movie
            dbResult = movieDataAccess.GetMovieById(idMovie);
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
        /// Get all movie
        /// </summary>
        /// <returns>Result object</returns>
        public BLResults<Movie> GetMovies()
        {
            DBResults<Movie> dbResult;
            BLResults<Movie> result = new BLResults<Movie>();

            // Try to get all movies
            dbResult = movieDataAccess.GetMovies();
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
        /// Get all movie
        /// </summary>
        /// <returns>Result object</returns>
        public BLFResults<Movie> GetMoviesFiltered(FilterModel filtro)
        {
            DBFResults<Movie> dbResult;
            BLFResults<Movie> result = new BLFResults<Movie>();

            // Try to get all movies
            dbResult = movieDataAccess.GetMoviesFiltered(filtro);
            if (dbResult.Success)
            {
                result.Success = true;
                result.Items = dbResult.Items;
                result.CountNextFilter = dbResult.CountNextFilter;
            }
            else
            {
                result.Message = dbResult.Message;
            }

            return result;
        }

        /// <summary>
        /// Update an User with Patch Document
        /// </summary>
        /// <param name="patchMovie">Patch document</param>
        /// <param name="idMovie">id Movie</param>
        /// <param name="idUser">id User</param>
        /// <returns>Result</returns>
        public async Task<BLResult<Movie>> PatchMovie(JsonPatchDocument<Movie> patchMovie, int idMovie,int idUser)
        {
            DBResult<Movie> dbResult;
            DBResult<AuditMovieLog> dbResultAudit;
            BLResult<Movie> result = new BLResult<Movie>();

            // Validating input data
            if (patchMovie == null)
            {
                result.Message = "Invalid data";
                return result;
            }

            // Validating allowed operations
            var op = patchMovie.Operations.Where(o => o.path.Equals("/Image") || o.path.Equals("/LikesCounter")).Count();
            if (op > 0)
            {
                result.Message = "Patch operation not allowed";
                return result;
            }

            // Patchin user
            dbResult = await movieDataAccess.PatchMovie(patchMovie, idMovie);
            if (dbResult.Success)
            {
                result.Success = true;
                result.Item = dbResult.Item;

                // Adding Audit Log
                dbResultAudit = movieDataAccess.AddAuditLog(dbResult.Item, idUser);
                if (!dbResult.Success)
                {
                    // Only return message
                    result.Message = dbResultAudit.Message;
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
