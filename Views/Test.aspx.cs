using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Optimization;
using System.Data;
using System.Web.Services;
using System.Web.Mvc;
using Newtonsoft.Json;
using Microsoft.Ajax.Utilities;
using TODOweb.Interfaces;
using TODOweb.Presenters;
namespace TODOweb
{
    public partial class Test : System.Web.UI.Page , IListItemView
    {
        public event EventHandler<ListEventArgs> OnAddItem;
        public event EventHandler<ListEventArgs> OnDeleteItem;
        public event EventHandler<ListEventArgs> OnMarkAsDone;

        private readonly TodoPresenter _presenter;
        public Test()
        {
            _presenter = new TodoPresenter(this);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _presenter.LoadData();
            }
            else
            {
                string eventTarget = Request["__EVENTTARGET"];
                string eventArgument = Request["__EVENTARGUMENT"];
                if (Request["__EVENTTARGET"] == "DeleteRecord")
                {
                    int id = int.Parse(Request["__EVENTARGUMENT"]);
                    DeleteRecord(id); // Call your existing DeleteRecord method
                }
                else if (eventTarget == "MarkAsDone")
                {
                    int id = int.Parse(Request["__EVENTARGUMENT"]);
                    MarkAsDone(id);

                }

                // Handle the events when it's a postback
                //string eventTarget = Request["__EVENTTARGET"];
                //string eventArgument = Request["__EVENTARGUMENT"];

                //if (eventTarget == "MarkAsDone")
                //{
                //    int itemId;
                //    if (int.TryParse(eventArgument, out itemId))
                //    {
                //        // Call the method to mark the item as done in the database
                //        MarkItemAsDoneInDatabase(itemId);
                //    }
                //    else
                //    {
                //        // Handle invalid itemId
                //        Console.WriteLine("Invalid item ID.");
                //    }
                //}

                //else if (eventTarget == "UpdateRowColor")
                //{
                //    UpdateRowColor(eventArgument);
                //}
                //else if (eventTarget == "UpdatePositions")
                //{
                //    if (!string.IsNullOrEmpty(eventArgument))
                //    {
                //        List<int> idsInOrder = JsonConvert.DeserializeObject<List<int>>(eventArgument);

                //        UpdatePositions(idsInOrder);

                //    }
                //    else
                //    {
                //        Console.WriteLine("Empty eventArgument for UpdatePositions.");
                //    }

                //}
                //else if (eventTarget == "DeleteRecord")
                //{
                //    // Parse the itemId from the event argument
                //    if (int.TryParse(eventArgument, out int id))
                //    {
                //        DeleteRecord(id);
                //    }
                //    else
                //    {
                //        Console.WriteLine("Invalid item ID.");
                //    }
                //}


                //DataTable dt = getData();
                //Repeater1.DataSource = dt;
                //Repeater1.DataBind();
            }
        }

        public void SetListData(DataTable data)
        {
            Repeater1.DataSource = data;
            Repeater1.DataBind();
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string mode = hdnMode.Value;
            string description = add_new.Text;

            var args = new ListEventArgs { Description = description, ListColor = "red" };

            if (mode == "Update")
            {
                //args.ItemId = int.Parse(hdnItemId.Value);
                //OnUpdateItem?.Invoke(this, args);
            }
            else
            {
                OnAddItem?.Invoke(this, args);
            }
        }

        protected void DeleteRecord(int id)
        {
            var args = new ListEventArgs { ItemId = id };
            OnDeleteItem?.Invoke(this, args);
        }

        protected void MarkAsDone(int id)
        {
            var args = new ListEventArgs { ItemId = id };
            OnMarkAsDone?.Invoke(this, args);
        }






    }
}