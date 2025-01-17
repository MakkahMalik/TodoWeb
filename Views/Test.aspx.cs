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
        public event EventHandler<ListEventArgs> OnUpdatePositions;
        public event EventHandler<ListEventArgs> OnUpdateItem;
        public event EventHandler<ListEventArgs> OnUpdateRowColor;
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
 
                else if (eventTarget == "UpdateRowColor")
                {
                    UpdateRowColor(eventArgument);
                }

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
                args.ItemId = int.Parse(hdnItemId.Value);
                OnUpdateItem?.Invoke(this, args);
            }
            else
            {
                OnAddItem?.Invoke(this, args);
            }
            add_new.Text = string.Empty;
            hdnItemId.Value = "0";
            hdnMode.Value = "Add";
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

        //protected void UpdatePositions(List<int> idsInOrder)
        //{
        //    var args = new ListEventArgs { ItemIds = idsInOrder };
        //    OnUpdatePositions?.Invoke(this, args);
        //}

        protected void UpdateRowColor(string eventArgument)
        {
            var args = new ListEventArgs { Action = eventArgument };
            OnUpdateRowColor?.Invoke(this, args);
        }


    }
}