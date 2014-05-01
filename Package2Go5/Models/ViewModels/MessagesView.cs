using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;

namespace Package2Go5.Models.ViewModels
{
    public class MessagesView
    {

        public int id { get; set; }

        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "date")]
        public System.DateTime date { get; set; }

        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "from")]
        public string from { get; set; }
        public string to { get; set; }

        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "message")]
        public string message { get; set; }
        public string status { get; set; }

    }
}
