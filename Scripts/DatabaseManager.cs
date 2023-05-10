using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.Data;
using System;

public class ProductData
{
    public int productId;
    public string productName;
    public float price;
    public int stock;
    public int categoryIndex;
}


public class DatabaseManager : MonoBehaviour
{
    private string connectionString;
    void Start()
    {
        connectionString = $"SERVER=database-roi.ct1udap7bruq.sa-east-1.rds.amazonaws.com;PORT=3306;DATABASE=RoiMarket;UID=diazfabian;PASSWORD={System.Environment.GetEnvironmentVariable("DB_PASSWORD")};";
        TestConnection();
    }

     public ProductData GetProduct(int productId)
    {
        ProductData productData = null;

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
        connection.Open();

        using (MySqlCommand command = new MySqlCommand("SELECT * FROM products WHERE product_id = @product_id", connection))
        {
            command.Parameters.AddWithValue("@product_id", productId);

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    productData = new ProductData
                    {
                        productId = reader.GetInt32("product_id"), 
                        productName = reader.GetString("product_name"),
                        price = (float)reader.GetDouble("price"),
                        stock = reader.GetInt32("stock"),
                        categoryIndex = reader.GetInt32("category_index") 
                    };
                }
            }
        }
    }

        return productData;
    }

     public void UpdateProductStock(int productId, int newStock)
    {
        MySqlConnection connection = new MySqlConnection(connectionString);

        try
        {
            connection.Open();
            MySqlCommand command = new MySqlCommand($"UPDATE products SET stock = {newStock} WHERE product_id = {productId}", connection);
            command.ExecuteNonQuery();
        }
        catch (MySqlException ex)
        {
            Debug.LogError("Error connecting to the database: " + ex.Message);
        }
        finally
        {
            connection.Close();
        }
    }

    public int CreateOrder(int userId, float total, string status)
{
    MySqlConnection dbConnection = new MySqlConnection(connectionString);
    dbConnection.Open();

    MySqlCommand dbCommand = dbConnection.CreateCommand();
    dbCommand.CommandText = "INSERT INTO orders (user_id, order_date, total, status) VALUES (@user_id, @order_date, @total, @status); SELECT LAST_INSERT_ID();";
    dbCommand.Parameters.AddWithValue("@user_id", userId);
    dbCommand.Parameters.AddWithValue("@order_date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
    dbCommand.Parameters.AddWithValue("@total", total);
    dbCommand.Parameters.AddWithValue("@status", status);

    int orderId = Convert.ToInt32(dbCommand.ExecuteScalar());

    dbConnection.Close();
    return orderId;
}

public void CreateOrderItem(int orderId, int productId, int quantity, float price)
{
    MySqlConnection dbConnection = new MySqlConnection(connectionString);
    dbConnection.Open();

    MySqlCommand dbCommand = dbConnection.CreateCommand();
    dbCommand.CommandText = "INSERT INTO order_items (order_id, product_id, quantity, price) VALUES (@order_id, @product_id, @quantity, @price)";
    dbCommand.Parameters.AddWithValue("@order_id", orderId);
    dbCommand.Parameters.AddWithValue("@product_id", productId);
    dbCommand.Parameters.AddWithValue("@quantity", quantity);
    dbCommand.Parameters.AddWithValue("@price", price);

    dbCommand.ExecuteNonQuery();

    dbConnection.Close();
}


    public void TestConnection()
    {
        MySqlConnection conn =new MySqlConnection(connectionString);

        try
        {
            conn.Open();
            Debug.Log("Connection to database succesfull!!");
            conn.Close();
        }
        catch (MySqlException ex)
        {
            Debug.LogError("Error connecting to database: " + ex.Message);
        }
    }
}
