using AutoMapper;
using Package2Go5.Models.ObjectManager;
using Package2Go5.Models.ViewModels;
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
            int userId = 0;

            if (Request.Cookies["UserId"] != null)
                userId = Int32.Parse(Request.Cookies["UserId"].Value);

            ViewBag.User_id = userId;
            ViewBag.Routes = routesManager.GetAll(userId);
            ViewBag.RoutesList = routesManager.GetRoutesList(userId);
            ViewBag.status = manager.getStatusList(-1);

            List<ItemsView> items;

            if(userManager.getRole(userId) != 1)
            {
                items = Mapper.Map<List<Items>, List<ItemsView>>(manager.GetAllUserItems(userId));
            }else{
                items = Mapper.Map<List<Items>, List<ItemsView>>(manager.GetAll(userId));
            }

            foreach (ItemsView item in items) 
            {
                item.route_id = routesManager.GetItemRouteId(item.id);
            }

            return View(items);
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
            Items item = new Items();
            ViewBag.Currencies = currencyManager.GetCurrenciesList();

            item.currency_id = currencyManager.GetCurrency(Int32.Parse(Request.Cookies["UserId"].Value)).id;

            var itemView = Mapper.Map<Items, ItemsView>(item);

            return View(itemView);
        }

        //
        // POST: /Items/Create

        [HttpPost]
        [Authorize]
        public ActionResult Create(ItemsView items)
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
            ItemsView item = manager.GetItem(id);
            ViewBag.status = manager.getStatusList(item.status_id);

            return View(item);
        }

        //
        // POST: /Items/Edit/5

        [HttpPost]
        [Authorize]
        public ActionResult Edit(int id, ItemsView item)
        {
            ViewBag.status = manager.getStatusList(item.status_id);
            try
            {
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

        [Authorize]
        public ActionResult History()
        {
            int userId = 0;

            if (Request.Cookies["UserId"] != null)
                userId = Int32.Parse(Request.Cookies["UserId"].Value);

            return View(manager.GetAllHistory(userId));
        }

        [Authorize]
        public void DeliveredItem(int id) 
        {
            int userId = 0;
            if (Request.Cookies["UserId"] != null)
                userId = Int32.Parse(Request.Cookies["UserId"].Value);

            manager.deliveredItem(id, userId);
        }
    }
}
