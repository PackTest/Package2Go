using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Package2Go5.Models.ObjectManager;
using Package2Go5.Models.ViewModels;
using Package2Go5.Models.EditModels;
using AutoMapper;
using System.Web.Security;
using System.Security.Cryptography;
using Package2Go5.Constants;
using System.Resources;

namespace Package2Go5.Controllers
{
    public class UserProfileController : Controller
    {

        private UserProfileManager manager = new UserProfileManager();
        private CurrencyManager currencyManager = new CurrencyManager();
        private CommentsManager commentManager = new CommentsManager();
        private int userId = 0;

        //
        // GET: /UserProfile/
        [Authorize]
        public ActionResult Index()
        {
            if (Request.Cookies["UserId"] != null)
                userId = Int32.Parse(Request.Cookies["UserId"].Value);

            if (User.Identity.IsAuthenticated && manager.getRole(userId) == 1) 
            {
                return View(manager.GetAllUsers());
            }
            else
                return RedirectToAction("Edit");

            //UserProfile user = manager.Get(User.Identity.Name);

            //var userView = Mapper.Map<UserProfile, UserProfileView>(user);

            //var comments = commentManager.GetComments(userView.Username);

            //userView.commentCount = comments.Count();
            //userView.comments = comments.Take(10).ToList();

            //return View(userView);
        }

        //
        // GET: /UserProfile/Details/5

        public ActionResult Profile(string username, int e = 0)
        {
            if(e == 1)
                ViewBag.error = "You can't write more comments.";

            UserProfileView user = Mapper.Map<UserProfile, UserProfileView>(manager.Get(username));

            if (user == null)
                return RedirectToAction("Index", "Home");

            var comments = commentManager.GetComments(username);
            user.commentCount = comments.Count();
            user.comments = comments.Take(10).ToList();

            return View(user);
        }

        //
        // GET: /UserProfile/Create

        public ActionResult Create()
        {
            ViewBag.currencies = currencyManager.GetCurrenciesList();
            return View();
        }

        [HttpPost]
        public ActionResult Create(UserProfileView viewModel)
        {
            try
            {
                manager.Create(viewModel);
                ViewBag.currencies = currencyManager.GetCurrenciesList();

                UserProfile user = Mapper.Map<UserProfileView, UserProfile>(viewModel);

                LogIn(user);

                return RedirectToAction("Index", "Home");
            }
            catch
            {
                ViewBag.currencies = currencyManager.GetCurrenciesList();
                return View();
            }
        }

        //Check if username avaible
        [HttpPost]
        public JsonResult doesUserNameExist(string UserName)
        {
            return Json(manager.doesUserNameExist(UserName));
        }

        //
        // GET: /UserProfile/Edit/5

        [Authorize]
        public ActionResult Edit(int e = 0)
        {

            UserProfile user = manager.Get(User.Identity.Name);
            UserProfileEdit model = Mapper.Map<UserProfile, UserProfileEdit>(user);

            ViewBag.currencies = currencyManager.GetCurrenciesList();

            if (e == 1)
                ViewBag.error = DisplayErrors.upload;

            if (!String.IsNullOrWhiteSpace(user.image_url))
                ViewBag.image = "../Images/Profiles/"+user.image_url;

            return View(model);
        }

        //
        // POST: /UserProfile/Edit/5

        [HttpPost]
        [Authorize]
        public ActionResult Edit(UserProfileEdit viewModel)
        {
            try
            {
                ViewBag.currencies = currencyManager.GetCurrenciesList();
                manager.Update(viewModel);
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.error = DisplayErrors.userForm;
                return View(viewModel);
            }
        }

        //
        // GET: /UserProfile/Delete/5
        //[Authorize]
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //
        // POST: /UserProfile/Delete/5

        [Authorize]
        public ActionResult Delete(int id)
        {
            try
            {
                if (Request.Cookies["UserId"] != null)
                    userId = Int32.Parse(Request.Cookies["UserId"].Value);
                if (userId != 0 && User.Identity.IsAuthenticated && manager.getRole(userId) == 1)
                    manager.Delete(id);
                return View("Index");
            }
            catch
            {
                return View("Index");
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(UserProfile user)
        {
            if (manager.IsValid(user.Username, user.password))
            {
                FormsAuthentication.SetAuthCookie(user.Username, false);
                Response.AppendCookie(new HttpCookie("UserId", manager.GetId(user.Username).ToString()));

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Login details are wrong.");
            }
            return View(user);
        }

        [Authorize]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();

            HttpCookie currentUserCookie = HttpContext.Request.Cookies["userId"];
            HttpContext.Response.Cookies.Remove("userId");
            currentUserCookie.Expires = DateTime.Now.AddDays(-10);
            currentUserCookie.Value = null;
            HttpContext.Response.SetCookie(currentUserCookie);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult CreateComment() 
        {
            try
            {
                int c_user = Int32.Parse(Request["UserId"].ToString());
                var rate = Int32.Parse(Request["rate"].ToString());
                var commentText = Request["Comment"].ToString();
                var username = "";

            if(Request.Cookies["UserId"] != null)
                username = commentManager.Create(Int32.Parse(Request.Cookies["UserId"].Value), c_user, rate, commentText);

            return Redirect("/UserProfile/Profile?username=" + username);
            }
            catch 
            {
                return Redirect(Request.UrlReferrer.ToString().Replace("&e=1", "") + "&e=1");
            }

        }
    }
}
