using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;

namespace Package2Go5.Models.ViewModels
{
    public class CommentsView
    {

        public int id { get; set; }

        //[Display(ResourceType = typeof(Resources.DisplayNames), Name = "title")]
        public string user { get; set; }

        public int user_id { get; set; }
        public int writer_id { get; set; }

        public string writer { get; set; }
        public string comment { get; set; }
        public System.DateTime date { get; set; }

    }
}
