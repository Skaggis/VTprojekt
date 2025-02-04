using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public GameObject Manager;
    public int playerHP = 3;
    public int movementSpeed;
    public int jumpSpeed;
    public bool isPlayer1 = false;
    public Rigidbody2D Rb2D;
    public Animator animator;
    float input1X;
    float input1Y;
    float input2X;
    float input2Y;
    bool isJumping = false;
    //bool isStabbing = false;
    bool isGrounded = true;

    // Start is called before the first frame update
    void Start()
    {
        Manager = GameObject.FindWithTag("Manager").gameObject;
        Rb2D = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();

    }
    void Update()
    {

        if (isPlayer1 == true)
        {
            input1X = Input.GetAxis("Horizontal_Player1");
            input1Y = Input.GetAxis("Vertical_Player1");

            if (Input.GetKeyDown(KeyCode.H))
            {
                animator.SetTrigger("stab");
            }
            if (Input.GetKeyDown(KeyCode.Space) && isJumping == false)
            {
                isJumping = true;
                isGrounded = false;


            }
        }
        else
        {
            input2X = Input.GetAxis("Horizontal_Player2");
            input2Y = Input.GetAxis("Vertical_Player2");

            if (Input.GetKeyDown(KeyCode.G))
            {
                animator.SetTrigger("stab");
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) && isJumping == false)
            {
                isJumping = true;
                isGrounded = false;
            }
        }


    }


    // Update is called once per frame
    void FixedUpdate()
    {
    //kollar vem som är P1 för att få rätt kontroller
        if (isPlayer1 == true)
        {
            Rb2D.AddForce(transform.right * movementSpeed * input1X);


        }
        //kollar om P1 + hopp
        if (isPlayer1 == true && isJumping == true)
        {
            // Apply an upward force to the Rigidbody
            Rb2D.AddForce(Vector3.up * jumpSpeed, ForceMode2D.Impulse);

            /*
            Rb2D.AddForce(Vector2.up * jumpHeight); // you need a reference to the RigidBody2D component
            isJumping = true;
            */
        }
        //ger andra spelaren P2-kontrolelr
        if (isPlayer1 == false)
        {
            Rb2D.AddForce(transform.right * movementSpeed * input2X);
            //Rb2D.AddForce(transform.up * movementSpeed * input2Y);

        }
        //kollar om P2 + hopp
        if (isPlayer1 == false && isJumping == true)
        {
            // Apply an upward force to the Rigidbody
            Rb2D.AddForce(Vector3.up * jumpSpeed, ForceMode2D.Impulse);

        }

    }
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.transform.tag == "Ground")
        {
            isGrounded = true;
            Debug.Log("Grounded");
        }
        else
        {
            isGrounded = false;
            Debug.Log("Not Grounded!");
        }
    }


    public void damageTaken(int damageTaken)
    {
        //ändrar prefab objektet, inte instancen
        playerHP = playerHP - damageTaken;
        Debug.Log("Player was hit\nHP: " + playerHP);


        if (playerHP <= 0)
        {
            playerHP = 0;
            Manager.GetComponent<Manager>().DeathTracker(this.gameObject);
        }
        
    }

}

