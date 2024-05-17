using System.ComponentModel.DataAnnotations.Schema;

namespace iTransition4.Models
{
    [NotMapped]
    public class CreateUserViewModel
    {
        private string fullName;
        private string email;
        private string password;


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
        public DateTime LastLoginTime { get; set; }
        public DateTime RegistrationTime { get; set; }
        public Status IsBlocked { get; set; } = Status.Active;
    }
}
