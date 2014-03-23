using Package2Go5.Models.ObjectManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Package2Go5.Controllers
{
    public class ItemsController : Controller
    {

        private ItemsManager manager = new ItemsManager();
        private RoutesManager routesManager = new RoutesManager();
        private UserProfileManager userManager = new UserProfileManager();
        private CurrencyManager currencyManager = new CurrencyManager();

        //
        // GET: /Items/

        public ActionResult Index()
        {
            int userId = Int32.Parse(Request.Cookies["UserId"].Value);
            ViewBag.User_id = userId;
            ViewBag.Routes = routesManager.GetAll(userId);
            ViewBag.RoutesList = routesManager.GetRoutesList(userId);

            var items = manager.GetAll(userId);

            return View(manager.GetAll(userId));
        }

        //
        // GET: /Items/Details/5

        public ActionResult Details(int id)
        {
            return View(manager.GetItem(id));
        }

        //
        // GET: /Items/Create
        [Authorize]
        public ActionResult Create()
        {
            Items items = new Items();
            ViewBag.Currencies = currencyManager.GetCurrenciesList();

            items.currency_id = currencyManager.GetCurrency(Int32.Parse(Request.Cookies["UserId"].Value)).id;

            return View(items);
        }

        //
        // POST: /Items/Create

        [HttpPost]
        [Authorize]
        public ActionResult Create(Items items)
        {
            try
            {
                ViewBag.Currencies = currencyManager.GetCurrenciesList();
                manager.Create(items, userManager.GetId(User.Identity.Name));
                return RedirectToAction("Index");
            }
            catch
            {
                return View(items);
            }
        }

        //
        // GET: /Items/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            return View(manager.GetItem(id));
        }

        //
        // POST: /Items/Edit/5

        [HttpPost]
        [Authorize]
        public ActionResult Edit(int id, Items item)
        {
            try
            {
                // TODO: Add update logic here
                manager.Update(id, item);
                return RedirectToAction("Index");
            }
            catch
            {
                return View(item);
            }
        }

        //
        // GET: /Items/Delete/5

        public ActionResult Delete(int id)
        {
            manager.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
