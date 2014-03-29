using Package2Go5.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Package2Go5.Models.ObjectManager
{
    public class ItemsManager
    {
        private Package2GoEntities db = new Package2GoEntities();

        public void Create(ItemsView itemView, int userId)
        {
            Items item = new Items();

            item.title = HttpUtility.HtmlEncode(itemView.title);
            item.delivery_address = HttpUtility.HtmlEncode(itemView.delivery_address);
            item.size = itemView.size;
            item.delivery_price = itemView.delivery_price;
            item.note = HttpUtility.HtmlEncode(itemView.note);
            item.currency_id = itemView.currency_id;

            item.UsersItems.Add(new UsersItems { item_id = item.id, user_id = userId });

            db.Items.Add(item);
            db.SaveChanges();
        }

        public void Update(int id, ItemsView itemView)
        {
            Items item = db.Items.FirstOrDefault(u => u.id == id);

            item.title = HttpUtility.HtmlEncode(itemView.title);
            item.delivery_address = HttpUtility.HtmlEncode(itemView.delivery_address);
            item.size = itemView.size;
            item.delivery_price = itemView.delivery_price;
            item.note = HttpUtility.HtmlEncode(itemView.note);

            db.SaveChanges();
        }

        public void Delete(int id) 
        {
            foreach (UsersItems ui in db.UsersItems.Where(ui => ui.item_id == id)) 
            {
                db.UsersItems.Remove(ui);
            }
            db.Items.Remove(GetItem(id));
            db.SaveChanges();
        }

        public Items GetItem(int id)
        {
            return db.Items.Where(r => r.id == id).First();
        }

        public List<Items> GetAll( int userId = 0, int routeId = 0)
        {
            if (userId != 0)
            {
                return db.Items.Where(i => i.UsersItems.Any()).ToList();
            }
            if (routeId == 0)
                return db.Items.ToList();
            else
                return db.Items.Where(i=>!i.ItemsRoutes.Any()).ToList();
        }

        public List<Items> GetAllUserItems(int userId)
        {
            return db.Items.Where(i => i.UsersItems.Any(ui=>ui.user_id == userId)).ToList();
        }

        public SelectList GetNotUsedUserItemsList(int userId)
        {
            List<KeyValuePair<string, int>> userItems = new List<KeyValuePair<string, int>>();

            foreach (Items item in db.Items.Where(i => i.UsersItems.Any(ui => ui.user_id == userId) && !i.ItemsRoutes.Any()).ToList())
            {
                userItems.Add(new KeyValuePair<string, int>(item.title, item.id));
            }

            return new SelectList(userItems, "Value", "Key");
        }

        public Dictionary<int, int> GetPriceUserVal(List<Items> items, int userCur) 
        {
            Dictionary<int, int> pricesUserVal = new Dictionary<int, int>();

            Dictionary<int, decimal> currencies = new Dictionary<int, decimal>();

            foreach (Items item in items) 
            {
                if (userCur != item.currency_id) 
                {
                    if(!currencies.ContainsKey(userCur))
                    {
                        currencies[userCur] = db.Currencies.Where(c=>c.id == userCur).First().currency_rate;
                    }
                    if(!currencies.ContainsKey(item.currency_id))
                    {
                        currencies[item.currency_id] = db.Currencies.Where(c => c.id == item.currency_id).First().currency_rate;
                    }

                    pricesUserVal.Add(item.id, Decimal.ToInt32(item.delivery_price * currencies[userCur] / currencies[item.currency_id]));
                }
                else
                    pricesUserVal.Add(item.id, item.delivery_price);
            }

            return pricesUserVal;
        }
    }
}