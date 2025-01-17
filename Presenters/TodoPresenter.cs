using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI.WebControls;
using TODOweb.Interfaces;
using TODOweb.Models;
using static TODOweb.Models.DatabaseHelper;

namespace TODOweb.Presenters
{
     public class TodoPresenter
    {

        private readonly IListItemView _view;

        private readonly DatabaseHelper model;

        public TodoPresenter(IListItemView view)
        {
            _view = view;
            model = new DatabaseHelper();
            _view.OnAddItem += AddItem;
            _view.OnDeleteItem += DeleteItem;
            _view.OnMarkAsDone += MarkAsDone;
            _view.OnUpdateRowColor += UpdateRowColor;
            //_view.OnUpdateItem += UpdateItem;
            //_view.OnDeleteItem += DeleteItem;
           
            //_view.OnUpdatePositions += UpdatePositions;
        }


        public void LoadData()
        {
            var data = model.GetData();
            _view.SetListData(data);
        }
        private void AddItem(object sender, ListEventArgs e)
        {
            model.AddItem(e.Description, e.ListColor);
            LoadData();
        }
        private void DeleteItem(object sender, ListEventArgs e)
        {
            model.DeleteItem(e.ItemId);
            LoadData();
        }

        private void MarkAsDone(object sender, ListEventArgs e)
        {
            model.MarkItemAsDone(e.ItemId);
            LoadData();
        }
       
        //private void UpdatePositions(object sender , ListEventArgs e)
        //{
        //    List<int> idsInOrder = new List<int> { e.ItemId};
        //    model.UpdatePositions(idsInOrder);
        //    LoadData();
        //}


        private void UpdateRowColor(object sender, ListEventArgs e)
        {

            model.UpdateRowColor(e.Action);
            LoadData();
        }



    }
}