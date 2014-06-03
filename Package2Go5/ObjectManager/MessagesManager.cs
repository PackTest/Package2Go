using Package2Go5.Models.ViewModels;
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
            Messages message = db.Messages.Where(m => m.id == id).FirstOrDefault();
            if (message.statusId != 3)
                message.statusId = 2;

            db.SaveChanges();
        }

        public List<vw_messages> GetNewUserMessages(string userId)
        {
            return db.vw_messages.Where(m => m.to == userId && m.status == "New").OrderByDescending(m=>m.date).ToList();
        }

        public List<MessagesView> GetAllUserMessages(string userId)
        {
            return AutoMapper.Mapper.Map<List<vw_messages>, List<MessagesView>>(db.vw_messages.Where(m => m.to == userId).OrderByDescending(m => m.date).Take(100).ToList());
        }

        public MessagesView GetUserMessagesById(int id)
        {
            return AutoMapper.Mapper.Map<vw_messages, MessagesView>(db.vw_messages.Where(m => m.id == id).First());
        }

        public void Remove(int id) 
        {
            db.UsersMessages.RemoveRange(db.UsersMessages.Where(um => um.messageId == id));

            db.Messages.Remove(db.Messages.Where(m => m.id == id).First());
            db.SaveChanges();
        }

        public List<MessagesView> GetAllMessages() 
        {
            return AutoMapper.Mapper.Map<List<vw_messages>, List<MessagesView>>(db.vw_messages.ToList());
        }

    }
}