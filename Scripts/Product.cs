using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Product : MonoBehaviour
{
    public int productId;
    public string productName;
    public float price;
    public int stock;
    public int categoryIndex;

    private DatabaseManager databaseManager;

    private void Start()
    {
        databaseManager = FindObjectOfType<DatabaseManager>();
        InitializeProductFromDatabase();
    }

    public bool HasEnoughStock(int requestedQuantity)
    {
        return stock >= requestedQuantity;
    }

    public void UpdateStock(int newStock)
    {
        stock = newStock;
        UpdateStockInDatabase();
    }

    private void InitializeProductFromDatabase()
    {
        // I can use the GetProduct() function in the DatabaseManager to load the product information from the database using the productId
        ProductData productData = databaseManager.GetProduct(productId);

        if (productData != null)
        {
            productName = productData.productName;
            price = productData.price;
            stock = productData.stock;
            categoryIndex = productData.categoryIndex;
        }
        else
        {
            Debug.LogError($"Product with ID {productId} not found in the database");
        }
    }

    private void UpdateStockInDatabase()
    {
        // I can use the UpdateProductStock() function in the DatabaseManager to update the product stock in the database
        databaseManager.UpdateProductStock(productId, stock);
    }
}
