using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace iTransition4.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        private string fullName;
        private string email;
        private string password;

        public Guid Id { get; set; }
        public string FullName
        {
            get => fullName; set
            {
                fullName = value;
            }
        }
        public string Email
        {
            get => email; set
            {
                email = value;
            }
        }
        public string Password
        {
            get => password; set
            {
                password = value;
            }
        }
        public DateTime? LastLoginTime { get; set; } = DateTime.MinValue;
        public DateTime RegistrationTime { get; set; }
        public Status IsBlocked { get; set; } = Status.Active;
    }
}
