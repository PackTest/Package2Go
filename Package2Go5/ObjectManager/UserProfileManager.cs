using System.Linq;
using Package2Go5.Models.ViewModels;
using Package2Go5.Models;
using System.Data.Entity;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.IO;
using Package2Go5.Models.EditModels;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using Package2Go5.Constants;
using System.Web.Mvc;
using System.Collections.Generic;

namespace Package2Go5.Models.ObjectManager
{
    public class UserProfileManager
    {
        private Package2GoEntities db = new Package2GoEntities();

        public void Create(UserProfileView user)
        {
            UserProfile userProfile = new UserProfile();

            userProfile.Username = HttpUtility.HtmlEncode(user.Username);
            userProfile.password = CreateHash(user.password);
            userProfile.name = HttpUtility.HtmlEncode(user.name);
            userProfile.lastname = HttpUtility.HtmlEncode(user.lastname);
            userProfile.phone_nr = user.phone_nr;
            userProfile.gender = HttpUtility.HtmlEncode(((CProfile.genderType)user.gender).ToString());
            userProfile.birthday = user.birthday;
            userProfile.rate = user.rate;
            userProfile.UsersRoutes = new Collection<UsersRoutes>();
            userProfile.currency_id = user.currency_id;

            userProfile.Roles = new List<Roles>();
            userProfile.Roles.Add(db.Roles.Where(r => r.RoleId == 2).First());

            db.UserProfile.Add(userProfile);
            //db.use.Add(userProfile);
            db.SaveChanges();
        }

        public bool doesUserNameExist(string UserName)
        {
            try
            {
                db.UserProfile.Where(up => up.Username == UserName).First();
                return false;
            }
            catch 
            {
                return true;
            }
        }

        public void Update(UserProfileEdit user) 
        {
            UserProfile userProfile = db.UserProfile.FirstOrDefault(u => u.Username == user.Username);

            if (MatchHash(userProfile.password, user.password))
            {

                if (!String.IsNullOrEmpty(user.newPassword))
                    userProfile.password = CreateHash(user.newPassword);

                userProfile.name = HttpUtility.HtmlEncode(user.name);
                userProfile.lastname = HttpUtility.HtmlEncode(user.lastname);
                userProfile.phone_nr = user.phone_nr;
                userProfile.gender = HttpUtility.HtmlEncode(((CProfile.genderType)user.gender).ToString());
                userProfile.currency_id = user.currency_id;

                if (user.image != null && user.image.ContentLength > 0)
                {

                    using (var img = Image.FromStream(user.image.InputStream))
                    {

                        if (img.RawFormat.Equals(ImageFormat.Png) || img.RawFormat.Equals(ImageFormat.Jpeg) || img.RawFormat.Equals(ImageFormat.Gif))
                        {
                            string path = System.Web.HttpContext.Current.Server.MapPath("~/Images/Profiles");
                            string fileName = user.Username + Path.GetExtension(user.image.FileName);

                            user.image.SaveAs(Path.Combine(path, fileName));

                            userProfile.image_url = fileName;
                        }
                    }
                }
            }
            else
            {
                userProfile.name = null;
            }
            db.SaveChanges();
        }

        public Bitmap ProportionallyResizeBitmap(Bitmap src, int maxWidth, int maxHeight)
        {

            // original dimensions
            int w = src.Width;
            int h = src.Height;

            // Longest and shortest dimension
            int longestDimension = (w > h) ? w : h;
            int shortestDimension = (w < h) ? w : h;

            // propotionality
            float factor = ((float)longestDimension) / shortestDimension;

            // default width is greater than height
            double newWidth = maxWidth;
            double newHeight = maxWidth / factor;

            // if height greater than width recalculate
            if (w < h)
            {
                newWidth = maxHeight / factor;
                newHeight = maxHeight;
            }

            // Create new Bitmap at new dimensions
            Bitmap result = new Bitmap((int)newWidth, (int)newHeight);
            using (Graphics g = Graphics.FromImage((System.Drawing.Image)result))
                g.DrawImage(src, 0, 0, (int)newWidth, (int)newHeight);

            return result;
        }

        public UserProfile Get(string userName)
        {
            return db.UserProfile.FirstOrDefault(u => u.Username == userName);
        }

        public int GetId(string userName)
        {
            return db.UserProfile.Select(u => new { u.UserId, u.Username }).Where(u => u.Username == userName).First().UserId;
        }

        public string CreateHash(string unHashed)
        {
            SHA512CryptoServiceProvider sha5 = new SHA512CryptoServiceProvider();
            byte[] data = System.Text.Encoding.Default.GetBytes(unHashed);
            data = sha5.ComputeHash(data);
            return System.Text.Encoding.Default.GetString(data);
        }

        public bool MatchHash(string HashData, string HashUser)
        {
            HashUser = CreateHash(HashUser);
            if (HashUser == HashData)
                return true;
            else
                return false;
        }

        public IEnumerable<string> GetAllUsernames() 
        {
            return db.UserProfile.Select(u => u.Username).ToList();
        }

        public bool IsValid(string username, string password)
        {
            bool IsValid = false;

            using (var db = new Package2GoEntities())
            {
                var user = db.UserProfile.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    if (MatchHash(user.password, password))
                    {
                        IsValid = true;
                    }
                }
            }
            return IsValid;
        }

        public int getRole(int id) 
        {
            return db.UserProfile.Where(u => u.UserId == id).First().Roles.First().RoleId;
        }

    }
}
