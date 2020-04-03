using Microsoft.Extensions.Configuration;
using SevenDays.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SevenDays.DataAccess
{
    public class MovieDataAccess
    {

        private readonly IConfiguration Configuration;

        public MovieDataAccess(IConfiguration configuration)
        {
            Configuration = configuration;
        }
       
        /// <summary>
        /// Createing a new movie
        /// </summary>
        /// <param name="movie">Movie object</param>
        /// <returns>Result object</returns>
        public DBResult<Movie> CreateMovie(Movie movie)
        {
            DBResult<Movie> dbResult = new DBResult<Movie>();

            if (movie == null)
            {
                dbResult.Message = "Invalid movie";
                return dbResult;
            }
            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    db.Movie.Add(movie);
                    db.SaveChanges();
                    dbResult.Success = true;
                    dbResult.Item = movie;
                    // Get movie with ID
                    movie = db.Movie.Where(m => m.Image == movie.Image).FirstOrDefault();
                }                
            }
            catch (Exception ex)
            {
                dbResult.Message = GetMessageError(ex);
            }


            return dbResult;
        }

        /// <summary>
        /// Get one movie
        /// </summary>
        /// <param name="idmovie">Id Movie</param>
        /// <returns>Result object</returns>
        public async Task<DBResult<Movie>> GetMovie(int idMovie)
        {
            DBResult<Movie> dbResult = new DBResult<Movie>();

            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    var movie = await db.Movie.FindAsync(idMovie);
                   
                    if(movie == null)
                    {
                        dbResult.Message = "Movie not found";
                    }
                    else{
                        dbResult.Success = true;
                        dbResult.Item = movie;
                    }
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
