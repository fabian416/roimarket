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

        //eliminar los controladores de eventos anteriores
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(ClosePopup);

        yesButton.onClick.AddListener(ConfirmPurchase);
        noButton.onClick.AddListener(ClosePopup);
    }

    public void IncreaseQuantity()
    {
        productQuantity = Mathf.Min(productQuantity + 1, 99); // Aumenta la cantidad y limita a 99
        productQuantityText.text = productQuantity.ToString();//Actualiza el texto en el panel
    }

    public void DecreaseQuantity() 
    {
        productQuantity = Mathf.Max(productQuantity - 1, 1);// Disminuye la cantidad y limita a 1 
        productQuantityText.text = productQuantity.ToString();// Actualiza el texto en el panel
    }

    public void ConfirmPurchase()
    {
    if (currentProduct.HasEnoughStock(productQuantity))
    {
        shoppingCart.AddToCart(currentProduct, productQuantity);
        currentProduct.stock -= productQuantity; //Actualiza el stock del producto
         // Actualiza el stock en la base de datos
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
        gameObject.SetActive(false); // Desactiva el objeto 
    }

    
}