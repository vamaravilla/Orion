using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SevenDays.Api.Controllers;
using SevenDays.Api.Helpers;
using SevenDays.Api.Models;
using System.Collections.Generic;
using Xunit;


namespace SevenDaysApiTest
{
    public class MovieTest
    {

        public MovieTest()
        {
            //Inital data
            LoadTestData();
        }

        [Fact]
        public async void TestGetOrderedMovies()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SevenDaysContext>()
                .UseInMemoryDatabase(databaseName: "SevenDays")
                .Options;

            // Use a clean instance of the context to run the test
            using (var context = new SevenDaysContext(options))
            {
                // Act
                MoviesController movieController = new MoviesController(context);
                
                ActionResult<IEnumerable<Movie>> movies = await movieController.GetMovie(null, "title_desc");

                // Assert
                Assert.True(movies.Value != null);

            }

        }

        [Fact]
        public async void TestPagingMovies()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<SevenDaysContext>()
                .UseInMemoryDatabase(databaseName: "SevenDays")
                .Options;

            // Use a clean instance of the context to run the test
            using (var context = new SevenDaysContext(options))
            {
                // Act
                MoviesController movieController = new MoviesController(context);
                FilterModel filter = new FilterModel()
                {
                    Title = string.Empty,
                    IncludeInactive = false
                };
               ActionResult<PagedCollectionResponse<Movie>> movies =  movieController.GetFilteredMovie(filter);

                // Assert
                Assert.True(movies.Value != null);

            }

        }


        /// <summary>
        /// Loading data for testing
        /// </summary>
        private void LoadTestData()
        {
            var options = new DbContextOptionsBuilder<SevenDaysContext>()
                .UseInMemoryDatabase(databaseName: "SevenDays")
                .Options;

            // Insert seed data into the database using one instance of the context
            using (var context = new SevenDaysContext(options))
            {
                context.Movie.Add(new Movie()
                {
                    IdMovie = 1,
                    Title = "Movie A",
                    Description = "Movie A",
                    Image = "",
                    SalePrice = 20.000m,
                    RentalPrice = 5.000m,
                    Stock = 2,
                    LikesCounter = 1,
                });
                context.Movie.Add(new Movie()
                {
                    IdMovie = 2,
                    Title = "Movie B",
                    Description = "Movie B",
                    Image = "",
                    SalePrice = 20.000m,
                    RentalPrice = 5.000m,
                    Stock = 4,
                    LikesCounter = 4,
                });
                context.Movie.Add(new Movie()
                {
                    IdMovie = 3,
                    Title = "Movie C",
                    Description = "Movie C",
                    Image = "",
                    SalePrice = 20.000m,
                    RentalPrice = 5.000m,
                    Stock = 10,
                    LikesCounter = 3,
                });
                context.Movie.Add(new Movie()
                {
                    IdMovie = 4,
                    Title = "Movie D",
                    Description = "Movie D",
                    Image = "",
                    SalePrice = 20.000m,
                    RentalPrice = 5.000m,
                    Stock = 7,
                    LikesCounter = 15,
                });
                context.Movie.Add(new Movie()
                {
                    IdMovie = 5,
                    Title = "Movie E",
                    Description = "Movie E",
                    Image = "",
                    SalePrice = 20.000m,
                    RentalPrice = 5.000m,
                    Stock = 1,
                    LikesCounter = 5,
                });
                context.SaveChanges();
            }

        }

    }
}