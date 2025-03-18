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
    private GameObject foot;
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
        foot = this.gameObject.transform.GetChild(2).gameObject;
        foot.SetActive(false);

        halfPlayerHeight = GetComponent<SpriteRenderer>().bounds.size.y / 2; //3 & 1.6
        //colliderDimensions = new Vector2(GetComponent<BoxCollider2D>().size.x, GetComponent<BoxCollider2D>().size.y);


    }
    private IEnumerator DelayedJump()
    {
        int frameDelay = 25; // Antal FixedUpdate-cykler att v�nta, en cykel �r 50 fps
        for (int i = 0; i < frameDelay; i++)
        {
            yield return new WaitForFixedUpdate(); // V�ntar en physics frame
        }

        Rb2D.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
    }
    void Update()
    {

        if (isPlayer1 == true)
        {
            input1X = Input.GetAxis("Horizontal_Player1");
            input1Y = Input.GetAxis("Vertical_Player1");
            Debug.Log("input1X: "+input1X);

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
       
        else
        {
            input2X = Input.GetAxis("Horizontal_Player2");
            input2Y = Input.GetAxis("Vertical_Player2");
            Debug.Log("input2X: " + input2X);

            //fist attack WASD
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
    }
    //uppdatera velocityY �r mindre, f�reg�ende frame
    float previousYvalue;

    // Update is called once per frame
    //Fixed Update = physics update
    void FixedUpdate()
    {
        //raycast om velocityY �r mindre �n f�reg�ende frame
        float currentYvalue = Rb2D.velocity.y;

        if (currentYvalue > previousYvalue)
        {
            GroundInSight = false;
            animator.SetBool("descend", false);
            Debug.Log("R�r sig upp�t");
        }

        //st�mmer p�v�g upp en bit?
        else if (currentYvalue < previousYvalue)
        {
            Debug.Log("R�r sig ner�t");
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, Mathf.Infinity);
            //Vector2 rayStart = transform.position + Vector3.down * halfPlayerHeight;
            Vector2 rayEnd = transform.position + Vector3.down * 10;

            Debug.DrawRay(transform.position, rayEnd - (Vector2)transform.position, Color.red);

            //f�r varje tr�ff cast:en g�r
            //raycasta enbart n�r player r�r sig ner�t
            foreach (RaycastHit2D hit in hits)
            {
                //om tagg = ground, gult streck
                if (hit.collider.CompareTag("Ground"))
                {
                    Transform target = hit.transform;
                    float distanceToTarget = Vector2.Distance(transform.position, hit.point);

                    //halfsize 1.6
                    if (distanceToTarget <= halfPlayerHeight+.5)
                    {
                        GroundInSight = true;
                        //nu f�r descendfunktionen k�ras, den kallas av anim event i inAir
                        Debug.DrawRay(transform.position, hit.point - (Vector2)transform.position, Color.yellow);

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
        //uppdatera Ytranslate-v�rdet f�r att kunna j�mf�ra med f�reg�ende frames 
        previousYvalue = currentYvalue;

        //kollar vem som �r P1 f�r att f� r�tt kontroller
        if (isPlayer1 == true)
        {
            animator.SetFloat("walking", Mathf.Abs(input1X));
            Rb2D.AddForce(transform.right * movementSpeed * input1X);
            //Rb2D.MovePosition(Rb2D.position + Vector2.right * input1X * movementSpeed * Time.deltaTime);

            /*
            input1X ska p�verka movementspeed?
            under 0.01 -> idle
            �ver 0.01 -> walk
            �ver o.25 walk -> run
            �ver .5 run -> sprint
             */

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
        //kollar om P1 + hopp
        if (isPlayer1 == true && isJumping == true)
        {
            animator.SetTrigger("jump");
            StartCoroutine(DelayedJump());
            //f�r fart upp�t 20 frames efter SetTrigger
            //la till en corutine f�r detta, den ligger �verst

            isJumping = false;
            isGrounded = false;


        }
        //kollar om P2 + hopp
        if (isPlayer1 == false && isJumping == true)
        {
            //upward force Rigidbody
            //b�jer kn�na i luften, f�r upp�tkraft f�r snabbt!
            //Rb2D.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
            animator.SetTrigger("jump");
            StartCoroutine(DelayedJump());

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

    //anim event JumpInAir
    public void descend()
    {
        //delay:a �ven descend med ienumerator/x frames?
        if (GroundInSight == true)
        {
            Debug.Log("!!!!!!!!!!!!!!!!!descending");
            animator.SetBool("descend", true);

        }

    }
    //anim event dead
    public void killInstance(int killInst)
    {
        if (killInst == 1)
        {
            //collidern �ndras inte
            colliderDimensions = new Vector2 (GetComponent<BoxCollider2D>().size.y, GetComponent<BoxCollider2D>().size.x);
            
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
}

