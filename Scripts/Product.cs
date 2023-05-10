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
        // Puedes usar la función GetProduct() en DatabaseManager para cargar la información del producto desde la base de datos usando el productId
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
        // Puedes usar la función UpdateProductStock() en DatabaseManager para actualizar el stock del producto en la base de datos
        databaseManager.UpdateProductStock(productId, stock);
    }
}
