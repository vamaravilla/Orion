using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Configuration;
using SevenDays.Api.Helpers;
using SevenDays.Entities;
using SevenDays.Util;
using System;
using System.Collections.Generic;
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
                dbResult.Message = Common.GetMessageError(ex);
            }


            return dbResult;
        }

        /// <summary>
        /// Get one movie
        /// </summary>
        /// <param name="idmovie">Id Movie</param>
        /// <returns>Result object</returns>
        public DBResult<Movie> GetMovieById(int idMovie)
        {
            DBResult<Movie> dbResult = new DBResult<Movie>();

            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    var movie = db.Movie.Find(idMovie);
                   
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
                dbResult.Message = Common.GetMessageError(ex);
            }

            return dbResult;
        }

        /// <summary>
        /// Get All movies
        /// </summary>
        /// <returns>Result object</returns>
        public DBResults<Movie> GetMovies()
        {
            DBResults<Movie> dbResult = new DBResults<Movie>();

            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    var movie = db.Movie.Where(m => m.IsAvailable == true).OrderBy(m => m.Title).ToList();

                    dbResult.Success = true;
                    dbResult.Items = movie;
                }
            }
            catch (Exception ex)
            {
                dbResult.Message = Common.GetMessageError(ex);
            }

            return dbResult;
        }

        /// <summary>
        /// Get All movies filtered
        /// </summary>
        /// <returns>Result object</returns>
        public DBFResults<Movie> GetMoviesFiltered(FilterModel filter)
        {
            DBFResults<Movie> dbResult = new DBFResults<Movie>();

            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    // Get filtered movies and count next page
                    dbResult.Items = FilterData(filter, filter,db).ToList();
                    FilterModel nextFilter = filter.Clone() as FilterModel;
                    nextFilter.Page += 1;
                    dbResult.CountNextFilter = FilterData(nextFilter, filter,db).Count();
                    dbResult.Success = true;
                }
            }
            catch (Exception ex)
            {
                dbResult.Message = Common.GetMessageError(ex);
            }

            return dbResult;
        }

        /// <summary>
        /// Complex filter
        /// </summary>
        /// <param name="filterModel">filter new</param>
        /// <param name="filter">old filter</param>
        /// <param name="db">DB conext</param>
        /// <returns>Result enumerable</returns>
        private IEnumerable<Movie> FilterData(FilterModel filterModel,FilterModel filter,SevenDaysContext db)
        {

            // First filter
            IEnumerable<Movie> listaMovies;
            if (filterModel.IncludeInactive)
            {
                listaMovies = db.Movie.ToList();
            }
            else
            {
                listaMovies = db.Movie.Where(m => m.IsAvailable == true).ToList();
            }

            // Filter title
            if (filterModel.Title != null && !string.IsNullOrEmpty(filterModel.Title))
            {
                listaMovies = listaMovies.Where(m => m.Title.Contains(filterModel.Title, StringComparison.InvariantCultureIgnoreCase));
            }

            // Sorting movie list
            switch (filterModel.Sort)
            {
                case "title_desc":
                    listaMovies = listaMovies.OrderByDescending(m => m.Title).ToList();
                    break;
                case "title":
                    listaMovies = listaMovies.OrderBy(m => m.Title).ToList();
                    break;
                case "popularity_desc":
                    listaMovies = listaMovies.OrderByDescending(m => m.LikesCounter).ToList();
                    break;
                case "popularity":
                    listaMovies = listaMovies.OrderBy(m => m.LikesCounter).ToList();
                    break;
                default:
                    listaMovies = listaMovies.OrderBy(m => m.Title).ToList();
                    break;
            }

            // Filtering paging logic  
            listaMovies = listaMovies.Skip((filterModel.Page - 1) * filter.Limit)
                            .Take(filterModel.Limit);

            return listaMovies;
        }

        /// <summary>
        /// Patch Movie
        /// </summary>
        /// <param name="patchMovie">patch document</param>
        /// <param name="idMovie">Id Movie</param>
        /// <returns>Result object</returns>
        public async Task<DBResult<Movie>> PatchMovie(JsonPatchDocument<Movie> patchMovie, int idMovie)
        {
            DBResult<Movie> dbResult = new DBResult<Movie>();

            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    // Get our original object from the database.
                    var movie = await db.Movie.FindAsync(idMovie);

                    if (movie == null)
                    {
                        dbResult.Message = "Movie not found";
                        return dbResult;
                    }

                    // Applying Path to DB Object
                    patchMovie.ApplyTo(movie);
                    db.Movie.Update(movie);
                    await db.SaveChangesAsync();
                    dbResult.Item = movie;
                    dbResult.Success = true;
                }
            }
            catch (Exception ex)
            {
                dbResult.Message = Common.GetMessageError(ex);
            }


            return dbResult;
        }


        /// <summary>
        /// +1 or -1 like
        /// </summary>
        /// <param name="IdMovie">Movie ID</param>
        /// <param name="increment">Increment like</param>
        /// <returns>Result object</returns>
        public DBResult<Movie> IncrementLikesCounter(int IdMovie,int increment)
        {
            DBResult<Movie> dbResult = new DBResult<Movie>();

            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    var movie = db.Movie.Find(IdMovie);
                    if(movie != null)
                    {
                        movie.LikesCounter += increment;

                        db.Movie.Update(movie);
                        db.SaveChangesAsync();
                        dbResult.Success = true;
                        dbResult.Item = movie;
                    }
                    else
                    {
                        dbResult.Message = "Movie not found";
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
        /// +1 or -1 Sotock
        /// </summary>
        /// <param name="IdMovie">Movie ID</param>
        /// <param name="increment">Increment like</param>
        /// <returns>Result object</returns>
        public DBResult<Movie> IncrementStockCounter(int IdMovie, int increment)
        {
            DBResult<Movie> dbResult = new DBResult<Movie>();

            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    var movie = db.Movie.Find(IdMovie);
                    if (movie != null)
                    {
                        movie.Stock += increment;

                        db.Movie.Update(movie);
                        db.SaveChangesAsync();
                        dbResult.Success = true;
                        dbResult.Item = movie;
                    }
                    else
                    {
                        dbResult.Message = "Movie not found";
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
        /// Save audit log for Mavie Table
        /// </summary>
        /// <param name="movie">Movie object edited</param>
        /// <param name="idUser">User</param>
        /// <returns>Result</returns>
        public DBResult<AuditMovieLog> AddAuditLog(Movie movie, int idUser)
        {
            DBResult<AuditMovieLog> dbResult = new DBResult<AuditMovieLog>();
            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    AuditMovieLog audit = new AuditMovieLog()
                    {
                        IdMovie = movie.IdMovie,
                        IdUser = idUser,
                        Title = movie.Title,
                        RentalPrice = movie.RentalPrice,
                        SalePrice = movie.SalePrice,
                        Action = "Update",
                    };
                    db.AuditMovieLog.Add(audit);
                    db.SaveChangesAsync();
                    dbResult.Item = audit;
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
