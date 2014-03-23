using Package2Go5.Models.ObjectManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Package2Go5.Controllers
{
    public class MessagesController : Controller
    {
        UserProfileManager userManager = new UserProfileManager();
        MessagesManager messagesManager = new MessagesManager();

        //
        // GET: /Messages/

        public ActionResult Index()
        {
            return View(messagesManager.GetAllUserMessages(User.Identity.Name));
        }

        //
        // GET: /Messages/Details/5

        public ActionResult Details(int id)
        {
            return View(messagesManager.GetUserMessagesById(id));
        }

        public ActionResult Create()
        {
            try
            {
                Request.Form.GetValues(0);

                var sendTo = Request.Form["to"];
                userManager.GetId(sendTo);
                messagesManager.Create(Int32.Parse(Request.Cookies["UserId"].Value), userManager.GetId(sendTo), Request.Form["message"]);

                return Redirect(System.Web.HttpContext.Current.Request.UrlReferrer.ToString());
            }
            catch
            {
                return Redirect(System.Web.HttpContext.Current.Request.UrlReferrer.ToString());
            }
        }

        //
        // GET: /Messages/Edit/5

        public void Edit(int id)
        {
            messagesManager.Update(id);
        }

        //
        // POST: /Messages/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Messages/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Messages/Delete/5

        [HttpPost]
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
    }
}
