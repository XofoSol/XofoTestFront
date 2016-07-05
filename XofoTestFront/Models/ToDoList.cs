using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace XofoTestFront.Models
{
    public class ToDoList
    {
        public int Id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public System.DateTime due { get; set; }
        public bool done { get; set; }
        public string UserId { get; set; }

    }

}