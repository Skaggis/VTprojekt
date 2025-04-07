using System;
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
    public Target cam;
    private GameObject fist;
    private GameObject foot;
    private GameObject sword;
    private GameObject hurtBox;
    // private bool throwSword = false;

    private float myInputX;
    private float myInputY;
    public float halfPlayerHeight;
    public int jumpSpeed;
    public int playerHP = 3;
    public int movementSpeed;
    
    public Rigidbody2D Rb2D;
    public Rigidbody2D swordRb2D;
    public BoxCollider2D Bc2D;
    //public Vector2 colliderDimensions;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    public bool isPlayer1;
    public bool GroundInSight;
    public bool goal = false;
    private bool isJumping = false;
    public bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        Manager = GameObject.FindWithTag("Manager");
        Ground = GameObject.FindWithTag("Ground");
        cam = GameObject.Find("Main Camera").GetComponent<Target>();
        Rb2D = gameObject.GetComponent<Rigidbody2D>();
        Bc2D = gameObject.GetComponent<BoxCollider2D>();
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        halfPlayerHeight = GetComponent<SpriteRenderer>().bounds.size.y / 2; //3 & 1.6

        //child3/hurbox active när man inte har ett svärd
        sword = this.gameObject.transform.GetChild(0).GetChild(0).gameObject;
        sword.SetActive(true);
        fist = this.gameObject.transform.GetChild(1).gameObject;
        fist.SetActive(false);
        foot = this.gameObject.transform.GetChild(2).gameObject;
        foot.SetActive(false);
        hurtBox = this.gameObject.transform.GetChild(3).gameObject;
        hurtBox.SetActive(true);

        foreach (Transform child in transform)
        {
            if (transform.childCount != 0)
            {
                //sword
                child.gameObject.layer = gameObject.layer;
            }
            //equip, fist, foot
            child.gameObject.layer = gameObject.layer;
        }

    }
    #region delay jump
    private IEnumerator DelayedJump()
    {

        int frameDelay = 25; // Antal FixedUpdate-cykler att vänta, en cykel är 50 fps
        for (int i = 0; i < frameDelay; i++)
        {
            yield return new WaitForFixedUpdate(); // Väntar en physics frame
        }

        Rb2D.velocity = new Vector2(Rb2D.velocity.x, jumpSpeed);
    }
    #endregion
 
    // Update is called once per frame
    void Update()
    {
        /*
        if(this.gameObject.transform.GetChild(0).GetChild(0).transform == null);
        {
            Debug.Log(this.gameObject.transform.GetChild(0).GetChild(0));
            //hurtBox.SetActive(true);
        }
        */
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
        #region movement
        //RÖRELSE
        if (Rb2D.bodyType != RigidbodyType2D.Static)
        {
            Rb2D.velocity = new Vector2(movementSpeed * myInputX, Rb2D.velocity.y);
        }
       
        animator.SetFloat("walking", Mathf.Abs(Rb2D.velocity.x));

        if (myInputX < 0)
        {
            transform.localScale = new Vector3(-5, 5, 5);

        }
        if (myInputX > 0)
        {
            transform.localScale = new Vector3(5, 5, 5);

        }
        #endregion
        #region jump/volt
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
        #endregion
    }
    #region raycast
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
            //leta enbart efter Ground
            LayerMask groundLayer = LayerMask.GetMask("Ground");
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, Mathf.Infinity, groundLayer);

            //bara för at få se ett rött streck
            Vector2 rayEnd = transform.position + Vector3.down * 20;
            Debug.DrawRay(transform.position, rayEnd - (Vector2)transform.position, Color.red);
            animator.SetBool("descend", false);


            // Sortera träffarna efter avstånd från spelaren
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            //för varje träff cast:en gör
            foreach (RaycastHit2D hit in hits)
            {
                //om tagg = ground, gult streck
                if (hit.collider.CompareTag("Ground"))
                {
                    float distanceToTarget = hit.distance;
                    //Transform target = hit.transform;
                    //float distanceToTarget = Vector2.Distance(transform.position, hit.point);
                    Debug.DrawRay(transform.position, hit.point - (Vector2)transform.position, Color.yellow);

                    //halfsize 1.6
                    if (distanceToTarget <= halfPlayerHeight+.5)
                    {
                        GroundInSight = true;
                        animator.SetBool("descend", true);
                        //Debug.Log("dist" + distanceToTarget + "1 / 2 playerheight: " + halfPlayerHeight);
                        
                        Debug.DrawRay(transform.position, hit.point - (Vector2)transform.position, Color.green);
                    }
                    else
                    {
                        GroundInSight = false;
                        animator.SetBool("descend", false);
                        //Debug.Log("else\ndist" + distanceToTarget + "1 / 2 playerheight: " + halfPlayerHeight);
                    }
                    // Bryt loopen eftersom vi redan har hittat den närmaste markträffen
                    break;
                }
            }
        }
        //uppdatera Ytranslate-värdet för att kunna jämföra med föregående frame
        previousYvalue = currentYvalue;
    }
    #endregion
    #region goal
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isPlayer1 == true && other.tag == "Goal_P1")
        {
            //volt ist för hopp
            goal = true;
            //Debug.Log("P1 goal T");
        }
        if (isPlayer1 == false && other.tag == "Goal_P2")
        {
            //volt ist för hopp
            goal = true;
            //Debug.Log("P2 goal T");
        }
    }
    #endregion
    #region isGrounded
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag == "Ground")
        {                                                       
            isGrounded = true;
            //Debug.Log("grounded");
        }
    }
    /*
    void OnCollisionExit2D(Collision2D other)
    {
        if (other.transform.tag == "Ground")
        {
            //isGrounded = false;
            Debug.Log("NOT grounded");
        }
    }
    */
    #endregion
    #region P1binds
    void P1KeyBinds()
    {
        //sword hi WASD
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            animator.SetTrigger("swordHi");
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            animator.SetTrigger("swordMid");
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            animator.SetTrigger("swordLo");
        }
        //svärdkast WASD
        if (Input.GetKeyDown(KeyCode.T))
        {
            animator.SetTrigger("throw");
            sword.GetComponent<Weapon>().ThrowSwordNow();
           
        }
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
            isGrounded = false;

        }
        //hopp joystick
        if (Input.GetKeyDown(KeyCode.Joystick1Button1) && isJumping == false && isGrounded == true)
        {
            isJumping = true;
        }
        //pil uppåt kastar when hit pressed?
        //pil neråt crouchar, eller plockar upp svärd
    }
    #endregion
    #region P2binds
    void P2KeyBinds()
    {
        //sword hi WASD
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            animator.SetTrigger("swordHi");
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            animator.SetTrigger("swordMid");
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            animator.SetTrigger("swordLo");
        }
        //svärdkast WASD
        if (Input.GetKeyDown(KeyCode.Y))
        {
            animator.SetTrigger("throw");
            sword.GetComponent<Weapon>().ThrowSwordNow();

        }

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
    #endregion //P2binds
    #region damage
    public void damageTaken(int damageTaken)
    {
        //matas av Weapon-script
        playerHP = playerHP - damageTaken;

        if (playerHP <= 0)
        {
            //dead anim har event för Die-func
            animator.SetTrigger("dead");
            //static Rb2D för att inte falla genom marken
            Rb2D.bodyType = RigidbodyType2D.Static;
            //disable för att kunna kliva över kroppen
            Bc2D.enabled = false;

            Manager.GetComponent<Manager>().DeathTracker(gameObject);

        }
        if (playerHP > 0)
        {
            animator.SetTrigger("hurt");
            //Debug.Log("Player was hit\nHP: " + playerHP);
        }

    }
    #endregion
    public void Die()
    {
        //körs sista frame:n i Dead anim
        Destroy(gameObject);
        
       // Debug.Log("p1 target");

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
        if (activeChild == 3)
        {
            //aktiverar hurtbox
            hurtBox.SetActive(true);
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
        if (inactivateChild == 3)
        {
            //dektiverar hurtbox
            hurtBox.SetActive(false);
        }
    }

}