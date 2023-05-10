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

    int userId = 1; // Reemplaza esto con el ID de usuario real si tienes un sistema de autenticación
    string status = "completed"; // Asumiendo que el estado de la orden es "completed" al momento de realizar la compra

    int orderId = databaseManager.CreateOrder(userId, orderTotal, status); // Crea una nueva orden y obtiene el ID de la orden

    foreach (CartItem cartItem in cartItems)
    {
        // Agrega cada artículo del carrito a la tabla order_items
        databaseManager.CreateOrderItem(orderId, cartItem.product.productId, cartItem.quantity, cartItem.product.price);
    }

    // Agrega los registros de depuración aquí
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

        // Agrega registros de depuración aquí
        Debug.Log("Converted Product ID: " + paymentCartItem.product_id + " Quantity: " + paymentCartItem.quantity);
    }

    return paymentCartItems;
}


 private void OnPaymentCompleted(bool success)
{
    if (success)
    {
        // Aquí puedes agregar el código que se ejecutará cuando el pago sea exitoso
        Debug.Log("Pago completado exitosamente");

        // Actualiza el stock de los productos en la base de datos
        foreach (CartItem cartItem in cartItems)
        {
            // Obtiene el stock actual del producto
            ProductData productData = databaseManager.GetProduct(cartItem.product.productId);
            int currentStock = productData.stock;

            // Calcula el nuevo stock
            int newStock = currentStock - cartItem.quantity;

            // Actualiza el stock en la base de datos
            databaseManager.UpdateProductStock(cartItem.product.productId, newStock);
        }
    }
    else
    {
        // Aquí puedes agregar el código que se ejecutará si el pago falla
        Debug.LogError("Error en el pago");
    }
}


    public void AssignToExitDoor(ExitDoor exitDoor)
    {
        exitDoor.SetShoppingCart(this);
    }

    // Método para cambiar el estado de holdingCart
    public void ToggleHoldingCart()
    {
        holdingCart = !holdingCart;
        Debug.Log("Holding cart: " + holdingCart);
    }
}
