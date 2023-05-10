using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastInteraction : MonoBehaviour
{
    public Camera mainCamera;
    public float maxInteractionDistance = 5f;
    public LayerMask interactionLayer;
    public ShoppingCart shoppingCart; // Asegúrate de asignar el carrito en el Inspector
    public GameObject panel; // Asegúrate de asignar el objeto Panel en el Inspector

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
                // Verifica si el objeto golpeado es el carrito
                if (hit.collider.gameObject == shoppingCart.gameObject)
                {
                    // Cambia el estado de holdingCart
                    shoppingCart.ToggleHoldingCart();
                }
                // Verifica si el jugador tiene el carrito antes de interactuar con los productos
                else if (shoppingCart.holdingCart)
                {
                    Product product = hit.collider.GetComponent<Product>();
                    if (product != null)
                    {
                        int quantity = 1;// la cantidad que deseas comprar
                        if (product.HasEnoughStock(quantity))
                        {
                            panel.SetActive(true); // Activa el objeto Panel
                            ProductPopupController popupController = panel.GetComponent<ProductPopupController>();
                            popupController.Initialize(product, shoppingCart);
                        }
        
                    }
                }
            }
        }
    }
}
