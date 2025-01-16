using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TODOweb.Models
{
    public class ListItem
    {
        public string Description { get; set; }
        public bool IsDone { get; set; }

        public int ItemPosition { get; set; }

        public string ListColor { get; set; }

        public DateTime CreateDt { get; set; }



    }
}