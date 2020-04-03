using System.ComponentModel.DataAnnotations;

namespace SevenDays.Entities
{
    /// <summary>
    /// Model Class used only for Authentication
    /// </summary>
    public class SimpleUser
    {
        // Profile types
        public static string Admin = "1";
        public static string Customer = "2";

        public SimpleUser()
        {
            Password = "";
        }

        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public int IdUser { get; set; }
        public int? Profile { get; set; }
        public string Token { get; set; }
    }
}
