//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Package2Go5
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserProfile
    {
        public UserProfile()
        {
            this.UsersItems = new HashSet<UsersItems>();
            this.UsersRoutes = new HashSet<UsersRoutes>();
            this.Roles = new HashSet<Roles>();
        }
    
        public int UserId { get; set; }
        public string Username { get; set; }
        public string password { get; set; }
        public string name { get; set; }
        public string lastname { get; set; }
        public string gender { get; set; }
        public System.DateTime birthday { get; set; }
        public int phone_nr { get; set; }
        public Nullable<int> rate { get; set; }
        public string image_url { get; set; }
        public int currency_id { get; set; }
    
        public virtual Currencies Currencies { get; set; }
        public virtual ICollection<UsersItems> UsersItems { get; set; }
        public virtual ICollection<UsersRoutes> UsersRoutes { get; set; }
        public virtual ICollection<Roles> Roles { get; set; }
    }
}