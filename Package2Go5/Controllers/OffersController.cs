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

        OffersManager offersManager = new OffersManager();

        //
        // GET: /Offers/

        public ActionResult Index()
        {
            return View(offersManager.GetUserOffersList(Int32.Parse(Request.Cookies["UserId"].Value)));
        }

        //
        // GET: /Offers/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // POST: /Offers/Create

        public ActionResult Create(int i, int r)
        {
            try
            {
                offersManager.Create(i, r, Int32.Parse(Request.Cookies["UserId"].Value));
                return RedirectToAction("Index", "Routes");
            }
            catch
            {
                return RedirectToAction("Index" ,"Routes");
            }
        }

        //
        // GET: /Offers/Edit/5

        public void Edit(int id)
        {
            offersManager.Update(id, 2);
        }

        //
        // POST: /Offers/Delete/5

        //[HttpPost]
        public ActionResult Delete(int i, int r)
        {
            try
            {
                offersManager.Delete(i, r, Int32.Parse(Request.Cookies["UserId"].Value));
                return RedirectToAction("Index", "Routes");
            }
            catch
            {
                return RedirectToAction("Index", "Routes");
            }
        }
    }
}
