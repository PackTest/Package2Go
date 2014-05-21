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
        private CommentsManager commentsManager = new CommentsManager();

        public ActionResult Index()
        {
            int userId = 0;
            if(Request.Cookies["UserId"] != null)
                userId = Int32.Parse(Request.Cookies["UserId"].Value);
            ViewBag.User_id = userId;
            ViewBag.Items = itemManager.GetNotUsedUserItemsList(userId);

            ViewBag.status = manager.GetStatus(-1);

            ViewBag.Offers = offersManager.GetUserOfferedRoutesIds(userId);

            List<RoutesView> routes;

            if(userManager.getRole(userId) != 1)
            {
                routes = Mapper.Map<List<vw_routes>, List<RoutesView>>(manager.GetAll(userId));
            }else{
                routes = Mapper.Map<List<vw_routes>, List<RoutesView>>(manager.GetAll());
            }

            return View(routes);
        }

        public ActionResult Details(int id)
        {
            var routes = Mapper.Map<vw_routes, RoutesView>(manager.Get_vw_Route(id));

            ViewBag.commentCount = commentsManager.GetCommentsCount(routes.user_id);

            return View(routes);
        }

        //
        // GET: /Routes/Create

        [Authorize]
        public ActionResult Create()
        {
            ViewBag.status = manager.GetStatus(0);
            return View();
        }

        //
        // POST: /Routes/Create

        [HttpPost]
        [Authorize]
        public ActionResult Create(RoutesView rotesView)
        {
            ViewBag.status = manager.GetStatus(rotesView.status_id);

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
                var routes = Mapper.Map<Routes, RoutesView>(manager.GetRoute(id));
                routes.Items = itemManager.GetAll(userId, -1);

                ViewBag.actionList = manager.GetActionList();
                ViewBag.userCurrency = currency.code;
                ViewBag.status = manager.GetStatus(routes.status_id);
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
            ViewBag.status = manager.GetStatus(routesView.status_id);

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

        public void AcceptOrder(int r, int i)
        {
            manager.AcceptOrder(r, i);
        }

        public ActionResult FindRoute()
        {
            int userId = 0;
            if (Request.Cookies["UserId"] != null)
                userId = Int32.Parse(Request.Cookies["UserId"].Value);

            return View(itemManager.GetNotUsedUserItemsList(userId));
        }

        public bool DeclineRoute(int i)
        {
            int userId = 0;
            if (Request.Cookies["UserId"] != null)
                userId = Int32.Parse(Request.Cookies["UserId"].Value);
            try
            {
                manager.Decline(i);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
