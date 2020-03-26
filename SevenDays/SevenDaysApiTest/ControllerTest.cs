using Microsoft.Extensions.Logging;
using NUnit.Framework;
using SevenDays.Api.Controllers;
using SevenDays.Api.Models;
using System.Collections.Generic;

namespace SevenDaysApiTest
{
    public class Tests
    {
        MovieController movieController;
        ILogger<MovieController> logger;

        [SetUp]
        public void Setup()
        {
            logger = null;
            movieController = new MovieController(logger);
        }

        [Test]
        public void TestGetMovie()
        {
            //Arrange + Act
            IEnumerable<Movie> response = movieController.Get();

            //Assert
            Assert.IsNotNull(response);
        }
    }
}