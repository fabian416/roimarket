using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProductPopupController : MonoBehaviour
{
    public DatabaseManager databaseManager;
    public CartPanelController cartPanelController;
    public TextMeshProUGUI productNameText;
    public TextMeshProUGUI productPriceText;
    public TextMeshProUGUI productQuantityText;
    public Button yesButton;
    public Button noButton;
    

    private int productQuantity = 1;
    private Product currentProduct;
    private ShoppingCart shoppingCart;
    private Product[] allProducts;


    public void Initialize(Product product, ShoppingCart cart)
    {
        productQuantity = 1;
        currentProduct = product;
        shoppingCart = cart;

        productNameText.text = product.productName;
        productPriceText.text ="$" + product.price.ToString();
        productQuantityText.text = productQuantity.ToString();

        // Remove previous event handlers
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(ClosePopup);

        yesButton.onClick.AddListener(ConfirmPurchase);
        noButton.onClick.AddListener(ClosePopup);
    }

    public void IncreaseQuantity()
    {
        productQuantity = Mathf.Min(productQuantity + 1, 99); // Increase the amount and limit to 99
        productQuantityText.text = productQuantity.ToString();//Update the text in the panel
    }

    public void DecreaseQuantity() 
    {
        productQuantity = Mathf.Max(productQuantity - 1, 1);// Decrease quantity and limit to 1
        productQuantityText.text = productQuantity.ToString();// Update the text in the panel
    }

    public void ConfirmPurchase()
    {
    if (currentProduct.HasEnoughStock(productQuantity))
    {
        shoppingCart.AddToCart(currentProduct, productQuantity);
        currentProduct.stock -= productQuantity; //Update product stock
         // Update the stock in the database
        databaseManager.UpdateProductStock(currentProduct.productId, currentProduct.stock);

        cartPanelController.AddProductToCart(currentProduct, productQuantity); // Asegúrate de llamar a este método
        ClosePopup();
    }
    else
    {
        Debug.Log("No hay stock suficiente");
    }
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false); // Desactive the object
    }

    
}