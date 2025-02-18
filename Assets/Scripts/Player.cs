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
    public float jumpSpeed;
    public bool isPlayer1 = false;
    public Rigidbody2D Rb2D;
    public Animator animator;
    float input1X;
    float input1Y;
    float input2X;
    float input2Y;
    bool isJumping = false;
    bool isGrounded;
    public SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        Manager = GameObject.FindWithTag("Manager").gameObject;
        Rb2D = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();


    }
    void Update()
    {

        if (isPlayer1 == true)
        {
            input1X = Input.GetAxisRaw("Horizontal_Player1");
            input1Y = Input.GetAxis("Vertical_Player1");

            //stab attack
            if (Input.GetKeyDown(KeyCode.H))
            {
                animator.SetTrigger("hit");
            }
            //hopp
            if (Input.GetKeyDown(KeyCode.Space) && isJumping == false && isGrounded == true)
            {
                isJumping = true;
            }
            //pil uppåt kastar
            //pil neråt crouchar
        }
        else
        {
            input2X = Input.GetAxis("Horizontal_Player2");
            input2Y = Input.GetAxis("Vertical_Player2");

            //stab attack
            if (Input.GetKeyDown(KeyCode.G))
            {
                animator.SetTrigger("hit");
            }
            //hopp
            if (Input.GetKeyDown(KeyCode.LeftShift) && isJumping == false && isGrounded == true)
            {
                isJumping = true;
            }
        }


    }


    // Update is called once per frame
    void FixedUpdate()
    {
    //kollar vem som är P1 för att få rätt kontroller
        if (isPlayer1 == true)
        {
            animator.SetFloat("walking", Mathf.Abs(input1X));
            Rb2D.AddForce(transform.right * movementSpeed * input1X);

            if (input1X < 0)
            {
                spriteRenderer.flipX = true;
                
            }
            if (input1X > 0)
            {
                spriteRenderer.flipX = false;

            }

        }
        //kollar om P1 + hopp
        if (isPlayer1 == true && isJumping == true)
        {
            //JumpDescending, dela spriten upp beroende på fallhöjd
            //Falling - Not Grounded and Y Velocity/jumpSpeed? less than 0

            animator.SetTrigger("jump");
            animator.SetTrigger("descend");

            //upward force Rigidbody
            Rb2D.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse); // you need a reference to the RigidBody2D component
           
            

            isJumping = false;
            isGrounded = false;

        }

        //ger andra spelaren P2-kontrolelr
        if (isPlayer1 == false)
        {
            animator.SetFloat("walking", Mathf.Abs(input2X));
            Rb2D.AddForce(transform.right * movementSpeed * input2X);

            if (input2X > 0)
            {
                spriteRenderer.flipX = true;

            }
            if (input2X < 0)
            {
                spriteRenderer.flipX = false;

            }

        }
        //kollar om P2 + hopp
        if (isPlayer1 == false && isJumping == true)
        {
            //JumpDescending, dela spriten upp beroende på fallhöjd
            animator.SetTrigger("jump");
            //upward force Rigidbody
            Rb2D.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse); // you need a reference to the RigidBody2D component

            isJumping = false;
            isGrounded = false;

        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isPlayer1 == true && other.tag == "Goal_P1")
        {
            Debug.Log("P1 WIN");

        }
        if (isPlayer1 == false && other.tag == "Goal_P2")
        {
            Debug.Log("P2 WIN");

        }
    }
    private void OnCollisionStay2D(Collision2D other)
    {
    
        if (other.transform.tag == "Ground")
        {
            isGrounded = true;
            Debug.Log("Grounded");
        }

    }
    void OnCollisionExit2D(Collision2D other)
    {
        if (other.transform.tag == "Ground")
        {
            isGrounded = false;
            Debug.Log("NOT grounded");
        }
    }


    public void damageTaken(int damageTaken)
    {
      //matas av Weapon-script
        playerHP = playerHP - damageTaken;
        Debug.Log("Player was hit\nHP: " + playerHP);


        if (playerHP <= 0)
        {
            playerHP = 0;
            Manager.GetComponent<Manager>().DeathTracker(this.gameObject);
        }
        
    }

}

