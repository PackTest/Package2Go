using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;

namespace Package2Go5.Models.ViewModels
{
    public class UserProfileView
    {
        [Required]
        [Display(Name = "User Name")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Password")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9]).{6,20}$", ErrorMessage = "Password is invalid. It must contain 1 number, 1 Uppercase letter, 1 Lowercase letter and be a minimum of 6, maximum 20 charachters")]
        public string password { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string name { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string lastname { get; set; }

        [Required]
        [Display(Name = "Gender")]
        [Range(0, 1)]
        public int gender { get; set; }

        [Required]
        [Display(Name = "Birthday")]
        [DataType(DataType.Date)]
        public DateTime birthday { get; set; }

        [Required]
        [Display(Name = "Phone number")]
        public int phone_nr { get; set; }

        [Display(Name = "Rate")]
        public int rate { get; set; }

        [Display(Name = "Profile Image")]
        public HttpPostedFileBase image { get; set; }

        public int currency_id { get; set; }

    }
}
