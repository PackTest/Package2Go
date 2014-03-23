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

        public ActionResult FindUser(string to)
        {
            var result = (from u in db.UserProfile
                          where u.Username.ToLower().Contains(to.ToLower())
                          select new { u.Username }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult Header() 
        {
            var offers = offersManager.GetUserNewOffers(0);
            //var offers = offersManager.GetUserNewOffers(Int32.Parse(Request.Cookies["UserId"].Value));

            var messages = messagesManager.GetNewUserMessages(User.Identity.Name);
            ViewBag.Messages = messages;

            return PartialView(offers);
        }
    }
}
