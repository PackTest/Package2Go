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
    
    public partial class UsersRoutes
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int route_id { get; set; }
    
        public virtual Routes Routes { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}
