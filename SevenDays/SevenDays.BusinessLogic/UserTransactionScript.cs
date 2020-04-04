using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SevenDays.Api.Helpers;
using SevenDays.DataAccess;
using SevenDays.Entities;
using SevenDays.Util;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SevenDays.BusinessLogic
{
    public class UserTransactionScript
    {
        private UserDataAccess userDataAccess;
        private PasswordHasher passwordHasher;
        private UserService userService;
        public UserTransactionScript(IConfiguration configuration)
        {
            passwordHasher = new PasswordHasher(Options.Create(new HashingOptions()));
            userService = new UserService(Options.Create(new AppSettings()));
            userDataAccess = new UserDataAccess(configuration);
        }

        /// <summary>
        /// Authenticate user and generate JWT
        /// </summary>
        /// <param name="simpleUser">Simple user object</param>
        /// <returns>Result object</returns>
        public BLResult<SimpleUser> Authenticate(SimpleUser simpleUser)
        {
            BLResult<SimpleUser> result = new BLResult<SimpleUser>();

            // Validating input data
            if (simpleUser == null)
            {
                result.Message = "Invalid data";
                return result;
            }

            // Getting user from database
            DBResult<User> dbResult = userDataAccess.GetUser(simpleUser.Email);
            if (dbResult.Success == true)
            {
                // Valdiating user password   
                bool verified, needsUpgrade;
                (verified,needsUpgrade) = passwordHasher.Check(dbResult.Item.Password, simpleUser.Password);

                if (verified && needsUpgrade == false)
                {
                    // Generate Token
                    simpleUser.Token = userService.GenerateToken(dbResult.Item.IdUser, dbResult.Item.Profile);
                    // We should not return the password
                    simpleUser.Password = string.Empty;
                    simpleUser.Profile = dbResult.Item.Profile;
                    simpleUser.IdUser = dbResult.Item.IdUser;
                    result.Success = true;
                    result.Item = simpleUser;
                }
                else
                {
                    result.Message = "Invalid Password";
                }

            }
            else
            {
                result.Message = dbResult.Message;
            }
            return result;
        }

        /// <summary>
        /// Add new user
        /// </summary>
        /// <param name="user">User object</param>
        /// <returns>Result object</returns>
        public BLResult<User> AddUser(User user)
        {
            DBResult<User> dbResult;
            BLResult<User> result = new BLResult<User>();

            // Validating input data
            if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
            {
                result.Message = "Invalid data";
                return result;
            }

            // Validating repeated email
            dbResult = userDataAccess.GetUser(user.Email);
            if (dbResult.Success == true)
            {
                result.Message = "Email has already been registered";
                return result;
            }

            // Hashing password
            user.Password = passwordHasher.Hash(user.Password);
            user.IsActive = true;

            // Adding user
            dbResult = userDataAccess.CreateUser(user);
            if (dbResult.Success)
            {
                // We should not return the password
                result.Success = true;
                result.Item = dbResult.Item;
                result.Item.Password = String.Empty;
            }
            else
            {
                result.Message = dbResult.Message;
            }

            return result;
        }

        /// <summary>
        /// Get specifict user by id
        /// </summary>
        /// <param name="idUser">Id User</param>
        /// <returns>Result object</returns>
        public BLResult<User> GetUserById(int idUser)
        {
            DBResult<User> dbResult;
            BLResult<User> result = new BLResult<User>();

            // Search user
            dbResult = userDataAccess.GetUserById(idUser);
            if (dbResult.Success)
            {
                // We should not return the password
                result.Success = true;
                result.Item = dbResult.Item;
                result.Item.Password = String.Empty;
            }
            else
            {
                result.Message = dbResult.Message;
            }

            return result;
        }


        /// <summary>
        /// Update an User with Patch Document
        /// </summary>
        /// <param name="patchUser">Patch document</param>
        /// <param name="idUser">id User</param>
        /// <returns>Result</returns>
        public async Task<BLResult<User>> PatchUser(JsonPatchDocument<User> patchUser, int idUser)
        {
            DBResult<User> dbResult;
            BLResult<User> result = new BLResult<User>();

            // Validating input data
            if (patchUser == null)
            {
                result.Message = "Invalid data";
                return result;
            }

            // Validating allowed operations
            var op = patchUser.Operations.Where(o => o.path.Equals("/Password") || o.path.Equals("/Email")).Count();
            if(op > 0)
            {
                result.Message = "Patch operation not allowed";
                return result;
            }

            // Patchin user
            dbResult = await userDataAccess.PatchUser(patchUser, idUser);
            if (dbResult.Success)
            {
                // We should not return the password
                result.Success = true;
                result.Item = dbResult.Item;
                result.Item.Password = String.Empty;
            }
            else
            {
                result.Message = dbResult.Message;
            }

            return result;
        }

    }
}
