using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CartPanelController : MonoBehaviour
{
    public DatabaseManager databaseManager;
    public List<ProductInCart> productsInCart = new List<ProductInCart>();
    public GameObject cartPanel;
    public GameObject productInCartPrefab;
    public Transform[] productCategoriesContent; // an array to store the "Content" objects of each category
    public TextMeshProUGUI totalCostTotal;

    private float totalCost = 0f;


    public void CloseCartPanel()
    {
        gameObject.SetActive(false);

    }

    public void ToggleCartPanel()
    {
        gameObject.SetActive(!gameObject.activeSelf);

    }

    public void AddProductToCart(Product product, int quantity)
    {
    // Check if the product is already in the cart
    ProductInCart existingProductInCart = productsInCart.Find(p => p.GetProduct() == product);

    if (existingProductInCart != null)
    {
        // If the product is already in the cart, update the quantity
        existingProductInCart.UpdateQuantity(existingProductInCart.GetQuantity() + quantity);
    }
    else
    {
        // If the product is not in the cart, create a new instance and configure it
        GameObject newProductInCart = Instantiate(productInCartPrefab, productCategoriesContent[product.categoryIndex].transform);
        ProductInCart productInCartComponent = newProductInCart.GetComponent<ProductInCart>();

        productInCartComponent.SetProduct(product, quantity);
        productInCartComponent.removeButton.onClick.AddListener(() => RemoveProductFromCart(productInCartComponent));

        productsInCart.Add(productInCartComponent);
    }

    // Update total cost
    totalCost += product.price * quantity;
    UpdateTotalCostText();
    }

    public void RemoveProductFromCart(ProductInCart productInCart)
    {
        
        //Increase the stock of the product when it is removed from the cart
        productInCart.ProductInfo.stock += productInCart.Quantity;

        // Update total cost
        totalCost -= productInCart.GetTotalCost();
        UpdateTotalCostText();

        // Remove the product from the list
        productsInCart.Remove(productInCart);

        // Destroy the object in the UI
        Destroy(productInCart.gameObject);
    }

    private void UpdateTotalCostText()
    {
        totalCostTotal.text = "$" + totalCost.ToString("0.00");
    }
}