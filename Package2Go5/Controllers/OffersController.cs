using Package2Go5.Models.ObjectManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Package2Go5.Controllers
{

    public class OffersController : Controller
    {

        private OffersManager offersManager = new OffersManager();
        private UserProfileManager userManager = new UserProfileManager();
        private int id = 0;
        //
        // GET: /Offers/
        [Authorize]
        public ActionResult Index()
        {
            if(Request.Cookies["UserId"] != null)
                id = Int32.Parse(Request.Cookies["UserId"].Value);

            ViewBag.status =  offersManager.GetStatusList();
            return View(offersManager.GetUserOffersList(id));
        }

        [Authorize]
        public ActionResult Offers()
        {
            if (Request.Cookies["UserId"] != null)
                id = Int32.Parse(Request.Cookies["UserId"].Value);
            if (User.Identity.IsAuthenticated && userManager.getRole(id) == 1)
                return View(offersManager.GetAll());
            else
                return View();
        }

        //
        // GET: /Offers/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // POST: /Offers/Create
        [Authorize]
        public ActionResult Create(int i, int r)
        {
            try
            {
                if (Request.Cookies["UserId"] != null)
                    id = Int32.Parse(Request.Cookies["UserId"].Value);

                offersManager.Create(i, r, id);
                return RedirectToAction("Index", "Routes");
            }
            catch
            {
                return RedirectToAction("Index" ,"Routes");
            }
        }

        [Authorize]
        public void CreateOffer(int i, int r) 
        {
            if (Request.Cookies["UserId"] != null)
                id = Int32.Parse(Request.Cookies["UserId"].Value);
            offersManager.Create(i, r, id);            
        }

        //
        // GET: /Offers/Edit/5
        [Authorize]
        public void Edit(int id)
        {
            offersManager.Update(id, 2);
        }

        //
        // POST: /Offers/Delete/5

        [Authorize]
        public ActionResult Delete(int i, int r)
        {
            try
            {
                if (Request.Cookies["UserId"] != null)
                    id = Int32.Parse(Request.Cookies["UserId"].Value);
                offersManager.Delete(i, r, id);
                return RedirectToAction("Index", "Routes");
            }
            catch
            {
                return RedirectToAction("Index", "Routes");
            }
        }
    }
}
