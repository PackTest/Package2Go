using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Package2Go5.Models
{
    public class Coordinate
    {
        public Coordinate(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;

        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}