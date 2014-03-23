using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;

namespace Package2Go5.Models.ViewModels
{
    public class ItemsView
    {

        public int id { get; set; }

        public string title { get; set; }

        public string delivery_address { get; set; }

        public string size { get; set; }

        public string delivery_price { get; set; }

        public string note { get; set; }

        public List<Routes> routes { get; set; }

    }
}
