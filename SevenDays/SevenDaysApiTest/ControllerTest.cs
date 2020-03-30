using Microsoft.Extensions.Logging;
using NUnit.Framework;
using SevenDays.Api.Controllers;
using SevenDays.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SevenDaysApiTest
{
    public class Tests
    {
        MoviesController movieController;

        [SetUp]
        public void Setup()
        {
            movieController = new MoviesController(new SevenDaysContext());
        }

        [Test]
        public async void TestGetMovie()
        {
            //Arrange + Act
            var response = await movieController.GetMovie(true);

            //Assert
            Assert.IsNotNull(response);
        }
    }
}