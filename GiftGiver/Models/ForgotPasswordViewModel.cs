using System.ComponentModel.DataAnnotations;

namespace GiftGiver.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}