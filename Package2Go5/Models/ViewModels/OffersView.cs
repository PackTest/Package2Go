using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;

namespace Package2Go5.Models.ViewModels
{
    public class OffersView
    {
        public int id { get; set; }



        public int route_id { get; set; }
        public int item_id { get; set; }
        public int status_id { get; set; }

        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "route_id")]
        public string route { get; set; }

        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "item_id")]
        public string item { get; set; }

        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "status_id")]
        public string status { get; set; }

        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "date")]
        public System.DateTime date { get; set; }

    }
}
