using Package2Go5.Models.ObjectManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Package2Go5.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        private OffersManager offersManager = new OffersManager();
        private MessagesManager messagesManager = new MessagesManager();
        private RoutesManager routesManager = new RoutesManager();
        private UserProfileManager usersManager = new UserProfileManager();

        private Package2GoEntities db = new Package2GoEntities();

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult FindTripsPoints(int show)
        {
            var points = "";

            if (show == 0 || show == 1)
            {
                foreach (Routes route in db.Routes)
                {
                    points += route.id + ":" + routesManager.GetOnlyCities(route.from, ",") + ","
                            + routesManager.GetOnlyCities(route.waypoints, ",") + ",";
                }
            }

            if (show == 0 || show == 2)
            {
                points += "items,";
                IEnumerable<Items> items;
                int userId = 0;

                if (Request.Cookies["UserId"] != null)
                    userId = int.Parse(Request.Cookies["UserId"].Value);

                if (show == 2)
                {
                    items = db.Items.Where(i => i.UsersItems.Any(ui => ui.user_id != userId));
                }else{
                    items = db.Items;  
                }

                foreach (Items item in items)
                {
                    points += item.id + ":" + routesManager.GetOnlyCities(item.delivery_address, ",") + ",";
                }
            }

            points = points.Substring(0, points.Length - 1);

            return Json(points, JsonRequestBehavior.AllowGet);
        }

                //    var test = '{"1":{"id":"1", "address":"Kupiškis, Panevėžys County, Lithuania", "delivery_address":"Šilainiai, Kaunas, Kaunas County, Lithuania", "title":"Kedė"},'
                //+ '"2":{"id":"2", "address":"Kupiškis, Panevėžys County, Lithuania", "delivery_address":"Šilainiai, Kaunas, Kaunas County, Lithuania", "title":"Kedė"}}';

        public JsonResult FindItems() 
        {
            int userId = 0;
            var points = "{";
            IEnumerable<Items> items;

            if (Request.Cookies["UserId"] != null)
                userId = int.Parse(Request.Cookies["UserId"].Value);

            items = db.Items.Where(i => i.UsersItems.Any(ui => ui.user_id != userId));

            foreach (Items item in items)
            {
                points += "\"" + item.id + "\":{\"address\":\"" + item.address + "\", \"delivery_address\":\"" + item.delivery_address + "\", \"title\":\"" + item.title + "\"},";
            }

            points = points.Substring(0, points.Length - 1)+"}";

            return Json(points, JsonRequestBehavior.AllowGet);
        }


        public JsonResult FindItems2()
        {
            int userId = 0;
            //var points = "{ \"Items\": [";
            var points = "[";
            IEnumerable<Items> items;

            if (Request.Cookies["UserId"] != null)
                userId = int.Parse(Request.Cookies["UserId"].Value);

            items = db.Items.Where(i => i.UsersItems.Any(ui => ui.user_id != userId));

            foreach (Items item in items)
            {
                points += "{ \"id\":\"" + item.id + "\", \"address\":\"" + item.address + "\", \"delivery_address\":\"" + item.delivery_address + "\", \"title\":\"" + item.title + "\"},";
            }

            //points = points.Substring(0, points.Length - 1) + "]}";
            points = points.Substring(0, points.Length - 1)+"]";

            return Json(points, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FindRoutes()
        {
            int userId = 0;
            var points = "";
            IEnumerable<Routes> routes;

            if (Request.Cookies["UserId"] != null)
                userId = int.Parse(Request.Cookies["UserId"].Value);

            routes = db.Routes.Where(r => r.UsersRoutes.Any(ur => ur.user_id != userId));

            foreach (Routes route in routes)
            {
                points += route.id + ">" + route.from + ";" + route.waypoints + "/";
            }

            points = points.Substring(0, points.Length - 1);

            return Json(points, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMarkers() 
        {
            string markers = Request.Url.GetLeftPart(UriPartial.Authority) + "/Images/van.png" + "," + Request.Url.GetLeftPart(UriPartial.Authority) +"/Images/Item.png";
            return Json(markers, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FindUsers()
        {
            var userNames = db.UserProfile.Select(u => u.Username).ToList();

            return Json(userNames, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult Header() 
        {
            List<Offers> offers = new List<Offers>();
            var role = 0;
            if (Request.Cookies["UserId"] != null)
            {
                int userId = Int32.Parse(Request.Cookies["UserId"].Value);
                offers = offersManager.GetUserNewOffers(userId);
                role = usersManager.getRole(userId);
            }

            var messages = messagesManager.GetNewUserMessages(User.Identity.Name);
            ViewBag.Messages = messages;

            ViewBag.role = role;

            return PartialView(offers);
        }
    }
}
