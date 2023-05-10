using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class CharacterControllerCustom : MonoBehaviour
{
    public ExitDoor exitDoor;
    public float trotSpeed = 2.9f;
    public float runSpeed = 3.8f;
    public float rotationSpeed = 180f;
    private Rigidbody rb;
    public Animator animator;
    
    private ShoppingCart nearbyShoppingCart;

    private FixedJoint fixedJoint;
    private bool holdingCart = false;


    private void OnTriggerEnter(Collider other)
    {
        ShoppingCart shoppingCart = other.GetComponent<ShoppingCart>();
        if (shoppingCart != null)
        {
            nearbyShoppingCart = shoppingCart;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        ShoppingCart shoppingCart = other.GetComponent<ShoppingCart>();
        if (shoppingCart != null && shoppingCart == nearbyShoppingCart)
        {
            nearbyShoppingCart = null;
        }

    }

    private void InteractWithShoppingCart()
    { 
        Debug.Log("Interact with ShoppingCart called");
        // Cart grab and release logic

        if (holdingCart)
        {
            ReleaseCart();
        }
         else
        {
            GrabCart();
        }

        nearbyShoppingCart.AssignToExitDoor(exitDoor);


        nearbyShoppingCart.ToggleHoldingCart();
    
    }

    private void GrabCart()
    {   
        Debug.Log("GrabCart called");
        //Logic for the character to grab the cart
        fixedJoint = gameObject.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = nearbyShoppingCart.GetComponent<Rigidbody>();

        exitDoor.SetShoppingCart(nearbyShoppingCart);


        holdingCart = true;

        if (nearbyShoppingCart.additionalCollider != null)
        {
            nearbyShoppingCart.additionalCollider.enabled = false;
        }    
    
    }

    private void ReleaseCart()
    {
        Debug.Log("ReleaseCart called");
        //Logic for the character to drop the cart
       if (fixedJoint != null)
        {
        Destroy(fixedJoint);
        }

        holdingCart = false;

         //Reactivate cart mesh collider 
        if (nearbyShoppingCart.additionalCollider !=null)
        {
            nearbyShoppingCart.additionalCollider.enabled = true;
        } 
    
    }
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator=GetComponent<Animator>();
    }

    void UpdateCharacterMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movementDirection = transform.TransformDirection(new Vector3 (horizontal, 0.0f, vertical)).normalized;//new Vector3(horizontal, 0.0f, vertical).normalized;

        if (movementDirection != Vector3.zero)
         {
            float targetSpeed = Input.GetKey(KeyCode.LeftControl) ? runSpeed : trotSpeed;
            rb.velocity = new Vector3(movementDirection.x * targetSpeed, rb.velocity.y, movementDirection.z * targetSpeed);
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            rb.rotation = Quaternion.RotateTowards(rb.rotation, toRotation, rotationSpeed * Time.deltaTime);

            animator.SetFloat("Speed", targetSpeed);
        }
        else
        {
            rb.velocity = new Vector3(0.0f, rb.velocity.y, 0.0f);

            animator.SetFloat("Speed", 0);


        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCharacterMovement();

        if (Input.GetKeyDown(KeyCode.E) && nearbyShoppingCart != null)
        {
            InteractWithShoppingCart();
        }
    }   
}