using System.ComponentModel.DataAnnotations;

namespace SevenDays.Api.Models
{
    /// <summary>
    /// Model Class used only for Authentication
    /// </summary>
    public class SimpleUser
    {
        public SimpleUser(string email,int id)
        {
            Email = email;
            IdUser = id;
            Password = "";
        }
        public SimpleUser()
        {
            Email = null;
            Password = null;
        }
        public SimpleUser(string email, string password)
        {
            Email = email;
            Password = password;
        }

        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public int IdUser { get; set; }
        public string Token { get; set; }
    }
}
