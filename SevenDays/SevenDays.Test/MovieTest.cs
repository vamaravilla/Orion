using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SevenDays.Api.Helpers;
using SevenDays.DataAccess;
using SevenDays.Entities;
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

        /*
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
        */

        /// <summary>
        /// Loading data for testing
        /// </summary>
        private void LoadTestData()
        {
          

        }

    }
}