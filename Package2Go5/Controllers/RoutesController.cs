using Package2Go5.Models.ObjectManager;
using Package2Go5.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using GoogleMapsApi.Entities;
using AutoMapper;

namespace Package2Go5.Controllers
{
    public class RoutesController : Controller
    {
        //
        // GET: /Routes/

        private RoutesManager manager = new RoutesManager();
        private UserProfileManager userManager = new UserProfileManager();
        private ItemsManager itemManager = new ItemsManager();
        private CurrencyManager currencyManager = new CurrencyManager();
        private OffersManager offersManager = new OffersManager();

        public ActionResult Index()
        {
            int userId = Int32.Parse(Request.Cookies["UserId"].Value);
            ViewBag.User_id = userId;
            ViewBag.Items = itemManager.GetNotUsedUserItemsList(userId);

            ViewBag.Offers = offersManager.GetUserOfferedRoutesIds(userId);

            return View(manager.GetAll());
        }

        [Authorize]
        public ActionResult Details(int id)
        {
            Mapper.CreateMap<Routes, RoutesView>();
            RoutesView route = Mapper.Map<Routes, RoutesView>(manager.GetRoute(id));

            route.fromCoord = manager.GetCoordinates(route.from);

            return View(route);
        }

        //
        // GET: /Routes/Create

        [Authorize]
        public ActionResult Create()
        {
            ViewBag.status = new SelectList(manager.GetStatus(), "Value", "Key");
            return View();
        }

        //
        // POST: /Routes/Create

        [HttpPost]
        [Authorize]
        public ActionResult Create(RoutesView rotesView)
        {
            ViewBag.status = new SelectList(manager.GetStatus(), "Value", "Key");

            foreach (string key in Request.Form.AllKeys)
            {
                if (key.StartsWith("waypoint_"))
                {
                    rotesView.waypoints += Request.Form[key] + ";";
                }
            }
            rotesView.waypoints = rotesView.waypoints.Remove(rotesView.waypoints.Length - 1, 1);

            try
            {
                manager.Create(rotesView, Int32.Parse(Request.Cookies["UserId"].Value));

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Routes/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            int userId = Int32.Parse(Request.Cookies["UserId"].Value);
            if (manager.IsUserInRoute(id, userId)) 
            {
                var currency = currencyManager.GetCurrency(userId);
                ViewBag.status = new SelectList(manager.GetStatus(), "Value", "Key");
                ViewBag.actionList = manager.GetActionList();
                ViewBag.userCurrency = currency.code;

                Mapper.CreateMap<Routes, RoutesView>();
                var routes = Mapper.Map<Routes, RoutesView>(manager.GetRoute(id));

                routes.Items = itemManager.GetAll(userId);

                ViewBag.ItemsPrices = itemManager.GetPriceUserVal(routes.Items, currency.id);

                return View(routes);
            }

            return RedirectToAction("Index");
        }

        //
        // POST: /Routes/Edit/5

        [HttpPost]
        [Authorize]
        public ActionResult Edit(int id, RoutesView routesView)
        {
            ViewBag.status = new SelectList(manager.GetStatus(), "Value", "Key");

            foreach (string key in Request.Form.AllKeys)
            {
                if (key.StartsWith("waypoint_"))
                {
                    routesView.waypoints += Request.Form[key] + ";";
                }
            }
            routesView.waypoints = routesView.waypoints.Remove(routesView.waypoints.Length - 1, 1);

            try
            {
                manager.Update(id, routesView, Int32.Parse(Request.Cookies["UserId"].Value));

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }

        }

        //
        // GET: /Routes/Delete/5

        [Authorize]
        public ActionResult Delete(int id)
        {
            manager.Delete(id);
            return RedirectToAction("Index");
        }

        public ActionResult RouteView(int id) 
        {
            return PartialView(manager.GetRoute(id));
        }
    }
}
