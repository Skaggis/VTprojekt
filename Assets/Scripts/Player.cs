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
   // private bool throwSword = false;

    private float myInputX;
    private float myInputY;
    private float halfPlayerHeight;
    public float jumpSpeed;
    public int playerHP = 1;
    public int movementSpeed;
    
    public Rigidbody2D Rb2D;
    public Rigidbody2D swordRb2D;
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
        cam = GameObject.Find("Main Camera").GetComponent<Target>();
        Rb2D = gameObject.GetComponent<Rigidbody2D>();
        Bc2D = gameObject.GetComponent<BoxCollider2D>();
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        halfPlayerHeight = GetComponent<SpriteRenderer>().bounds.size.y / 2; //3 & 1.6

        sword = this.gameObject.transform.GetChild(0).gameObject;
        sword.SetActive(true);
        swordRb2D = sword.GetComponent<Rigidbody2D>();
        fist = this.gameObject.transform.GetChild(1).gameObject;
        fist.SetActive(false);
        foot = this.gameObject.transform.GetChild(2).gameObject;
        foot.SetActive(false);

        

        //Bc2D.enabled = true;

    }
    private IEnumerator DelayedJump()
    {
        cam.followY = false;

        int frameDelay = 25; // Antal FixedUpdate-cykler att vänta, en cykel är 50 fps
        for (int i = 0; i < frameDelay; i++)
        {
            yield return new WaitForFixedUpdate(); // Väntar en physics frame
        }

        Debug.Log("followY F");
        //kraft uppåt för jump/volt
        Rb2D.velocity = new Vector2(Rb2D.velocity.x, jumpSpeed);
    }
    private IEnumerator ThrowSword()
    {
            int frameDelay = 25; // Antal FixedUpdate-cykler att vänta, en cykel är 50 fps
            for (int i = 0; i < frameDelay; i++)
            {
                yield return new WaitForFixedUpdate(); // Väntar en physics frame
            }
            swordRb2D.velocity = new Vector2(swordRb2D.velocity.x, 0);
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
        if(Rb2D.bodyType != RigidbodyType2D.Static)
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
            //leta enbart efter Ground
            LayerMask groundLayer = LayerMask.GetMask("Ground");
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, Mathf.Infinity, groundLayer);
            Vector2 rayEnd = transform.position + Vector3.down * 20;
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
    private float lastGroundedY = 0f; // Senaste Y-position vid markkontakt
    private float lastXpos = 0f;      // Senaste X-position för att upptäcka backar

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.transform.tag == "Ground")
        {
            isGrounded = true;
            cam.followY = true;

            // Om spelaren rör sig horisontellt och inte bara står stilla
            bool movingForward = Mathf.Abs(transform.position.x - lastXpos) > 0.1f;

            // Om spelaren landar på en högre höjd (t.ex. en backe)
            if (transform.position.y > lastGroundedY + 0.1f && movingForward)
            {
                cam.followY = true; // Låt kameran följa Y i backe
            }
            // Om spelaren landar på en lägre nivå (fall från plattform)
            else if (transform.position.y < lastGroundedY - 0.5f)
            {
                cam.followY = true; // Låt kameran följa neråt
            }
            else
            {
                cam.followY = false; // Annars lås Y (för att undvika hopp-ryckighet)
            }

            lastGroundedY = transform.position.y; // Uppdatera senaste landade Y-position
            lastXpos = transform.position.x; // Uppdatera X för att känna av backar

        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.transform.tag == "Ground")
        {
            isGrounded = false;
            cam.followY = false; // Lås Y när spelaren hoppar
            Debug.Log("NOT grounded");
        }
    }


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
            //borde detta vara anim event?
            StartCoroutine(ThrowSword());
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
    public void Die()
    {
        //körs sista frame:n i Dead anim
        Destroy(gameObject);
        
        Debug.Log("p1 target");

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