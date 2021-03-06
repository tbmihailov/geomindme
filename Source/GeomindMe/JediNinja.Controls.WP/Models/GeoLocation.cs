﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace JediNinja.Controls.WP
{
    public class GeoLocation
    {
        public GeoLocation(double lat, double lon)
        {
            this.Latitude = lat;
            this.Longitude = lon;
        }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public override string ToString()
        {
            string geoLocationString = string.Format("{0}, {1}", Latitude, Longitude);
            return geoLocationString;
        }
    }
}
