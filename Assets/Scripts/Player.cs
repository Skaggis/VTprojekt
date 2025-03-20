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
    private GameObject fist;
    private GameObject foot;
    private GameObject sword;

    private float myInputX;
    private float myInputY;
    private float halfPlayerHeight;
    public float jumpSpeed;
    public int playerHP = 3;
    public int movementSpeed;
    
    public Rigidbody2D Rb2D;
    public BoxCollider2D Bc2D;
    public Vector2 colliderDimensions;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    public bool isPlayer1;
    public bool GroundInSight;
    public bool goal = false;
    private bool isJumping = false;
    private bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        Manager = GameObject.FindWithTag("Manager");
        Ground = GameObject.FindWithTag("Ground");
        Rb2D = gameObject.GetComponent<Rigidbody2D>();
        Bc2D = gameObject.GetComponent<BoxCollider2D>();
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        halfPlayerHeight = GetComponent<SpriteRenderer>().bounds.size.y / 2; //3 & 1.6

        sword = this.gameObject.transform.GetChild(0).gameObject;
        sword.SetActive(false);
        fist = this.gameObject.transform.GetChild(1).gameObject;
        fist.SetActive(false);
        foot = this.gameObject.transform.GetChild(2).gameObject;
        foot.SetActive(false);

        

    }
    private IEnumerator DelayedJump()
    {
        int frameDelay = 25; // Antal FixedUpdate-cykler att vänta, en cykel är 50 fps
        for (int i = 0; i < frameDelay; i++)
        {
            yield return new WaitForFixedUpdate(); // Väntar en physics frame
        }

        //kraft uppåt för jump/volt
        Rb2D.velocity = new Vector2(Rb2D.velocity.x, jumpSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        //INPUTS
        if (isPlayer1 == true)
        {
            myInputX = Input.GetAxis("Horizontal_Player1");
            myInputY = Input.GetAxis("Vertical_Player1");
            P1KeyBinds();
        }
        else if (isPlayer1 == false)
        {
            myInputX = Input.GetAxis("Horizontal_Player2");
            myInputY = Input.GetAxis("Vertical_Player2");
            P2KeyBinds();
        }

        //RÖRELSE
        Rb2D.velocity = new Vector2(movementSpeed * myInputX, Rb2D.velocity.y);

        animator.SetFloat("walking", Mathf.Abs(Rb2D.velocity.x));

        if (myInputX < 0)
        {
            transform.localScale = new Vector3(-5, 5, 5);

        }
        if (myInputX > 0)
        {
            transform.localScale = new Vector3(5, 5, 5);

        }
        //HOPP
        //får fart uppåt 20 frames efter SetTrigger
        if (isJumping == true && goal == false)
        {
            animator.SetTrigger("jump");
            StartCoroutine(DelayedJump());
            isJumping = false;
            isGrounded = false;

        }
        //VOLT
        else if (isJumping == true && goal == true)
        {
            animator.SetTrigger("volt");
            StartCoroutine(DelayedJump());
            isJumping = false;
            isGrounded = false;
            goal = false;

        }
    }

    //uppdatera velocityY är mindre, föregående frame
    float previousYvalue;

    //Fixed Update = physics update 50hz, fixed timestep = 0.02
    void FixedUpdate()
    {
        //raycast om velocityY är mindre än föregående frame
        float currentYvalue = Rb2D.velocity.y;

        if (currentYvalue > previousYvalue)
        {
            GroundInSight = false;
            animator.SetBool("descend", false);
        }

        //raycasta enbart när player rör sig neråt
        else if (currentYvalue < previousYvalue)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, Mathf.Infinity);
            Vector2 rayEnd = transform.position + Vector3.down * 10;
            animator.SetBool("descend", false);
            Debug.DrawRay(transform.position, rayEnd - (Vector2)transform.position, Color.red);

            //för varje träff cast:en gör
            foreach (RaycastHit2D hit in hits)
            {
                //om tagg = ground, gult streck
                if (hit.collider.CompareTag("Ground"))
                {
                    Transform target = hit.transform;
                    float distanceToTarget = Vector2.Distance(transform.position, hit.point);
                    Debug.DrawRay(transform.position, hit.point - (Vector2)transform.position, Color.yellow);
                    //halfsize 1.6
                    if (distanceToTarget <= halfPlayerHeight+.5)
                    {
                      
                        GroundInSight = true;
                        animator.SetBool("descend", true);
                        //tidigare när GroundInSight = true fick funktion köras, kallades i anim event i inAir
                        //Debug.Log("dist" + distanceToTarget + "1 / 2 playerheight: " + halfPlayerHeight);
                        
                        Debug.DrawRay(transform.position, hit.point - (Vector2)transform.position, Color.green);
                    }
                    
                    else
                    {
                        GroundInSight = false;
                        animator.SetBool("descend", false);
                        //Debug.Log("else\ndist" + distanceToTarget + "1 / 2 playerheight: " + halfPlayerHeight);
                    }
                }
            }
        }
        //uppdatera Ytranslate-värdet för att kunna jämföra med föregående frame
        previousYvalue = currentYvalue;

        
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isPlayer1 == true && other.tag == "Goal_P1")
        {
            //volt ist för hopp
            goal = true;
            Debug.Log("P1 goal T");
            
        }
        if (isPlayer1 == false && other.tag == "Goal_P2")
        {
            //volt ist för hopp
            goal = true;
            Debug.Log("P2 goal T");

        }
    }
    private void OnCollisionStay2D(Collision2D other)
    {
    
        if (other.transform.tag == "Ground")
        {
            isGrounded = true;
            //Debug.Log("Grounded");
        }

    }
    void OnCollisionExit2D(Collision2D other)
    {
        if (other.transform.tag == "Ground")
        {
            isGrounded = false;
            //Debug.Log("NOT grounded");
        }
    }

    void P1KeyBinds()
    {
        
        //fist attack WASD
        if (Input.GetKeyDown(KeyCode.H))
        {
            animator.SetTrigger("hit");

        }
        //kick attack WASD
        if (Input.GetKeyDown(KeyCode.J))
        {
            animator.SetTrigger("kick");

        }
        //fist attack joystick
        if (Input.GetKeyDown(KeyCode.Joystick1Button2))
        {
            animator.SetTrigger("hit");
            //funktion setactive på childobject
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
        //pil uppåt kastar when hit pressed?
        //pil neråt crouchar, eller plockar upp svärd
    }

    void P2KeyBinds()
    {
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            animator.SetTrigger("hit");
        }
        //kick attack WASD
        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.SetTrigger("kick");

        }
        //fist attack joystick
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



    //anim event dead
    public void killInstance(int killInst)
    {
        if (killInst == 1)
        {
            //collidern ändras inte
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
        if (activeChild == 2)
        {
            foot.SetActive(true);
        }

    }
    //anim event hit
    public void inactivateChild(int inactivateChild)
    {
        if (inactivateChild == 1)
        {
            fist.SetActive(false);
        }
        if (inactivateChild == 2)
        {
            foot.SetActive(false);
        }
    }

    //anim event i JumpInAir
    /*
    public void descend()
    {
        //delay:a även descend med ienumerator/x frames?
        if (GroundInSight == true)
        {

            //Debug.Log("!!!!!!!!!!!!!!!!!descending");
            //triggas fel :'(
            //P1 stannar inAir när jumpspeed är 5 eller mer, inte P2 
           
            //animator.SetBool("descend", true);

        }

    }
    */
}