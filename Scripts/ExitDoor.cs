using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    private bool triggerActivated = false;
    private ShoppingCart shoppingCart;
    public ConfirmationPanelManager confirmationPanelManager;

  private void OnTriggerEnter(Collider other)
{
    if (triggerActivated)
    {
        Debug.LogWarning("activo trigger already activated. Ignoring subsequent activations.");
        return;
    }
    
    Debug.Log("activo entered by: " + other.gameObject.name);

    if (other.CompareTag("Player") && shoppingCart != null && shoppingCart.holdingCart)
    {
        if (confirmationPanelManager == null)
        {
            Debug.Log("ConfirmationPanelManager is not assigned");
        }
        else
        {
            Debug.Log("ConfirmationPanelManager is assigned"); // Agrega este registro de depuración
        }

        Debug.Log("Player with cart entered the trigger");
        confirmationPanelManager.ShowConfirmationPanel(shoppingCart);

        // Mueve esta línea aquí, dentro de este bloque condicional
        triggerActivated = true;
    }
    else if (other.CompareTag("Player"))
    {
        Debug.Log("Player without cart entered the trigger"); // Agrega este registro de depuración
    }
}



    public void SetShoppingCart(ShoppingCart newShoppingCart)
    {
        shoppingCart = newShoppingCart;
        Debug.Log("SetShoppingCart called with ShoppingCart: " + newShoppingCart.gameObject.name); // Agrega este registro de depuración
    }
}
