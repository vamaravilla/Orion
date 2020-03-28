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

        [SetUp]
        public void Setup()
        {
            movieController = new MovieController(new SevenDaysContext());
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