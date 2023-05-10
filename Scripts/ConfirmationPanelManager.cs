using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmationPanelManager : MonoBehaviour
{
    private ShoppingCart shoppingCart;
    public GameObject confirmationPanel;

    public void ShowConfirmationPanel(ShoppingCart cart)
    {
        shoppingCart = cart;
        confirmationPanel.SetActive(true);
    }

    public void ConfirmPurchase()
    {
        shoppingCart.CompletePurchase();
        confirmationPanel.SetActive(false);
    }

    public void CancelPurchase()
    {
        confirmationPanel.SetActive(false);
    }
}
