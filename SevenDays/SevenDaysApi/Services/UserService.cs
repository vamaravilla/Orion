using SevenDays.Api.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SevenDays.Api.Helpers;
using System.Threading.Tasks;

namespace SevenDays.Api.Services
{
    public interface IUserService
    {
        SimpleUser Authenticate(string email, string password);
        SevenDaysContext GetDBContext();
    }

    public class UserService : IUserService
    {
        private readonly SevenDaysContext db;
        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings, SevenDaysContext dbContext)
        {
            _appSettings = appSettings.Value;
            db = dbContext;
        }

        public SimpleUser Authenticate(string email, string password)
        {
            var user = db.User.SingleOrDefault(x => x.Email == email && x.Password == password);
           
            // Return null if user not found
            if (user == null)
                return null;

            // Creating simple user object to add Token  
            SimpleUser simpleUser = new SimpleUser(user.Email, user.IdUser);
  

            // Authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, simpleUser.IdUser.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            simpleUser.Token = tokenHandler.WriteToken(token);

            return simpleUser;
        }

        public SevenDaysContext GetDBContext()
        {
            return db;
        }
    }
}
