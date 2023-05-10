using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastInteraction : MonoBehaviour
{
    public Camera mainCamera;
    public float maxInteractionDistance = 5f;
    public LayerMask interactionLayer;
    public ShoppingCart shoppingCart; // Make sure you assign the cart in the Inspector
    public GameObject panel; // Make sure you assign the Panel object in the Inspector

    void Start()
    {
        mainCamera = Camera.main;
        panel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxInteractionDistance, interactionLayer))
            {
                // Check if the hit object is the cart
                if (hit.collider.gameObject == shoppingCart.gameObject)
                {
                    // Change the state of holdingCart
                    shoppingCart.ToggleHoldingCart();
                }
                // Check if the player has the cart before interacting with the products
                else if (shoppingCart.holdingCart)
                {
                    Product product = hit.collider.GetComponent<Product>();
                    if (product != null)
                    {
                        int quantity = 1;//the amount you want to buy
                        if (product.HasEnoughStock(quantity))
                        {
                            panel.SetActive(true); // Active the panel object
                            ProductPopupController popupController = panel.GetComponent<ProductPopupController>();
                            popupController.Initialize(product, shoppingCart);
                        }
        
                    }
                }
            }
        }
    }
}
