using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public GameObject Manager;
    public GameObject Ground;
    public int playerHP = 3;
    public int movementSpeed;
    public float jumpSpeed;
    public bool isPlayer1 = false;
    public Rigidbody2D Rb2D;
    public BoxCollider2D Bc2D;
    public Animator animator;
    float input1X;
    float input1Y;
    float input2X;
    float input2Y;
    bool isJumping = false;
    bool isGrounded;
    public SpriteRenderer spriteRenderer;
    private GameObject fist;
    private GameObject sword;
    public bool GroundInSight;
    private float halfPlayerHeight;

    public Vector2 colliderDimensions;
    // Start is called before the first frame update
    void Start()
    {
        Manager = GameObject.FindWithTag("Manager");
        Ground = GameObject.FindWithTag("Ground");
        Rb2D = gameObject.GetComponent<Rigidbody2D>();
        Bc2D = gameObject.GetComponent<BoxCollider2D>();
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        sword = this.gameObject.transform.GetChild(0).gameObject;
        sword.SetActive(false);
        fist = this.gameObject.transform.GetChild(1).gameObject;
        fist.SetActive(false);

        halfPlayerHeight = GetComponent<BoxCollider2D>().size.y / 2;
        colliderDimensions = new Vector2(GetComponent<BoxCollider2D>().size.x, GetComponent<BoxCollider2D>().size.y);


    }
    void Update()
    {
        //Bc2D.size = colliderDimensions;

        if (isPlayer1 == true)
        {
            input1X = Input.GetAxis("Horizontal_Player1");
            input1Y = Input.GetAxis("Vertical_Player1");
            Debug.Log("input1X: " + input1X);

            //stab attack WASD
            if (Input.GetKeyDown(KeyCode.H))
            {
                animator.SetTrigger("hit");

            }
            //stab attack joystick
            if (Input.GetKeyDown(KeyCode.Joystick1Button2))
            {
                animator.SetTrigger("hit");
                //funktion setactive p� childobject
                //via animationEvent i "hit"

            }
            //hopp WASD
            if (Input.GetKeyDown(KeyCode.Space) && isJumping == false && isGrounded == true)
            {
                isJumping = true;

            }
            //hopp joystick
            if (Input.GetKeyDown(KeyCode.Joystick1Button1) && isJumping == false && isGrounded == true)
            {
                isJumping = true;
            }
            //pil upp�t kastar
            //pil ner�t crouchar
        }

        else
        {
            input2X = Input.GetAxis("Horizontal_Player2");
            input2Y = Input.GetAxis("Vertical_Player2");
            Debug.Log("input2X: " + input2X);

            //stab attack WASD
            if (Input.GetKeyDown(KeyCode.G))
            {
                animator.SetTrigger("hit");
            }
            //stab attack joystick
            if (Input.GetKeyDown(KeyCode.Joystick2Button2))
            {
                animator.SetTrigger("hit");

            }
            //hopp WASD
            if (Input.GetKeyDown(KeyCode.LeftShift) && isJumping == false && isGrounded == true)
            {
                isJumping = true;
            }
            //hopp joystick
            if (Input.GetKeyDown(KeyCode.Joystick2Button1) && isJumping == false && isGrounded == true)
            {
                isJumping = true;
            }
        }


    }


    // Update is called once per frame
    //Fixed Update = physics update
    void FixedUpdate()
    {
        //raycast
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, Mathf.Infinity);
        GroundInSight = false;
        Vector2 rayEnd = transform.position + Vector3.down * 10;
        //"default" falsk, r�tt streck
        Debug.DrawRay(transform.position, rayEnd - (Vector2)transform.position, Color.red);

        //f�r varje tr�ff cast:en g�r
        foreach (RaycastHit2D hit in hits)
        {
            //om tagg = ground, gult streck
            if (hit.collider.CompareTag("Ground"))
            {

                Debug.Log("GroundInSight T");
                Debug.DrawRay(transform.position, hit.point - (Vector2)transform.position, Color.yellow);

                Transform target = hit.transform;
                float distanceToTarget = Vector2.Distance(transform.position, target.position);
                //Debug.Log("dist: " + distanceToTarget); 8.128206
                //Debug.Log("playerheight: " + halfPlayerHeight); 1.6

                if (distanceToTarget < halfPlayerHeight * 10)
                {
                    GroundInSight = true;
                    //animator.SetTrigger("descend");
                }
            }

        }

        //kollar vem som �r P1 f�r att f� r�tt kontroller
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
        //kollar om P1 + hopp
        if (isPlayer1 == true && isJumping == true)
        {
            //upward force Rigidbody
            Rb2D.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);

            animator.SetTrigger("jump");
            //denna anim har event som k�r funktion f�r fall n�r GroundInSight = true

            /*
            JumpDescending - Not Grounded && Y Velocity/ner�t less than 0? krocka ej med crouch
            //jumpdescending n�st sista key
             * kallar p� funktion som r�knar p� hastighet/avst�nd?
             * triggar Descend - animator.SetTrigger("JumpDescending");
             * */

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
                transform.localScale = new Vector3(5, 5, 5);

            }
            if (input2X < 0)
            {
                transform.localScale = new Vector3(-5, 5, 5);

            }

        }
        //kollar om P2 + hopp
        if (isPlayer1 == false && isJumping == true)
        {
            //JumpDescending, dela spriten upp beroende p� fallh�jd
            animator.SetTrigger("jump");
            //upward force Rigidbody
            //Rb2D.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse); // you need a reference to the RigidBody2D component

            Rb2D.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);

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


    public void damageTaken(int damageTaken)
    {
        //matas av Weapon-script
        playerHP = playerHP - damageTaken;
        //Debug.Log("Player was hit\nHP: " + playerHP);


        if (playerHP <= 0)
        {
            animator.SetTrigger("dead");
            //anim event funktion med raderna nedan
            playerHP = 0;
            //Manager.GetComponent<Manager>().DeathTracker(this.gameObject);
        }

    }
    //anim event jump
    public void rayCast(int ray)
    {

        if ((ray == 1) && (GroundInSight = true))
        {
            Debug.Log("!!!!!!!!!!!!!!!!!descending");
            animator.SetTrigger("descend");

        }

    }
    //anim event dead
    public void killInstance(int killInst)
    {
        if (killInst == 1)
        {
            //collidern �ndras inte
            colliderDimensions = new Vector2(GetComponent<BoxCollider2D>().size.y, GetComponent<BoxCollider2D>().size.x);

            Manager.GetComponent<Manager>().DeathTracker(this.gameObject);
        }

    }
    //anim event hit
    public void activeChild(int activeChild)
    {

        if (activeChild == 1)
        {
            fist.SetActive(true);
        }

    }
    //anim event hit
    public void inactivateChild(int inactivateChild)
    {
        if (inactivateChild == 1)
        {
            fist.SetActive(false);
        }
    }
}

