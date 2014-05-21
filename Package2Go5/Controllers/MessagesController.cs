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
        private UserProfileManager userManager = new UserProfileManager();
        private MessagesManager messagesManager = new MessagesManager();
        private int userId = 0;

        //
        // GET: /Messages/
        [Authorize]
        public ActionResult Index()
        {
            List<SelectListItem> type = new List<SelectListItem>();
            type.Add(new SelectListItem{ Text = "Type", Value = "" });
            type.Add(new SelectListItem { Text = "Notification", Value = "1" });
            type.Add(new SelectListItem { Text = "Messages", Value = "2" });
            ViewBag.type = new SelectList(type, "Value", "Text");
            return View(messagesManager.GetAllUserMessages(User.Identity.Name));
        }

        [Authorize]
        public ActionResult Messages()
        {
            return View(messagesManager.GetAllMessages());
        }

        //
        // GET: /Messages/Details/5

        public ActionResult Details(int id)
        {
            return View(messagesManager.GetUserMessagesById(id));
        }

        [Authorize]
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

        public bool Delete(int id)
        {
            try
            {
                messagesManager.Remove(id);
                return true;
            }
            catch {
                return false;
            }
        }

        //
        // POST: /Messages/Delete/5

        [Authorize]
        public ActionResult DeleteMessage(int id)
        {
            try
            {
                if (Request.Cookies["UserId"] != null)
                    userId = Int32.Parse(Request.Cookies["UserId"].Value);
                if (User.Identity.IsAuthenticated && userManager.getRole(userId) == 1)
                    messagesManager.Remove(id);
                return RedirectToAction("Messages");
            }
            catch
            {
                return View();
            }
        }
    }
}
