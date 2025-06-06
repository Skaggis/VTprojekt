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
    public GameObject sword;
    private GameObject hurtBox;
    private GameObject kinRig;
    // private bool throwSword = false;

    private float myInputX;
    private float myInputY;
    public float halfPlayerHeight;
    public int jumpSpeed;
    public int playerHP = 3;
    public int movementSpeed;
    //private int reSpeed;

    public Rigidbody2D Rb2D;
    public Rigidbody2D swordRb2D;
    public BoxCollider2D Bc2D;
    public BoxCollider2D swordBc2D;
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

        //child3/hurbox active n�r man inte har ett sv�rd
        sword = this.gameObject.transform.GetChild(0).GetChild(0).gameObject;
        sword.SetActive(true);
        swordBc2D = sword.GetComponent<BoxCollider2D>();
        fist = this.gameObject.transform.GetChild(1).gameObject;
        fist.SetActive(false);
        foot = this.gameObject.transform.GetChild(2).gameObject;
        foot.SetActive(false);
        hurtBox = this.gameObject.transform.GetChild(3).gameObject;
        hurtBox.SetActive(true);
        kinRig = this.gameObject.transform.GetChild(4).gameObject;
        kinRig.SetActive(true);

        //reSpeed = movementSpeed;
        //Debug.Log("START reSpeed: "+reSpeed);

        foreach (Transform child in transform)
        {
            //equip, fist, foot, hurtbox
            child.gameObject.layer = gameObject.layer;

        }
        //kinRig layer
        //kinRig = gameObject.transform.GetChild(4).gameObject;
        // & sword layer
        if (this.gameObject.layer == 6)
        {
            sword.layer = 6;
            //sword groundColl
            sword.transform.GetChild(0).gameObject.layer = 6;
            Manager.GetComponent<Manager>().swordInst1 = sword;

            //enable kinRig+byt lager
            //kinRig.layer = 10;
            //kinRig.SetActive(true);

        }
        else if (this.gameObject.layer == 7)
        {
            sword.layer = 7;
            sword.transform.GetChild(0).gameObject.layer = 7;
            Manager.GetComponent<Manager>().swordInst2 = sword;

            //enable kinRig+byt lager
            //kinRig.layer = 9;
            //kinRig.SetActive(true);
        }
        
    }
    #region delay jump
    private IEnumerator DelayedJump()
    {

        int frameDelay = 25; // Antal FixedUpdate-cykler att v�nta, en cykel �r 50 fps
        for (int i = 0; i < frameDelay; i++)
        {
            yield return new WaitForFixedUpdate(); // V�ntar en physics frame
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
        if (goal == true)
        {
            Manager.GetComponent<Manager>().StopAllCoroutines();
        }
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

        //R�RELSE
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
        //f�r fart upp�t 20 frames efter SetTrigger
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
    //uppdatera velocityY �r mindre, f�reg�ende frame
    float previousYvalue;

    //Fixed Update = physics update 50hz, fixed timestep = 0.02
    void FixedUpdate()
    {
        //raycast om velocityY �r mindre �n f�reg�ende frame
        float currentYvalue = Rb2D.velocity.y;

        if (currentYvalue > previousYvalue)
        {
            GroundInSight = false;
            animator.SetBool("descend", false);
        }

        //raycasta enbart n�r player r�r sig ner�t
        else if (currentYvalue < previousYvalue)
        {
            //leta enbart efter Ground
            LayerMask groundLayer = LayerMask.GetMask("Ground");
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, Mathf.Infinity, groundLayer);

            //bara f�r at f� se ett r�tt streck
            Vector2 rayEnd = transform.position + Vector3.down * 20;
            Debug.DrawRay(transform.position, rayEnd - (Vector2)transform.position, Color.red);
            animator.SetBool("descend", false);


            // Sortera tr�ffarna efter avst�nd fr�n spelaren
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            //f�r varje tr�ff cast:en g�r
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
                    // Bryt loopen eftersom vi redan har hittat den n�rmaste marktr�ffen
                    break;
                }
            }
        }
        //uppdatera Ytranslate-v�rdet f�r att kunna j�mf�ra med f�reg�ende frame
        previousYvalue = currentYvalue;
    }
    #endregion
    #region goal
    private void OnTriggerEnter2D(Collider2D other)
    {
        /*
        if (other.gameObject.tag == "Player1" || other.gameObject.tag == "Player2")
        {
            //movementSpeed = 0;
            //Rb2D.velocity = new Vector2(0, jumpSpeed);
            Rb2D.velocity = new Vector2(0 * myInputX, Rb2D.velocity.y);
        }
        */
        //vem k�nner av detta? player boxcoll �r ej trigger, hurtbox �r det
        if(other.tag == "death")
        {
            //garantera
            damageTaken(playerHP);
        }

        if (isPlayer1 == true && other.tag == "Goal_P1")
        {
            //volt ist f�r hopp
            goal = true;
            Debug.Log("P1 goal");
        }
        if (isPlayer1 == false && other.tag == "Goal_P2")
        {
            //volt ist f�r hopp
            goal = true;
            Debug.Log("P2 goal");
        }
    }
    #endregion
    #region isGrounded
    //OnCollisionStay2D ?
    //will be called while the object is reacting with a collider, rather than the first time.
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag == "Ground")
        {                                                       
            isGrounded = true;
            //Debug.Log("grounded");
        }
       //f�rhindra att de puttar varandra
        /*
        if (other.gameObject.layer == 9 || other.gameObject.layer == 10)
        {
            //current direction?
            movementSpeed = 0;
        }*/
             
    }
    /*
    private void OnCollisionExit2D(Collision2D other)
    {
        
        if (other.gameObject.layer == 9 || other.gameObject.layer == 10)
        {
            movementSpeed = reSpeed;
            Debug.Log("EXIT2d->reSpeed ska bli tidigare movementspeed: " + reSpeed);
        }
        /*
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
            animator.SetBool("swordReadyHi", true);
            animator.SetBool("swordReadyMid", false);
            animator.SetBool("swordReadyLo", false);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            animator.SetBool("swordReadyHi", false);
            animator.SetBool("swordReadyMid", true);
            animator.SetBool("swordReadyLo", false);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            animator.SetBool("swordReadyHi", false);
            animator.SetBool("swordReadyMid", false);
            animator.SetBool("swordReadyLo", true);
        }
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            animator.SetBool("swordReadyHi", false);
            animator.SetBool("swordReadyMid", false);
            animator.SetBool("swordReadyLo", false);
            animator.SetTrigger("swordHit");
        }
        //sv�rdkast WASD
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
            //funktion setactive p� childobject
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
        //pil upp�t kastar when hit pressed?
        //pil ner�t crouchar, eller plockar upp sv�rd
    }
    #endregion
    #region P2binds
    void P2KeyBinds()
    {
        //sword ready WASD
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            animator.SetBool("swordReadyHi", true);
            animator.SetBool("swordReadyMid", false);
            animator.SetBool("swordReadyLo", false);
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            animator.SetBool("swordReadyHi", false);
            animator.SetBool("swordReadyMid", true);
            animator.SetBool("swordReadyLo", false);
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            animator.SetBool("swordReadyHi", false);
            animator.SetBool("swordReadyMid", false);
            animator.SetBool("swordReadyLo", true);
        }
        //sword hit WASD
        /*
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            animator.SetBool("swordReadyHi", false);
            animator.SetTrigger("swordHi");
            
        }*/
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            animator.SetBool("swordReadyHi", false);
            animator.SetBool("swordReadyMid", false);
            animator.SetBool("swordReadyLo", false);
            animator.SetTrigger("swordHit");
          
        }/*
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            animator.SetBool("swordReadyLo", false);
            animator.SetTrigger("swordLo");
           
        }*/
        //sv�rdkast WASD
        if (Input.GetKeyDown(KeyCode.Y))
        {
            animator.SetBool("swordReadyHi", false);
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
    #endregion
    #region damage
    public void damageTaken(int damageTaken)
    {
        //matas av Weapon-script
        playerHP = playerHP - damageTaken;

        if (playerHP > 0)
        {
            animator.SetTrigger("hurt");
            //Debug.Log("Player was hit\nHP: " + playerHP);
        }

        if (playerHP <= 0)
        {
            //m�ste disable:a sv�rdets box coll vid d�d
            //sword.SetActive(false);
            //flyttade activaChild till kast-anim
            //sword.GetComponent<BoxCollider2D>().enabled = false;

            Bc2D.enabled = false;
            //dead anim har event f�r Die-func
            animator.SetTrigger("dead");
            //static Rb2D f�r att inte falla genom marken
            Rb2D.bodyType = RigidbodyType2D.Static;
            hurtBox.SetActive(false);
            kinRig.SetActive(false);

            //Manager.GetComponent<Manager>().DeathTracker(gameObject);
            StartCoroutine(DelayedDeathAnnouncement());
            //irrelevant
            //Manager.GetComponent<Manager>().SwordTracker(sword);
        }
    }
    private IEnumerator DelayedDeathAnnouncement()
    {
        yield return new WaitForSeconds(1.4f);
        Manager.GetComponent<Manager>().DeathTracker(gameObject);

    }
    #endregion
    public void Die()
    {
       // StartCoroutine(Manager.GetComponent<Manager>().WaitBeforeToggle());

        //Manager.GetComponent<Manager>().DeathTracker(gameObject);
        //k�rs sista frame:n i Dead anim som �r 1:40 sek l�ng
        Destroy(gameObject);
        // Debug.Log("p1 target");

    }
    //anim event hit
    public void activeChild(int activeChild)
    {
        
        if (activeChild == 0)
        {
            //aktivera sv�rdets box coll ist
            //sword.SetActive(true);
            swordBc2D.enabled = true;
        }
        
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
            //aktiverar hurtbox - anv�nds detta?
            hurtBox.SetActive(true);
        }

    }
    //anim event hit
    public void inactivateChild(int inactivateChild)
    {
        
        if (inactivateChild == 0)
        {
            //deaktivera sv�rdets box coll ist
            //sword.SetActive(false);
            swordBc2D.enabled = false;
        }
        
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
    //anim event hit

}