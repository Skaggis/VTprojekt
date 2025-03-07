using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class jumping : MonoBehaviour
{
    public GameObject Manager;
    public GameObject Ground;

    public bool isPlayer1 = false;
    float input1X;
    float input1Y;
    public int movementSpeed;
    public float jumpSpeed;
    
    public Rigidbody2D Rb2D;
    public BoxCollider2D Bc2D;
    public Vector2 colliderDimensions;
    float halfPlayerHeight;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    bool isJumping = false;
    bool isGrounded;
    public bool GroundInSight;
    
    // Start is called before the first frame update
    void Start()
    {
        Manager = GameObject.FindWithTag("Manager");
        Ground = GameObject.FindWithTag("Ground");

        Rb2D = gameObject.GetComponent<Rigidbody2D>();
        Bc2D = gameObject.GetComponent<BoxCollider2D>();
        //colliderDimensions = new Vector2(GetComponent<BoxCollider2D>().size.x, GetComponent<BoxCollider2D>().size.y);

        //halfPlayerHeight = GetComponent<BoxCollider2D>().size.y / 2; //.2
        //halfPlayerHeight = transform.localScale.y / 2; // 2.5
        halfPlayerHeight = GetComponent<SpriteRenderer>().bounds.size.y / 2; //3 & 1.6
        //halfPlayerHeight = GetComponent<SpriteRenderer>().bounds.extents.y; //3.2 & 1.6
        //Debug.Log(halfPlayerHeight);

        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }
    // Update is called once per frame
    void Update()
    {
        //Bc2D.size = colliderDimensions;

        if (isPlayer1 == true)
        {
            input1X = Input.GetAxis("Horizontal_Player1");
            input1Y = Input.GetAxis("Vertical_Player1");

            //hopp joystick
            if (Input.GetKeyDown(KeyCode.Joystick1Button1) && isJumping == false && isGrounded == true)
            {
                isJumping = true;
            }

        }

       

    }


    //Fixed Update = physics update
    void FixedUpdate()
    {
        //raycast
        GroundInSight = false;
        Debug.Log("FALSE Sight");
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, Mathf.Infinity);
        //Vector2 rayStart = transform.position + Vector3.down * halfPlayerHeight;
        Vector2 rayEnd = transform.position + Vector3.down * 10;

        Debug.DrawRay(transform.position, rayEnd - (Vector2)transform.position, Color.red);

        //för varje träff cast:en gör
        foreach (RaycastHit2D hit in hits)
        {
            //om tagg = ground, gult streck
            if (hit.collider.CompareTag("Ground"))
            {
                
                Debug.DrawRay(transform.position, hit.point - (Vector2)transform.position, Color.yellow);

                Transform target = hit.transform;
                float distanceToTarget = Vector2.Distance(transform.position, hit.point);
               // Debug.Log("dist" + distanceToTarget + "1 / 2 playerheight: " + halfPlayerHeight);

                //stopp vid: dist1.66811 / 2 playerheight: 1.6

                if (distanceToTarget <= halfPlayerHeight)
                {
                    GroundInSight = true;
                 
                    Debug.Log("dist" + distanceToTarget + "1 / 2 playerheight: " + halfPlayerHeight);
                    animator.SetTrigger("descend");
                }
   
            }

        }

        //kollar om P1 + hopp
        if (isPlayer1 == true && isJumping == true)
        {
            //upward force Rigidbody
            Rb2D.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);

            animator.SetTrigger("jump");
            //denna anim har event som kör funktion för fall när GroundInSight = true
            //som ligger i raycasten

            isJumping = false;
            isGrounded = false;


        }
        //kollar vem som är P1 för att få rätt kontroller
        if (isPlayer1 == true)
        {
            animator.SetFloat("walking", Mathf.Abs(input1X));
            Rb2D.AddForce(transform.right * movementSpeed * input1X);

            if (input1X < 0)
            {

                //spriteRenderer.flipX = true;
                transform.localScale = new Vector3(-5, 5, 5);

            }
            if (input1X > 0)
            {
                //spriteRenderer.flipX = false;
                transform.localScale = new Vector3(5, 5, 5);

            }

        }
        }
    

    private void OnCollisionStay2D(Collision2D other)
    {

        if (other.transform.tag == "Ground")
        {
            isGrounded = true;
            //animator.SetTrigger("descend");
            //raycast ist?
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

    //anim event jump
    //körs en gång, GroundInSight hinner ej bli T
    /*
    public void rayCast(int ray)
    {
      
            if (GroundInSight == true)
            {
                Debug.Log("descending!!!!!!!!!!!!!!!!!");
                animator.SetTrigger("descend");

            }
        


    }
    */
}

