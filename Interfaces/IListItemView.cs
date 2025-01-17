using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace TODOweb.Interfaces
{
    public interface IListItemView
    {


        event EventHandler<ListEventArgs> OnAddItem;
        event EventHandler<ListEventArgs> OnUpdateItem;
        event EventHandler<ListEventArgs> OnDeleteItem;
        event EventHandler<ListEventArgs> OnMarkAsDone;
        event EventHandler<ListEventArgs> OnUpdateRowColor;
        //event EventHandler<ListEventArgs> OnUpdatePositions;

        void SetListData(DataTable data);    

    }
}
