using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Package2Go5.Models.ObjectManager
{
    public class CurrencyManager
    {
        private Package2GoEntities db = new Package2GoEntities();

        public void UpdateCurrencies() 
        {
            //Import from eurobank
            List<decimal> currency = new List<decimal>();

            string url = "http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";
            XDocument doc = XDocument.Load(url);

            XNamespace gesmes = "http://www.gesmes.org/xml/2002-08-01";
            XNamespace ns = "http://www.ecb.int/vocabulary/2002-08-01/eurofxref";

            var cubes = doc.Descendants(ns + "Cube")
                           .Where(x => x.Attribute("currency") != null)
                           .Select(x => new
                           {
                               Currency = (string)x.Attribute("currency"),
                               Rate = (decimal)x.Attribute("rate")
                           });

            //Update database
            foreach (var result in cubes)
            {
                db.Currencies.Where(c => c.code == result.Currency).First().currency_rate = result.Rate;
            }

            db.SaveChanges();
        }

        public SelectList GetCurrenciesList(int selected = 0)
        {
            List<KeyValuePair<string, int>> currenciesList = new List<KeyValuePair<string, int>>();

            foreach (Currencies currency in db.Currencies.ToList())
            {
                currenciesList.Add(new KeyValuePair<string, int>(currency.code, currency.id));
            }

            SelectList currenciesSelectList = new SelectList(currenciesList, "Value", "Key");

            return currenciesSelectList;
        }

        public Currencies GetCurrency(int user)
        {
            return db.UserProfile.Where(u => u.UserId == user).First().Currencies;
        }

    }
}