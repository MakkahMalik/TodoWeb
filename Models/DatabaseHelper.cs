using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common;
using System.Data.Common;

namespace TODOweb.Models
{
    public class DatabaseHelper
    {

        private readonly Database db;

        public DatabaseHelper()
        {
            var factory = new DatabaseProviderFactory();
            db = factory.Create("Todo");
        }


            public DataTable GetData()
            {

            string query = "SELECT * FROM ListItems ORDER BY ItemPosition ASC";
            DbCommand dbCommand = db.GetSqlStringCommand(query);

            using (IDataReader reader = db.ExecuteReader(dbCommand))
            {
                DataTable dt = new DataTable();
                dt.Load(reader);
                return dt;
            }
        }

            public void AddItem(string description, string listColor)
            {
         
            if (!string.IsNullOrWhiteSpace(description))
            {
               
                    string getLastPositionQuery = "SELECT MAX(ItemPosition) FROM ListItems";
                    DbCommand getPositionCmd = db.GetSqlStringCommand(getLastPositionQuery);
                    object result = db.ExecuteScalar(getPositionCmd);

                    int newPosition = result != DBNull.Value ? Convert.ToInt32(result) + 1 : 1;
                    string query = "INSERT INTO ListItems (Description, ItemPosition, ListColor) VALUES (@Description, @ItemPosition, @ListColor)";
                using (DbCommand cmd = db.GetSqlStringCommand(query))
                {
                    db.AddInParameter(cmd, "@Description", DbType.String, description);
                    db.AddInParameter(cmd, "@ItemPosition", DbType.Int32, newPosition);
                    db.AddInParameter(cmd, "@ListColor", DbType.String, "red");
                    db.ExecuteNonQuery(cmd);
                }

            }
            else
                {
                //    string spanId = "errorSpan";
                //    string errorMessage = "Description cannot be empty";
                //page.ClientScript.RegisterStartupScript(page.GetType(), "SetErrorMessage",
                //$"document.getElementById('{spanId}').innerText = '{errorMessage}';", true);
            }
            
            
            }

        public void UpdateItem(int itemId, string description)
            {
            try
            {
                string query = "UPDATE ListItems SET Description = @Description WHERE ListItemId = @ListItemId";
                using (DbCommand cmd = db.GetSqlStringCommand(query))
                {
                    db.AddInParameter(cmd, "@Description", DbType.String, description);
                    db.AddInParameter(cmd, "@ListItemId", DbType.Int32, itemId);
                    db.ExecuteNonQuery(cmd);
                }

            }  catch(Exception ex)
            {
                   LogErrorToDatabase(ex);
            } 
            

        }

        public void MarkItemAsDone(int itemId)
            {
            try
            {
                string query = "UPDATE ListItems SET IsDone = @IsDone WHERE ListItemId = @ListItemId";

                using (DbCommand cmd = db.GetSqlStringCommand(query))
                {
                    db.AddInParameter(cmd, "@IsDone", DbType.Boolean, true);
                    db.AddInParameter(cmd, "@ListItemId", DbType.Int32, itemId);
                    db.ExecuteNonQuery(cmd);
                }
            }catch(Exception ex) {
                LogErrorToDatabase(ex);
            }   
            }

        public void DeleteItem(int itemId)
            {

            try
            {
                string query = "DELETE FROM ListItems WHERE ListItemId = @ItemId";
                using (DbCommand cmd = db.GetSqlStringCommand(query))
                {
                    db.AddInParameter(cmd, "@ItemId", DbType.Int32, itemId);
                    db.ExecuteNonQuery(cmd);
                }
            }catch(Exception ex)
            {
                LogErrorToDatabase(ex);

            }




        }

        public void UpdateItemPositions(List<int> idsInOrder)
            {
            try
            {
                
                for (int i = 0; i < idsInOrder.Count; i++)
                {
                    string query = "UPDATE ListItems SET ItemPosition = @ItemPosition WHERE ListItemId = @ListItemId";

                    using (DbCommand cmd = db.GetSqlStringCommand(query))
                    {
                        db.AddInParameter(cmd, "@ItemPosition", DbType.Int32, i + 1);
                        db.AddInParameter(cmd, "@ListItemId", DbType.Int32, idsInOrder[i]);
                        db.ExecuteNonQuery(cmd);
                    }
                }

            }
            catch (Exception ex)
            {

                LogErrorToDatabase(ex);


            }
        }

        public void UpdateRowColor(string eventArgument)
        {
            try
            {
                var factory = new DatabaseProviderFactory();
                Database db = factory.Create("todo");

                string[] args = eventArgument.Split(';');
                int rowId = int.Parse(args[0]);
                string selectedColor = args[1];


                string query = "UPDATE ListItems SET ListColor = @ListColor WHERE ListItemId = @ListItemId";

                using (DbCommand cmd = db.GetSqlStringCommand(query))
                {
                    db.AddInParameter(cmd, "@ListColor", DbType.String, selectedColor);
                    db.AddInParameter(cmd, "@ListItemId", DbType.Int32, rowId);
                    db.ExecuteNonQuery(cmd);
                }

            }
            catch (Exception ex)
            {

                LogErrorToDatabase(ex);

            }



        }

        public static void LogErrorToDatabase(Exception ex)
         {

            var factory = new DatabaseProviderFactory();
            Database db = factory.Create("todo");

            string query = "INSERT INTO ErrorLogs (DateTime, Message, StackTrace) " +
                           "VALUES (@DateTime, @Message, @StackTrace)";

            using (DbCommand cmd = db.GetSqlStringCommand(query))
            {
                // Add parameters to insert error details
                db.AddInParameter(cmd, "@DateTime", DbType.DateTime, DateTime.Now);
                db.AddInParameter(cmd, "@Message", DbType.String, ex.Message); // Error message
                db.AddInParameter(cmd, "@StackTrace", DbType.String, ex.StackTrace); // Full stack trace

                // Execute the insert query
                db.ExecuteNonQuery(cmd);
            }
         }




    }


}
