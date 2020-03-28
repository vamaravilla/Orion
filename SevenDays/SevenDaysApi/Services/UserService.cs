using SevenDays.Api.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SevenDays.Api.Entities;
using SevenDays.Api.Helpers;

namespace SevenDays.Api.Services
{
    public interface IUserService
    {
        SimpleUser Authenticate(string email, string password);
        IEnumerable<SimpleUser> GetAll();
    }

    public class UserService : IUserService
    {

        // Users hardcoded for test only
        private List<SimpleUser> _users = new List<SimpleUser>
        {
            new SimpleUser { Id = 1, Name = "Alex", Email = "alex@gmail.com", Password = "test" }
        };

        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public SimpleUser Authenticate(string email, string password)
        {
            var user = _users.SingleOrDefault(x => x.Email == email && x.Password == password);

            // Return null if user not found
            if (user == null)
                return null;

            // Authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            return user;
        }

        public IEnumerable<SimpleUser> GetAll()
        {
            return _users;
        }
    }
}
