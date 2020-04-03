using Microsoft.EntityFrameworkCore;
using SevenDays.DataAccess;
using SevenDays.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenDays.Test
{
    class UserTest
    {
        public UserTest()
        {
            //Inital data
            LoadTestData();
        }

       
        /// Loading data for testing
        /// </summary>
        private void LoadTestData()
        {
            var options = new DbContextOptionsBuilder<SevenDaysContext>()
                .UseInMemoryDatabase(databaseName: "SevenDays")
                .Options;

            // Insert seed data into the database using one instance of the context
            //using (var context = new SevenDaysContext(options))
            //{
            //    context.User.Add(new User()
            //    {
            //        IdUser = 1,
            //        Email="juan@gmail.com",
            //        Password = "x1oetsad7pdw1mew",
            //        IsActive = true,
            //        CreatedDate = DateTime.Now,
            //        Name = "Juan",
            //        Profile = 2
            //    });
            //}

        }
    }
}
