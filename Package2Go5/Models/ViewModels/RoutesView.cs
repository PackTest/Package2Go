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

        public string from { get; set; }

        public string waypoints { get; set; }

        public System.DateTime departure_time { get; set; }

        public System.DateTime delivery_time { get; set; }

        public int status_id { get; set; }

        public Coordinate fromCoord { get; set; }

        public Coordinate toCoord { get; set; }

        public List<Items> Items { get; set; }

    }
}
