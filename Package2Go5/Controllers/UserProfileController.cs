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

namespace Package2Go5.Controllers
{
    public class UserProfileController : Controller
    {

        UserProfileManager manager = new UserProfileManager();
        CurrencyManager currencyManager = new CurrencyManager();

        //
        // GET: /UserProfile/

        public ActionResult Index()
        {
            return View(manager.Get(User.Identity.Name));
        }

        //
        // GET: /UserProfile/Details/5

        public ActionResult Profile(string username)
        {
            return View(manager.Get(username));
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

                Mapper.CreateMap<UserProfileView, UserProfile>();
                UserProfile user = Mapper.Map<UserProfileView, UserProfile>(viewModel);

                LogIn(user);

                return RedirectToAction("Login");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /UserProfile/Edit/5

        [Authorize]
        public ActionResult Edit()
        {

            Mapper.CreateMap<UserProfile, UserProfileEdit>()
                .ForMember(dest => dest.gender, opt => opt.MapFrom(
                    src => (CProfile.genderType)Enum.Parse(typeof(CProfile.genderType), src.gender)));

            UserProfile user = manager.Get(User.Identity.Name);
            UserProfileEdit model = Mapper.Map<UserProfile, UserProfileEdit>(user);

            ViewBag.currencies = currencyManager.GetCurrenciesList();

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
                manager.Update(viewModel);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /UserProfile/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /UserProfile/Delete/5

        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
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
            return RedirectToAction("Index", "Home");
        }
    }
}
