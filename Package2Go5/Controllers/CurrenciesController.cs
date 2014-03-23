using Package2Go5.Models.ObjectManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Package2Go5.Controllers
{
    public class CurrenciesController : Controller
    {
        CurrencyManager manager = new CurrencyManager();
        public ActionResult UploadNewCurrencies()
        {
            manager.UpdateCurrencies();
            return RedirectToAction("Index", "Home");
        }
    }
}
