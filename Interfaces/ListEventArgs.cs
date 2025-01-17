using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TODOweb.Interfaces
{
    public class ListEventArgs : EventArgs
    {
        public int ItemId { get; set; }  // Use nullable int for single item
        public List<int> ItemIds { get; set; } // Change to a list of IDs
        public string Description { get; set; }
        public string ListColor { get; set; }
        public bool? IsDone { get; set; }
        public string Action { get; set; } 
    }
}
