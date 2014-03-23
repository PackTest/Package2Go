using Package2Go5.Models.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Package2Go5.Models.EditModels
{
    public class UserProfileEdit : UserProfileView
    {
        [Display(Name = "New Password")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9]).{6,20}$")]
        public string newPassword { get; set; }

        [Display(Name = "Confirm Password")]
        [Compare("newPassword", ErrorMessage = "Password and Confirm Password should be the same")]
        public string confirmPassword { get; set; }
    }
}