using System.ComponentModel.DataAnnotations;

namespace DoodleLike.Models.Dtos
{
    public class VMRegister
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string PasswordVerify { get; set; }

    }
}
