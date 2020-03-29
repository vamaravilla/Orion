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
        MoviesOldController movieController;

        [SetUp]
        public void Setup()
        {
            movieController = new MoviesOldController(new SevenDaysContext());
        }

        [Test]
        public async void TestGetMovie()
        {
            //Arrange + Act
            var response = await movieController.GetMovie();

            //Assert
            Assert.IsNotNull(response);
        }
    }
}