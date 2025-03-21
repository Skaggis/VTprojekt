using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Weapon : MonoBehaviour
{
    public GameObject Player;
    public Rigidbody2D Rb2D;
    public BoxCollider2D Bc2D;

    // Start is called before the first frame update
    void Start()
    {
        //Weapon.SetActive(false);
        //transform.parent boxcollider ignore

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        //uteslut den andra
        if (other.gameObject.tag != transform.parent.parent.tag) //&& jag/denna inst har fart
        {
           // other.GetComponent<Player>().damageTaken(1);
            other.gameObject.GetComponent<Animator>().SetTrigger("dead");

            /*
            //se till att de inte puttar varandra
            other.GetComponent<RigidBody2D>().velocity = Vector2.zero;
            other.GetComponent<RigidBody2D>().isKinematic = true;
            */
        }
        else
        {
            Physics2D.IgnoreCollision(Bc2D, transform.parent.parent.GetComponent<BoxCollider2D>(), true);
        }


        if (other.gameObject.tag == "Sword") //&& jag/denna inst har fart
        {

            // other.GetComponent<Player>(). drop the child
            //Physics2D.IgnoreCollision(Bc2D, transform.parent.GetComponent<BoxCollider2D>(), false);
            /*
            //se till att de inte puttar varandra
            other.GetComponent<RigidBody2D>().velocity = Vector2.zero;
            other.GetComponent<RigidBody2D>().isKinematic = true;
            */


        }


    }
}

