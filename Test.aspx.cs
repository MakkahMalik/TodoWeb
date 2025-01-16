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
namespace TODOweb
{
    public partial class Test : System.Web.UI.Page
    {
       
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Todo"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                DataTable dt = getData();
                Repeater1.DataSource = dt;
                Repeater1.DataBind();
            }
            else
            {
                // Handle the events when it's a postback
                string eventTarget = Request["__EVENTTARGET"];
                string eventArgument = Request["__EVENTARGUMENT"];

                if (eventTarget == "MarkAsDone")
                {
                    int itemId;
                    if (int.TryParse(eventArgument, out itemId))
                    {
                        // Call the method to mark the item as done in the database
                        MarkItemAsDoneInDatabase(itemId);
                    }
                    else
                    {
                        // Handle invalid itemId
                        Console.WriteLine("Invalid item ID.");
                    }
                }

                else if (eventTarget == "UpdateRowColor")
                {
                    UpdateRowColor(eventArgument);
                }
                else if (eventTarget == "UpdatePositions")
                {
                    if (!string.IsNullOrEmpty(eventArgument))
                    {
                        List<int> idsInOrder = JsonConvert.DeserializeObject<List<int>>(eventArgument);

                        UpdatePositions(idsInOrder);

                    }
                    else
                    {
                        Console.WriteLine("Empty eventArgument for UpdatePositions.");
                    }

                }
                else if (eventTarget == "DeleteRecord")
                {
                    // Parse the itemId from the event argument
                    if (int.TryParse(eventArgument, out int id))
                    {
                        DeleteRecord(id);
                    }
                    else
                    {
                       
                        Console.WriteLine("Invalid item ID.");
                    }
                }


                DataTable dt = getData();
                Repeater1.DataSource = dt;
                Repeater1.DataBind();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string mode = hdnMode.Value; 
            string description = add_new.Text;

            if (mode == "Update")
            {
                int listItemId = int.Parse(hdnItemId.Value);
                UpdateItemInDatabase(listItemId, description); 
            }
            else
            {
                AddNewItemToDatabase(description); 
            }

            add_new.Text = string.Empty;
            hdnItemId.Value = "0";
            hdnMode.Value = "Add";
            Repeater1.DataSource = getData();
            Repeater1.DataBind();
        }

        private void AddNewItemToDatabase(string description)
        {
            string des = add_new.Text;
            if (!string.IsNullOrWhiteSpace(des))
            {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Todo"].ConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            string getLastPositionQuery = "SELECT MAX(ItemPosition) FROM ListItems";
            SqlCommand getPositionCmd = new SqlCommand(getLastPositionQuery, con);
            object result = getPositionCmd.ExecuteScalar();

            int newPosition = result != DBNull.Value ? Convert.ToInt32(result) + 1 : 1;
            string query = "INSERT INTO ListItems (Description, ItemPosition, ListColor) VALUES (@Description, @ItemPosition, @ListColor)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Description", add_new.Text);
            cmd.Parameters.AddWithValue("@ItemPosition", newPosition);
            cmd.Parameters.AddWithValue("@ListColor", "red");
                cmd.ExecuteNonQuery();

          
                    add_new.Text = string.Empty;
                     DataTable dt = getData(); 
                    Repeater1.DataSource = dt;
                    Repeater1.DataBind();
                    con.Close();
            }
            else
            {
                string spanId = "errorSpan";
                string errorMessage = "Description cannot be empty";
                ClientScript.RegisterStartupScript(this.GetType(), "SetErrorMessage", $"document.getElementById('{spanId}').innerText = '{errorMessage}';",true );
            }

        }

        private void UpdateItemInDatabase(int listItemId, string newDescription)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Todo"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE ListItems SET Description = @Description WHERE ListItemId = @ListItemId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {   cmd.Parameters.AddWithValue("@Description", newDescription);
                    cmd.Parameters.AddWithValue("@ListItemId", listItemId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                   
                   
                }
        
            }


            Repeater1.DataSource = getData();
            Repeater1.DataBind();

        }
        DataTable getData()
        {
            SqlConnection con = new SqlConnection(connectionString);
            string query = "select * from ListItems order by itemPosition Asc";
            SqlDataAdapter sda = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            return dt;
        }
                  
        public static void UpdatePositions(List<int> idsInOrder)
         {
             string connectionString = ConfigurationManager.ConnectionStrings["Todo"].ConnectionString;

             using (SqlConnection con = new SqlConnection(connectionString))
             {
                 con.Open();

                 for (int i = 0; i < idsInOrder.Count; i++)
                 {
                     string query = "UPDATE ListItems SET ItemPosition = @ItemPosition WHERE ListItemId = @ListItemId";
                     using (SqlCommand cmd = new SqlCommand(query, con))
                     {
                         cmd.Parameters.AddWithValue("@ItemPosition", i + 1);   // Position starts from 1
                         cmd.Parameters.AddWithValue("@ListItemId", idsInOrder[i]);
                         cmd.ExecuteNonQuery();
                     }
               }
           }
        }
 
        private void UpdateRowColor(string eventArgument)
        {
            
                // Parse the row ID and selected color
                string[] args = eventArgument.Split(';');
                int rowId = int.Parse(args[0]);
                string selectedColor = args[1];

                // Update the database with the selected color
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "UPDATE ListItems SET ListColor = @ListColor WHERE ListItemId = @ListItemId";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ListColor", selectedColor);
                        cmd.Parameters.AddWithValue("@ListItemId", rowId);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Optionally, reload the data to reflect the change
                DataTable dt = getData();
                Repeater1.DataSource = dt;
                Repeater1.DataBind();
            
          
        }
        private void MarkAsDone(string listItemIdStr)
        {
            int listItemId;
            if (int.TryParse(listItemIdStr, out listItemId))
            {
                // Connection string to your database
                string connectionString = ConfigurationManager.ConnectionStrings["Todo"].ConnectionString;

                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();

                        // SQL query to update the IsDone field to true for the selected ListItem
                        string query = "UPDATE ListItems SET IsDone = @IsDone WHERE ListItemId = @ListItemId";

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            // Add parameters explicitly with correct types
                            cmd.Parameters.Add("@IsDone", SqlDbType.Bit).Value = true;  // Set IsDone to true
                            cmd.Parameters.Add("@ListItemId", SqlDbType.Int).Value = listItemId;

                            cmd.ExecuteNonQuery();  // Execute the query to update the database
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the error (you can use a logging library or EventViewer here)
                    // For simplicity, let's just show an error message
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
            else
            {
                // Handle the case when the item ID is invalid
                Console.WriteLine("Invalid ListItemId");
            }
        }

        private void MarkItemAsDoneInDatabase(int itemId)
        {
          MarkAsDone(itemId.ToString());
        }
        private void DeleteRecord(int itemId)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Todo"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string deleteQuery = "DELETE FROM ListItems WHERE ListItemId = @ItemId";

                using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                {
                    cmd.Parameters.AddWithValue("@ItemId", itemId);
                    cmd.ExecuteNonQuery();
                }

                DataTable dt = getData();
                Repeater1.DataSource = dt;
                Repeater1.DataBind();
            }


        }


        }
}