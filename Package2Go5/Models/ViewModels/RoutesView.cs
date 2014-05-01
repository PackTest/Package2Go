using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;

namespace Package2Go5.Models.ViewModels
{
    public class RoutesView
    {

        public int id { get; set; }

        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "from")]
        public string from { get; set; }

        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "waypoints")]
        public string waypoints { get; set; }

        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "departure_time")]
        public System.DateTime departure_time { get; set; }

        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "delivery_time")]
        public System.DateTime delivery_time { get; set; }

        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "status_id")]
        public int status_id { get; set; }

        public List<Items> Items { get; set; }

        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "status_id")]
        public string status { get; set; }

        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "Username")]
        public string Username { get; set; }

        [Display(ResourceType = typeof(Resources.DisplayNames), Name = "rate")]
        public Nullable<int> rate { get; set; }

        public int user_id { get; set; }

        public int itemCount { get; set; }

        public List<string> waypointsList { get; set; }
    }
}
