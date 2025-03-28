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

        int frameDelay = 25; // Antal FixedUpdate-cykler att v�nta, en cykel �r 50 fps
        for (int i = 0; i < frameDelay; i++)
        {
            yield return new WaitForFixedUpdate(); // V�ntar en physics frame
        }

        Debug.Log("followY F");
        //kraft upp�t f�r jump/volt
        Rb2D.velocity = new Vector2(Rb2D.velocity.x, jumpSpeed);
    }
    private IEnumerator ThrowSword()
    {
            int frameDelay = 25; // Antal FixedUpdate-cykler att v�nta, en cykel �r 50 fps
            for (int i = 0; i < frameDelay; i++)
            {
                yield return new WaitForFixedUpdate(); // V�ntar en physics frame
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

        //R�RELSE
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
    }

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
            Vector2 rayEnd = transform.position + Vector3.down * 20;
            animator.SetBool("descend", false);
            Debug.DrawRay(transform.position, rayEnd - (Vector2)transform.position, Color.red);

            //f�r varje tr�ff cast:en g�r
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
        //uppdatera Ytranslate-v�rdet f�r att kunna j�mf�ra med f�reg�ende frame
        previousYvalue = currentYvalue;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isPlayer1 == true && other.tag == "Goal_P1")
        {
            //volt ist f�r hopp
            goal = true;
            Debug.Log("P1 goal T");
        }
        if (isPlayer1 == false && other.tag == "Goal_P2")
        {
            //volt ist f�r hopp
            goal = true;
            Debug.Log("P2 goal T");
        }
    }
    private float lastGroundedY = 0f; // Senaste Y-position vid markkontakt
    private float lastXpos = 0f;      // Senaste X-position f�r att uppt�cka backar

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.transform.tag == "Ground")
        {
            isGrounded = true;
            cam.followY = true;

            // Om spelaren r�r sig horisontellt och inte bara st�r stilla
            bool movingForward = Mathf.Abs(transform.position.x - lastXpos) > 0.1f;

            // Om spelaren landar p� en h�gre h�jd (t.ex. en backe)
            if (transform.position.y > lastGroundedY + 0.1f && movingForward)
            {
                cam.followY = true; // L�t kameran f�lja Y i backe
            }
            // Om spelaren landar p� en l�gre niv� (fall fr�n plattform)
            else if (transform.position.y < lastGroundedY - 0.5f)
            {
                cam.followY = true; // L�t kameran f�lja ner�t
            }
            else
            {
                cam.followY = false; // Annars l�s Y (f�r att undvika hopp-ryckighet)
            }

            lastGroundedY = transform.position.y; // Uppdatera senaste landade Y-position
            lastXpos = transform.position.x; // Uppdatera X f�r att k�nna av backar

        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.transform.tag == "Ground")
        {
            isGrounded = false;
            cam.followY = false; // L�s Y n�r spelaren hoppar
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
        //sv�rdkast WASD
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
        //pil upp�t kastar when hit pressed?
        //pil ner�t crouchar, eller plockar upp sv�rd
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
            //dead anim har event f�r Die-func
            animator.SetTrigger("dead");
            //static Rb2D f�r att inte falla genom marken
            Rb2D.bodyType = RigidbodyType2D.Static;
            //disable f�r att kunna kliva �ver kroppen
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
        //k�rs sista frame:n i Dead anim
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
        //delay:a �ven descend med ienumerator/x frames?
        if (GroundInSight == true)
        {

            //Debug.Log("!!!!!!!!!!!!!!!!!descending");
            //triggas fel :'(
            //P1 stannar inAir n�r jumpspeed �r 5 eller mer, inte P2 
           
            //animator.SetBool("descend", true);

        }

    }
    */
}