using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodePlusBlog.Model
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        [NotMapped]
        public string NewPassword { get; set; }
        public int UsertypeId { get; set; }
        [NotMapped]
        public string RoleName { get; set; }
        [NotMapped]
        public bool RememberMe { get; set; } = false;
        public bool PasswordChangeRequired { get; set; } = true;
        [NotMapped]
        public string OTP { get; set; }

    }
}
