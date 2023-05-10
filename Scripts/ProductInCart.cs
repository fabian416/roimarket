using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProductInCart : MonoBehaviour
{
    public TextMeshProUGUI productNameText;
    public TextMeshProUGUI productQuantityText;
    public TextMeshProUGUI productCostText;
    public Button removeButton;

    private Product product;
    private int quantity;

    public Product GetProduct()
    {
    return product;
    }

    public int GetQuantity()
    {
    return quantity;
    }

    public void UpdateQuantity(int newQuantity)
    {
    quantity = newQuantity;
    productQuantityText.text = quantity.ToString();
    productCostText.text = "$" + (product.price * quantity).ToString("0.00");
    }


    public void SetProduct(Product newProduct, int newQuantity)
    {
        Debug.Log("Configurando producto en el carrito: " + newProduct.name); // Añade esta línea
        product = newProduct;
        quantity = newQuantity;

        productNameText.text = product.productName;
        productQuantityText.text = quantity.ToString();
        productCostText.text = "$" + (product.price * quantity).ToString("0.00");
    }

    public float GetTotalCost()
    {
        return product.price * quantity;
    }
    
    public Product ProductInfo
    {
    get { return product; }
    }

    public int Quantity
    {
    get { return quantity; }
    }

}
