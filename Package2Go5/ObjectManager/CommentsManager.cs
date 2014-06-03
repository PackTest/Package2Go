using AutoMapper;
using Package2Go5.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Package2Go5.Models.ObjectManager
{
    public class CommentsManager
    {
        private Package2GoEntities db = new Package2GoEntities();

        //c_user_id user who will get commnet id
        public string Create(int user_id, int c_user_id, int rate, string commentText)
        {
            UserProfile user = db.UserProfile.Where(up => up.UserId == c_user_id).First();

            //Jeigu yra pasiūlimas kur kelias priklauso kuriam nors useriui ir itemas priklauso kuriam nors useriui
            //int commentCount = db.Offers.Where(o => o.Routes.UsersRoutes.Any(ur => ur.user_id == c_user_id || ur.user_id == user_id) 
            //    && o.Items.UsersItems.Any(ui => ui.user_id == c_user_id || ui.user_id == user_id)).Count();

            //Jei užbaigtas tiek kelias, tiek itemas
            int commentCount = db.ItemsRoutes.Where(ir => ir.Items.UsersItems.Any(ui => ui.user_id == user_id || ui.user_id == c_user_id) 
                && ir.Routes.UsersRoutes.Any(ur => ur.user_id == user_id || ur.user_id == c_user_id) && ir.Items.status_id == 3 && ir.Routes.status_id == 3).Count();

            //int curCommentCount = db.Comments.Where(c => c.user_id == user_id && c.writer_id == c_user_id || c.user_id == c_user_id && c.writer_id == user_id).Count();
            int curCommentCount = db.Comments.Where(c => c.user_id == c_user_id && c.writer_id == user_id).Count();

            Comments comment = new Comments();

            if (commentCount > curCommentCount && c_user_id != db.UserProfile.Where(up=>up.UserId == user_id).Select(up=>up.UserId).First())
            {
                comment.user_id = user.UserId;
                comment.writer_id = user_id;
                comment.comment = commentText;
                comment.date = DateTime.Now;

                user.rate = (user.rate * curCommentCount + rate) / (curCommentCount + 1);
            }
            db.Comments.Add(comment);
            db.SaveChanges();
            
            return user.Username;
        }

        public List<CommentsView> GetComments(string username) 
        {
            List<Comments> comments = db.Comments.Where(c => c.UserProfile.Username == username).OrderByDescending(c=>c.date).ToList();
            List<CommentsView> commnetsView = Mapper.Map<List<Comments>, List<CommentsView>>(comments);

            foreach (CommentsView comment in commnetsView) 
            {
                //comment.user = comments.Where(up => up.user_id == comment.user_id).First().UserProfile.Username;
                //comment.writer = comments.Where(up => up.writer_id == comment.writer_id).First().UserProfile.Username;
                comment.writer = db.UserProfile.Where(up => up.UserId == comment.writer_id).First().Username;
            }

            return commnetsView;
        }

        public int GetCommentsCount(int user_id)
        {
            return db.Comments.Where(c => c.user_id == user_id).Count();
        }

        public List<CommentsView> GetAll()
        {
            return Mapper.Map<List<Comments>, List<CommentsView>>(db.Comments.ToList());
        }

        public void Delete(int id) 
        {
            db.Comments.Remove(db.Comments.Where(c=>id == id).FirstOrDefault());
            db.SaveChanges();
        }

    }
}