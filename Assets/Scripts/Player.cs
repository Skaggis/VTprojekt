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
    float input1X;
    float input1Y;
    float input2X;
    float input2Y;

    // Start is called before the first frame update
    void Start()
    {
        Manager = GameObject.FindWithTag("Manager").gameObject;
        Rb2D = gameObject.GetComponent<Rigidbody2D>();

    }
    void Update()
    {

        if (isPlayer1 == true)
        {
            input1X = Input.GetAxis("Horizontal_Player1");
            input1Y = Input.GetAxis("Vertical_Player1");
        }
        else
        {
            input2X = Input.GetAxis("Horizontal_Player2");
            input2Y = Input.GetAxis("Vertical_Player2");
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
    
        if (isPlayer1 == true)
        {
            Rb2D.AddForce(transform.right * movementSpeed * input1X);
            //Rb2D.AddForce(transform.up * movementSpeed * input1Y);
            // Check if the player presses the jump key (space bar)
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Apply an upward force to the Rigidbody
                Rb2D.AddForce(Vector3.up * jumpSpeed, ForceMode2D.Impulse);
            }
        }
        else
        {
            Rb2D.AddForce(transform.right * movementSpeed * input2X);
            //Rb2D.AddForce(transform.up * movementSpeed * input2Y);
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                // Apply an upward force to the Rigidbody
                Rb2D.AddForce(Vector3.up * jumpSpeed, ForceMode2D.Impulse);
            }
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

