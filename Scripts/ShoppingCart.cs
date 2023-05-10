using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ShoppingCart : MonoBehaviour
{
    public PaymentManager paymentManager;
    public DatabaseManager databaseManager; 
    public Collider additionalCollider;
    public bool holdingCart; 

    public List<CartItem> cartItems = new List<CartItem>();

    private void Start()
    {
    }

    private void Update()
    {
    }

    public void AddToCart(Product product, int quantity)
    {
        CartItem existingCartItem = cartItems.Find(item => item.product == product);

        if (existingCartItem != null)
        {
            existingCartItem.quantity += quantity;
        }
        else
        {
            CartItem newCartItem = new CartItem { product = product, quantity = quantity };
            cartItems.Add(newCartItem);
        }
    }

   public void CompletePurchase()
{
    float orderTotal = 0f;

    foreach (CartItem cartItem in cartItems)
    {
        orderTotal += cartItem.product.price * cartItem.quantity;
    }

    int userId = 1; // replace this with the id of the real user when you have the authentication system
    string status = "completed"; //Assuming that the order status is "completed" at the time of purchase

    int orderId = databaseManager.CreateOrder(userId, orderTotal, status); //Create a new order and get the order ID

    foreach (CartItem cartItem in cartItems)
    {
        // Add each cart item to the order_items table
        databaseManager.CreateOrderItem(orderId, cartItem.product.productId, cartItem.quantity, cartItem.product.price);
    }

    Debug.Log("Cart Items Count: " + cartItems.Count);
    foreach (CartItem item in cartItems)
    {
        Debug.Log("Product ID: " + item.product.productId + " Quantity: " + item.quantity);
    }

    List<CartItem> convertedCartItems = ConvertToPaymentCartItems(cartItems);

    string userEmail = ManageUser.Instance.CurrentUser.email;
    StartCoroutine(paymentManager.MakePayment(userEmail, convertedCartItems, OnPaymentCompleted));
}

private List<CartItem> ConvertToPaymentCartItems(List<CartItem> shoppingCartItems)
{
    List<CartItem> paymentCartItems = new List<CartItem>();

    foreach (CartItem shoppingCartItem in shoppingCartItems)
    {
        CartItem paymentCartItem = new CartItem
        {
            product = shoppingCartItem.product,
            product_id = shoppingCartItem.product.productId,
            quantity = shoppingCartItem.quantity
        };

        paymentCartItems.Add(paymentCartItem);

        Debug.Log("Converted Product ID: " + paymentCartItem.product_id + " Quantity: " + paymentCartItem.quantity);
    }

    return paymentCartItems;
}


 private void OnPaymentCompleted(bool success)

{
    if (success)

    {
        // The code that will be executed when the payment is successful
        Debug.Log("Pago completado exitosamente");

        // Update the stock of products in the database
        foreach (CartItem cartItem in cartItems)
        {
            // Gets the current stock of the product
            ProductData productData = databaseManager.GetProduct(cartItem.product.productId);
            int currentStock = productData.stock;

            // Calculate the new stock
            int newStock = currentStock - cartItem.quantity;

            // Update the stock in the database
            databaseManager.UpdateProductStock(cartItem.product.productId, newStock);
        }
    }
    else
    {
        // Here the code that will be executed if the payment fails
        Debug.LogError("Error en el pago");
    }
}


    public void AssignToExitDoor(ExitDoor exitDoor)
    {
        exitDoor.SetShoppingCart(this);
    }

    // Method to change the state of holdingCart
    public void ToggleHoldingCart()
    {
        holdingCart = !holdingCart;
        Debug.Log("Holding cart: " + holdingCart);
    }
}
