using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace TODOweb.Models
{
    public class DatabaseHelper
    {
       
            private string connectionString = ConfigurationManager.ConnectionStrings["Todo"].ConnectionString;

            public DataTable GetData()
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM ListItems ORDER BY ItemPosition ASC";
                    SqlDataAdapter sda = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    return dt;
                }
            }

            public void AddItem(string description, string listColor)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string getLastPositionQuery = "SELECT MAX(ItemPosition) FROM ListItems";
                    SqlCommand getPositionCmd = new SqlCommand(getLastPositionQuery, con);
                    object result = getPositionCmd.ExecuteScalar();

                    int newPosition = result != DBNull.Value ? Convert.ToInt32(result) + 1 : 1;
                    string query = "INSERT INTO ListItems (Description, ItemPosition, ListColor) VALUES (@Description, @ItemPosition, @ListColor)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@ItemPosition", newPosition);
                    cmd.Parameters.AddWithValue("@ListColor", listColor);
                    cmd.ExecuteNonQuery();
                }
            }

            public void UpdateItem(int itemId, string description)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE ListItems SET Description = @Description WHERE ListItemId = @ListItemId";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@ListItemId", itemId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            public void MarkItemAsDone(int itemId)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE ListItems SET IsDone = @IsDone WHERE ListItemId = @ListItemId";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@IsDone", true);
                    cmd.Parameters.AddWithValue("@ListItemId", itemId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            public void DeleteItem(int itemId)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM ListItems WHERE ListItemId = @ListItemId";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ListItemId", itemId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            public void UpdatePositions(List<int> idsInOrder)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    for (int i = 0; i < idsInOrder.Count; i++)
                    {
                        string query = "UPDATE ListItems SET ItemPosition = @ItemPosition WHERE ListItemId = @ListItemId";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@ItemPosition", i + 1);
                        cmd.Parameters.AddWithValue("@ListItemId", idsInOrder[i]);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }


    }
