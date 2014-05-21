using AutoMapper;
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
            item.address = HttpUtility.HtmlEncode(itemView.address);
            item.delivery_address = HttpUtility.HtmlEncode(itemView.delivery_address);
            item.size = itemView.size;
            item.delivery_price = itemView.delivery_price;
            item.note = HttpUtility.HtmlEncode(itemView.note);
            item.currency_id = itemView.currency_id;
            item.delivery_date = itemView.delivery_date;
            item.status_id = 1;

            item.UsersItems.Add(new UsersItems { item_id = item.id, user_id = userId });

            db.Items.Add(item);
            db.SaveChanges();
        }

        public void Update(int id, ItemsView itemView)
        {
            Items item = db.Items.FirstOrDefault(u => u.id == id);

            item.title = HttpUtility.HtmlEncode(itemView.title);
            item.address = HttpUtility.HtmlEncode(itemView.address);
            item.delivery_address = HttpUtility.HtmlEncode(itemView.delivery_address);
            item.size = itemView.size;
            item.delivery_price = itemView.delivery_price;
            item.delivery_date = itemView.delivery_date;
            item.note = HttpUtility.HtmlEncode(itemView.note);
            item.status_id = itemView.status_id;

            db.SaveChanges();
        }

        public void Delete(int id) 
        {
            foreach (UsersItems ui in db.UsersItems.Where(ui => ui.item_id == id)) 
            {
                db.UsersItems.Remove(ui);
            }

            foreach (ItemsRoutes ir in db.ItemsRoutes.Where(ir => ir.item_id == id)) 
            {
                db.ItemsRoutes.Remove(ir);                
            }
            foreach (Offers o in db.Offers.Where(o => o.item_id == id)) 
            {
                db.Offers.Remove(o);
            }

            db.Items.Remove(db.Items.Where(i=>i.id==id).First());
            db.SaveChanges();
        }

        public ItemsView GetItem(int id)
        {
            var item = Mapper.Map<Items, ItemsView>(db.Items.Where(r => r.id == id).First());

            item.owner = db.UserProfile.Where(up => up.UsersItems.Any(ui => ui.item_id == item.id)).First().Username;

            return item;
        }

        public List<Items> GetAll( int userId = 0, int routeId = 0)
        {
            if (userId != 0 && routeId != -1)
            {
                return db.Items.Where(i => i.UsersItems.Any()).ToList();
            }
            if (routeId == 0)
                return db.Items.ToList();
            else if (routeId != -1)
                return db.Items.Where(i=>!i.ItemsRoutes.Any()).ToList();
            else
                return db.Items.Where(i => i.UsersItems.Any(ui=>ui.user_id != userId) && (i.status_id==1 || (i.ItemsRoutes.Any(ir=>ir.Routes.UsersRoutes.Any(ur=>ur.user_id == userId)) && i.status_id == 2))).ToList();
        }

        public List<ItemsView> GetAllHistory(int userId)
        {
            List<ItemsView> itemsView = Mapper.Map<List<Items>, List<ItemsView>>(db.Items.Where(i => i.status_id == 3 && i.UsersItems.Any(ui=>ui.user_id == userId)).ToList());

            Routes route = null;

            foreach (ItemsView item in itemsView)
            {
                route = db.Routes.Where(r => r.ItemsRoutes.Any(ir => ir.item_id == item.id)).FirstOrDefault();
                if (route == null)
                    continue;
                item.date = route.departure_time;
                item.route_id = route.id;
            }


            return itemsView;
        }

        public List<Items> GetAllUserItems(int userId)
        {
            return db.Items.Where(i => i.UsersItems.Any(ui=>ui.user_id == userId) && i.status_id != 3).ToList();
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

        public SelectList getStatusList(int status_id) 
        {
            List<SelectListItem> status = new List<SelectListItem>();
            bool selected = false;
            
            if(status_id == -1)
                status.Add(new SelectListItem { Text = "Status", Value = "", Selected = selected });

            foreach (ItemStatus stat in db.ItemStatus) 
            {
                if (stat.id == status_id)
                    selected = true;
                else
                    selected = false;
                if (status_id != -1)
                    status.Add(new SelectListItem { Text = stat.title, Value = stat.id.ToString(), Selected = selected });
                else
                    status.Add(new SelectListItem { Text = stat.title, Value = stat.title, Selected = selected });
            }

            return new SelectList(status, "Value", "Text");
        }
    }
}