using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Weapon : MonoBehaviour
{
    public GameObject Player1;
    public GameObject Player2;
    public Rigidbody2D Rigidbody2;

    // Start is called before the first frame update
    void Start()
    {
        //Weapon.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        //uteslut den andra
        if (other.tag == "Player1" || other.tag == "Player2") //&& jag/denna inst har fart
        {
            //P2 d�dar b�da om b�da har fists
            //P1 d�dar P2
           // other.GetComponent<Player>().damageTaken(1);
            other.GetComponent<Animator>().SetTrigger("dead");

            //se till att de inte puttar varandra

            //other.GetComponent<RigidBody2D>().velocity = Vector2.zero;
            //other.GetComponent<RigidBody2D>().isKinematic = true;



        }


    }
}

