using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

//using System.Numerics;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Weapon : MonoBehaviour
{
    public GameObject Player;
    public Rigidbody2D Rb2D;
    public BoxCollider2D thisBc2D;
    public BoxCollider2D grannyBc2D;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(true);
        //gäller fist, foot OCH sword
        thisBc2D = gameObject.GetComponent<BoxCollider2D>();
        
        //gäller enbart sword/equippable grandchild
        if (gameObject.tag == "Sword")
        {
            Debug.Log("iam Sword");//CHECK
            Rb2D = gameObject.GetComponent<Rigidbody2D>();
            //HMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM!
            Rb2D.velocity = Vector2.zero;
            grannyBc2D = transform.parent.parent.GetComponent<BoxCollider2D>();
            Physics2D.IgnoreCollision(thisBc2D, grannyBc2D, true);
            /*
            if (grannyBc2D != null && thisBc2D != null)
            {
                Debug.Log("Sword ignore granny");//CHECK
                Physics2D.IgnoreCollision(thisBc2D, grannyBc2D, true);
            }
            */
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {

        //om det är fist eller foot - gör bara skada på other
        if (gameObject.tag == "Weapon" && other.tag != gameObject.transform.parent.tag)
        {
            //null ref???
            other.GetComponent<Player>().damageTaken(1);

        }
        /*
        if (other.velocity.y > gameObject.velocity.y)
        {
            gameObject.SetActive(false);
            //ska falla till marken
        }
        */

    }
    //collision pga sword har rigidbody
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //uteslut den andra

        //om det är ett svärd - gör bara skada på Other
        if (gameObject.tag == "Sword" && gameObject.transform.parent.parent.tag != collision.gameObject.tag)
        {
            Debug.Log("iam Sword & other != granny");
            //funkar int alls & klagar när man collide:ar med Goal t.ex.
            collision.gameObject.GetComponent<Player>().damageTaken(3);

            /*
            //se till att de inte puttar varandra
            other.GetComponent<RigidBody2D>().velocity = Vector2.zero;
            other.GetComponent<RigidBody2D>().isKinematic = true;
            
            */

        if (gameObject.tag == "Sword" && collision.gameObject.tag == "Sword") //&& jag/denna inst har fart
        {

                // other.GetComponent<Player>(). drop the child FUNC???
                //Physics2D.IgnoreCollision(Bc2D, transform.parent.GetComponent<BoxCollider2D>(), false);



                //se till att de inte puttar varandra
                //collision.gameObject.GetComponent<RigidBody2D>().velocity = Vector2.zero;
                //collision.gameObject.GetComponent<RigidBody2D>().isKinematic = true;
            


        }
        
        }
    }
}


