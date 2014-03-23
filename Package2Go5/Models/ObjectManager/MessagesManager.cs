using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Package2Go5.Models.ObjectManager
{
    public class MessagesManager
    {
        private Package2GoEntities db = new Package2GoEntities();

        public void Create(int from, int to, string text) 
        {
            Messages message = new Messages();

            message.from = from;
            message.to = to;
            message.message = text;
            message.date = DateTime.Now;
            message.statusId = 1;

            db.Messages.Add(message);
            db.UsersMessages.Add(new UsersMessages { messageId = message.id, userId = message.to });
            db.UsersMessages.Add(new UsersMessages { messageId = message.id, userId = message.from });

            db.SaveChanges();
        }

        public void Update(int id)
        {
            db.Messages.Where(m => m.id == id).First().statusId = 2;

            db.SaveChanges();
        }

        public List<vw_messages> GetNewUserMessages(string userId)
        {
            return db.vw_messages.Where(m => m.to == userId && m.status == "New").OrderByDescending(m=>m.date).ToList();
        }

        public List<vw_messages> GetAllUserMessages(string userId)
        {
            return db.vw_messages.Where(m => m.to == userId).OrderByDescending(m=>m.date).ToList();
        }

        public vw_messages GetUserMessagesById(int id)
        {
            return db.vw_messages.Where(m => m.id == id).First();
        }

    }
}