using System.Linq;
using Package2Go5.Models.ViewModels;
using Package2Go5.Models;
using System.Data.Entity;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.IO;
using Package2Go5.Models.EditModels;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using Package2Go5.Constants;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Net;
using System.Xml.Linq;
using System.Globalization;

namespace Package2Go5.Models.ObjectManager
{
    public class RoutesManager
    {
        private Package2GoEntities db = new Package2GoEntities();
        //private OffersManager offersManager = new OffersManager();
        private ItemsManager itemsManager = new ItemsManager();

        public void Create(RoutesView routeView, int userId)
        {
            Routes route = new Routes();

            route.from = HttpUtility.HtmlEncode(routeView.from);
            route.waypoints = HttpUtility.HtmlEncode(routeView.waypoints);

            route.departure_time = routeView.departure_time;
            route.delivery_time = routeView.delivery_time;
            route.status_id = routeView.status_id;

            db.UsersRoutes.Add(new UsersRoutes { user_id = userId, route_id = route.id });

            db.Routes.Add(route);
            db.SaveChanges();
        }

        public void Update(int id, RoutesView routeView, int UserId)
        {
            Routes route = db.Routes.FirstOrDefault(u => u.id == id);
            List<Offers> offers = new List<Offers>();

            route.from = HttpUtility.HtmlEncode(routeView.from);
            route.waypoints = HttpUtility.HtmlEncode(routeView.waypoints);
            route.status_id = routeView.status_id;
            route.delivery_time = routeView.delivery_time;
            route.departure_time = routeView.departure_time;

            db.ItemsRoutes.RemoveRange(db.ItemsRoutes.Where(ir=>ir.route_id == id));

            

            if(routeView.Items != null)
                foreach (Items ir in routeView.Items)
                {
                    offers = db.Offers.Where(o => o.item_id == ir.id && o.Routes.UsersRoutes.Any(ur=>ur.user_id == UserId)).ToList();
                    if (offers.Count != 0) 
                    {
                        //db.Offers.RemoveRange(offers);
                        foreach (Offers offer in offers) 
                        {
                            offer.status_id = 3;
                        }
                    }

                    route.ItemsRoutes.Add(new ItemsRoutes { item_id = ir.id, route_id = id });
                }

            db.SaveChanges();
        }

        public void Delete(int id) 
        {
            foreach (UsersRoutes userRoute in db.UsersRoutes.Where(ur => ur.route_id == id))
            {
                db.UsersRoutes.Remove(userRoute);
            }
            db.Routes.Remove(GetRoute(id));

            db.SaveChanges();
        }

        public bool IsUserInRoute(int routeId, int userId) 
        {
            return db.UsersRoutes.Any(r => (r.user_id == userId && r.route_id == routeId));
        }

        public List<vw_routes> GetAll(int userId = 0)
        {
            List<vw_routes> routes;

            if (userId != 0) 
            {
                routes = db.vw_routes.Where(r=> r.user_id == userId).ToList();
            }else
                routes = db.vw_routes.ToList();

            foreach (vw_routes route in routes)
            {
                route.from = GetOnlyCities(route.from, "->");
                route.waypoints = GetOnlyCities(route.waypoints, "->");
            }

            return routes;
        }

        public Routes GetRoute(int id) 
        {
            return db.Routes.Where(r => r.id == id).First();
        }

        public vw_routes Get_vw_Route(int id)
        {
            var route = db.vw_routes.Where(r => r.id == id).First();

            //route.from = route.from.Substring(route.from.IndexOf(':'), route.from.Length);

            route.waypoints = route.waypoints.Split(';').Aggregate((i, j) => i.Split(':')[1] + "->" + j.Split(':')[1]);

            //route.waypoints = GetOnlyCities(route.waypoints, "->");

            return route;
        }

        public List<KeyValuePair<string, int>> GetStatus() 
        {
            List<KeyValuePair<string, int>> status = new List<KeyValuePair<string, int>>();

            foreach (RouteStatus stat in db.RouteStatus.ToList()) 
            {
                status.Add(new KeyValuePair<string,int>(stat.title, stat.id));
            }

            return status;
        }

        public SelectList GetActionList() 
        {
            List<KeyValuePair<string, string>> actionList = new List<KeyValuePair<string, string>>();

            actionList.Add(new KeyValuePair<string, string>("Choose", ""));
            actionList.Add(new KeyValuePair<string, string>("Taken", "Remove"));
            actionList.Add(new KeyValuePair<string, string>("Free", "Add"));

            return new SelectList(actionList, "Value", "Key");
        }


        public List<SelectListItem> GetRoutesList(int user_id = 0)
        {
            List<SelectListItem> routesList = new List<SelectListItem>();
            List<vw_routes> routes;

            if (user_id != 0)
                routes = db.vw_routes.Where(r => r.user_id == user_id).ToList();
            else
                routes = db.vw_routes.ToList();

            routesList.Add(new SelectListItem { Text = "Choose Route", Value = "" });

            foreach (vw_routes route in routes)
            {
                routesList.Add(new SelectListItem { Text = route.waypoints, Value = route.id.ToString() });
            }

            return routesList;
        }

        public Coordinate GetCoordinates(string region)
        {
            WebRequest request = WebRequest
               .Create("http://maps.googleapis.com/maps/api/geocode/xml?sensor=false&address="
                  + HttpUtility.UrlEncode(region));

            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    XDocument document = XDocument.Load(new StreamReader(stream));

                    XElement longitudeElement = document.Descendants("lng").FirstOrDefault();
                    XElement latitudeElement = document.Descendants("lat").FirstOrDefault();

                    if (longitudeElement != null && latitudeElement != null)
                    {
                        return new Coordinate(Double.Parse(longitudeElement.Value, CultureInfo.InvariantCulture),
                            Double.Parse(latitudeElement.Value, CultureInfo.InvariantCulture));
                    }
                }
            }

            return null;
        }

        public string GetOnlyCities(string addresses, string separator) 
        {
            string waypoints = "";
            foreach (string waypoint in addresses.Split(';'))
            {
                string[] address;
                string[] city;

                if(waypoint.Contains("County"))
                    address = waypoint.Split(',').Reverse().Skip(2).First().Split(':');
                else
                    address = waypoint.Split(',').Reverse().Skip(1).First().Split(':');
                
                if(address.Length > 1)
                    city = address[1].Split(' ');
                else
                    city = address[0].Split(' ');

                if (city.Length > 1)
                    waypoints += city[1] + separator;
                else
                    waypoints += city[0] + separator;
            }
            waypoints = waypoints.Remove(waypoints.Length - separator.Length, separator.Length);

            return waypoints;
        }

        public void AcceptOrder(int r, int i) 
        {
            ItemsRoutes itemRoute = new ItemsRoutes();

            itemRoute.item_id = i;
            itemRoute.route_id = r;

            db.ItemsRoutes.Add(itemRoute);

            db.Offers.Where(o => o.item_id == i && o.route_id == r).First().status_id = 3;

            db.Routes.Where(route => route.id == r).First().waypoints += ";"+i+":"+db.Items.Where(item=>item.id == i).First().delivery_address;

            db.Offers.RemoveRange(db.Offers.Where(o => o.item_id == i && o.route_id != r));

            db.SaveChanges();
        }

    }
}
