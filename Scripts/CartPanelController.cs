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
    public Transform[] productCategoriesContent; // un array para almacenar los objetos "Content" de cada categoria 
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
    // Verifica si el producto ya está en el carrito
    ProductInCart existingProductInCart = productsInCart.Find(p => p.GetProduct() == product);

    if (existingProductInCart != null)
    {
        // Si el producto ya está en el carrito, actualiza la cantidad
        existingProductInCart.UpdateQuantity(existingProductInCart.GetQuantity() + quantity);
    }
    else
    {
        // Si el producto no está en el carrito, crea una nueva instancia y configúrala
        GameObject newProductInCart = Instantiate(productInCartPrefab, productCategoriesContent[product.categoryIndex].transform);
        ProductInCart productInCartComponent = newProductInCart.GetComponent<ProductInCart>();

        productInCartComponent.SetProduct(product, quantity);
        productInCartComponent.removeButton.onClick.AddListener(() => RemoveProductFromCart(productInCartComponent));

        productsInCart.Add(productInCartComponent);
    }

    // Actualizar el costo total
    totalCost += product.price * quantity;
    UpdateTotalCostText();
    }

    public void RemoveProductFromCart(ProductInCart productInCart)
    {
        
        // Incrementar el stock del producto cuando se elimina del carrito
        productInCart.ProductInfo.stock += productInCart.Quantity;

        // Actualizar el costo total
        totalCost -= productInCart.GetTotalCost();
        UpdateTotalCostText();

        // Eliminar el producto de la lista
        productsInCart.Remove(productInCart);

        // Destruir el objeto en la interfaz de usuario
        Destroy(productInCart.gameObject);
    }

    private void UpdateTotalCostText()
    {
        totalCostTotal.text = "$" + totalCost.ToString("0.00");
    }
}