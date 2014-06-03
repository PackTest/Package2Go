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
        private ItemsManager itemsManager = new ItemsManager();

        public void Create(RoutesView routeView, int userId)
        {
            Routes route = new Routes();

            route.from = HttpUtility.HtmlEncode(routeView.from);
            route.waypoints = HttpUtility.HtmlEncode(routeView.waypoints);

            route.departure_time = routeView.departure_time;
            route.delivery_time = routeView.delivery_time;
            route.status_id = 1;

            db.UsersRoutes.Add(new UsersRoutes { user_id = userId, route_id = route.id });

            db.Routes.Add(route);
            db.SaveChanges();
        }

        public void Update(int id, RoutesView routeView, int UserId)
        {
            Routes route = db.Routes.FirstOrDefault(u => u.id == id);

            List<Offers> offers = new List<Offers>();

            route.from = HttpUtility.HtmlEncode(routeView.from);
            route.status_id = routeView.status_id;
            route.delivery_time = routeView.delivery_time;
            route.departure_time = routeView.departure_time;

            //if (routeView.delivery_time < routeView.departure_time || routeView.departure_time < DateTime.Now) 
            //{
            //    throw new Exception();
            //}

            if (route.status_id == 1)
            {
                route.waypoints = HttpUtility.HtmlEncode(routeView.waypoints);

                List<int> itemsId = new List<int>();
                foreach (ItemsRoutes ir in route.ItemsRoutes)
                {
                    if (!itemsId.Contains(ir.item_id))
                        itemsId.Add(ir.item_id);
                }

                db.ItemsRoutes.RemoveRange(db.ItemsRoutes.Where(ir => ir.route_id == id));
                foreach (Items item in db.Items.Where(i => i.ItemsRoutes.Any(ir => ir.route_id == id)))
                {
                    item.status_id = 1;
                }

                List<int> messagesId = new List<int>();
                List<int> ItemsRoutesIds = new List<int>();
                if (routeView.Items != null)
                    foreach (Items ir in routeView.Items)
                    {
                        offers = db.Offers.Where(o => o.item_id == ir.id && o.Routes.UsersRoutes.Any(ur => ur.user_id == UserId)).ToList();
                        if (offers.Count != 0)
                        {
                            foreach (Offers offer in offers)
                            {
                                offer.status_id = 3;
                            }
                        }

                        //send Message about added item
                        if (!itemsId.Contains(ir.id))
                        {
                            if (!messagesId.Contains(ir.id))
                            {
                                var message = new Messages
                                {
                                    from = UserId,
                                    to = db.UsersItems.Where(ui => ui.item_id == ir.id).First().user_id,
                                    date = DateTime.Now,
                                    message = "Your item was added to route =" + route.id + "=" + ir.id + "=" + db.Items.Where(i => i.id == ir.id).First().title,
                                    statusId = 1
                                };
                                messagesId.Add(ir.id);
                                db.Messages.Add(message);
                                db.UsersMessages.Add(new UsersMessages { userId = UserId, messageId = message.id });
                                db.UsersMessages.Add(new UsersMessages { userId = db.UsersItems.Where(ui => ui.item_id == ir.id).First().user_id, messageId = message.id });
                            }
                        }
                        else
                        {
                            itemsId.Remove(ir.id);
                        }

                        if (!ItemsRoutesIds.Contains(ir.id))
                        {
                            route.ItemsRoutes.Add(new ItemsRoutes { item_id = ir.id, route_id = id });
                            ItemsRoutesIds.Add(ir.id);
                            db.Items.Where(i => i.id == ir.id).First().status_id = 2;
                        }

                    }

                List<Messages> messages = new List<Messages>();
                List<UsersMessages> usersMessages = new List<UsersMessages>();
                foreach (int itemId in itemsId)
                {
                    var message = new Messages
                    {
                        from = UserId,
                        to = db.UsersItems.Where(ui => ui.item_id == itemId).First().user_id,
                        date = DateTime.Now,
                        message = "Your item was removed from route =" + route.id + "=" + itemId + "=" + db.Items.Where(i => i.id == itemId).First().title,
                        statusId = 1
                    };
                    messages.Add(message);
                    message.UsersMessages.Add(new UsersMessages { userId = UserId, messageId = message.id });
                    message.UsersMessages.Add(new UsersMessages { userId = db.UsersItems.Where(ui => ui.item_id == itemId).First().user_id, messageId = message.id });
                }
                db.Messages.AddRange(messages);
            }
            else if (route.status_id == 2)
            {
                db.ItemsRoutes.RemoveRange(db.ItemsRoutes.Where(ir => ir.route_id == id));
                foreach (Items item in db.Items.Where(i => i.ItemsRoutes.Any(ir => ir.route_id == id)))
                {
                    item.status_id = 1;
                }
                var waypoints = "";
                foreach (string waypoint in route.waypoints.Split(';'))
                {
                    if (waypoint.Split(':')[0] == "0")
                        waypoints += waypoint + ";";
                }
                route.waypoints = HttpUtility.HtmlEncode(waypoints.Substring(0, waypoints.Length - 1));
            }
            else if (routeView.Items != null)
            {
                var messages = new List<Messages>();
                foreach (Items item in routeView.Items) 
                {
                    var message = new Messages
                    {
                        from = UserId,
                        to = db.UsersItems.Where(ui => ui.item_id == item.id).First().user_id,
                        date = DateTime.Now,
                        message = "Did your item arrived =" + id + "=" + item.id + "=" + db.Items.Where(i => i.id == item.id).First().title,
                        statusId = 1
                    };
                    messages.Add(message);
                    message.UsersMessages.Add(new UsersMessages { userId = UserId, messageId = message.id });
                    message.UsersMessages.Add(new UsersMessages { userId = db.UsersItems.Where(ui => ui.item_id == item.id).First().user_id, messageId = message.id });
                }
                db.Messages.AddRange(messages);
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

            route.from = route.from.Split(':')[1];

            var waypoints = "";

            foreach (string r in route.waypoints.Split(';')) 
            {
                waypoints += r.Split(':')[1] + "->";
            }

            route.waypoints = waypoints.Substring(0, waypoints.Length - 2);

            return route;
        }

        public SelectList GetStatus(int status_id)
        {
            List<SelectListItem> status = new List<SelectListItem>();
            bool selected = false;

            if (status_id == -1)
                status.Add(new SelectListItem { Text = "All", Value = "", Selected = selected });

            foreach (RouteStatus stat in db.RouteStatus)
            {
                if (stat.id == status_id)
                    selected = true;
                else
                    selected = false;
                if (status_id != -1)
                    status.Add(new SelectListItem { Text = stat.title, Value = stat.id.ToString(), Selected = selected });
                else
                    status.Add(new SelectListItem { Text = stat.title, Value = stat.title, Selected = selected });
            }

            return new SelectList(status, "Value", "Text");
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

        public string GetOnlyCities(string addresses, string separator) 
        {
            string waypoints = "";
            foreach (string waypoint in addresses.Split(';'))
            {
                string[] address;
                string[] city;

                try
                {
                    if (waypoint.Contains("County") && waypoint.Split(',').Length > 2)
                        address = waypoint.Split(',').Reverse().Skip(2).First().Split(':');
                    else
                    {
                        address = waypoint.Split(',').Reverse().Skip(1).First().Split(':');
                    }

                    if (address.Length > 1)
                        city = address[1].Split(' ');
                    else
                        city = address[0].Split(' ');

                    if (city.Length > 1)
                        waypoints += city[1] + separator;
                    else
                        waypoints += city[0] + separator;
                }
                catch 
                {
                    waypoints += waypoint.Split(':')[1];
                }
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

            db.Items.Where(item => item.id == i).First().status_id = 2;

            var offer = db.Offers.Where(o => o.item_id == i && o.route_id == r).First();
            offer.status_id = 3;

            var orderItem = db.Items.Where(item => item.id == i).First();
            db.Routes.Where(route => route.id == r).First().waypoints += ";" + i + ":" + orderItem.address + ";" + i + ":" + orderItem.delivery_address;

            db.Offers.RemoveRange(db.Offers.Where(o => o.item_id == i && o.route_id != r && o.id != offer.id));

            //Message
            var message = new Messages
            {
                from = offer.Routes.UsersRoutes.First().user_id,
                to = offer.Items.UsersItems.First().user_id,
                date = DateTime.Now,
                message = "=" + i + "=" + db.Items.Where(item=>item.id == i).First().title + "= was added to route =" + r,
                statusId = 1
            };

            db.Messages.Add(message);

            db.UsersMessages.Add(new UsersMessages { userId = offer.Routes.UsersRoutes.First().user_id, messageId = message.id });
            db.UsersMessages.Add(new UsersMessages { userId = offer.Items.UsersItems.First().user_id, messageId = message.id });

            db.SaveChanges();
        }


        public int GetItemRouteId(int itemId) 
        {
            var route = db.ItemsRoutes.Where(ir => ir.item_id == itemId).FirstOrDefault();
            if (route != null)
                return route.route_id;
            else
                return 0;
        }

        public void Decline(int item_id) 
        {
            var route = db.Routes.Where(r => r.ItemsRoutes.Any(ir => ir.item_id == item_id)).First();

            var item = db.Items.Where(i => i.id == item_id).First();

            var userFrom = db.UserProfile.Where(up=>up.UsersItems.Any(ui=>ui.item_id==item_id)).First().UserId;
            var userTo = db.UserProfile.Where(up => up.UsersRoutes.Any(ur => ur.route_id == route.id)).First().UserId;

            var message = new Messages
            {
                from = userFrom,
                to = userTo,
                date = DateTime.Now,
                message = "=" + item.id + "=" + item.title + "= was removed from your route =" + route.id,
                statusId = 1
            };

            db.Messages.Add(message);

            db.UsersMessages.Add(new UsersMessages { userId = userFrom, messageId = message.id });
            db.UsersMessages.Add(new UsersMessages { userId = userTo, messageId = message.id });

            db.ItemsRoutes.RemoveRange(route.ItemsRoutes);
            item.status_id = 1;

            var newWaypoint = "";
            foreach (string waypoint in route.waypoints.Split(';')) 
            {
                if (int.Parse(waypoint.Split(':')[0]) != item_id)
                    newWaypoint += waypoint + ";";
            }

            route.waypoints = newWaypoint.Substring(0, newWaypoint.Length - 1);

            db.Offers.RemoveRange(db.Offers.Where(o=>o.item_id == item_id));

            db.SaveChanges();
        }
    }
}
