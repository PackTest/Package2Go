using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;

namespace Package2Go5.Models.ViewModels
{
    public class UserProfileView
    {
        [Required]
        [Remote("doesUserNameExist", "UserProfile", HttpMethod = "POST", ErrorMessage = "User name already exists. Please enter a different user name.")]
        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "Username")]
        public string Username { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "password")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9]).{6,20}$", ErrorMessage = "Password is invalid. It must contain 1 number, 1 Uppercase letter, 1 Lowercase letter and be a minimum of 6, maximum 20 charachters")]
        public string password { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "name")]
        public string name { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "lastname")]
        public string lastname { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "gender")]
        [Range(0, 1)]
        public int gender { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "birthday")]
        [DataType(DataType.Date)]
        public DateTime birthday { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "phone_nr")]
        public string phone_nr { get; set; }

        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "rate")]
        public int rate { get; set; }

        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "image")]
        public HttpPostedFileBase image { get; set; }

        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "image")]
        public string image_url { get; set; }

        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "currency_id")]
        public int currency_id { get; set; }

        public int UserId { get; set; }

        public List<CommentsView> comments { get; set; }

        public int commentCount { get; set; }

    }
}
