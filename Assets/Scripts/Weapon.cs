using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;


//using System.Numerics;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Weapon : MonoBehaviour
{
    public GameObject Player;
    public Rigidbody2D Rb2D;
    public Rigidbody2D otherRb2D;
    public BoxCollider2D thisBc2D;
    public BoxCollider2D grannyBc2D;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(true);
        //gäller fist, foot OCH sword
        thisBc2D = gameObject.GetComponent<BoxCollider2D>();
        //otherRb2D = null?
        //otherRb2D = GameObject.Find("Player").GetComponent<Rigidbody2D>();

        Rb2D = gameObject.GetComponent<Rigidbody2D>();
        grannyBc2D = gameObject.transform.parent.parent.GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(thisBc2D, grannyBc2D, true);

    }

    // Update is called once per frame
    void Update()
    {

        //gäller enbart sword/equippable grandchild
        if (gameObject.tag == "Sword")
        {
            Debug.Log("granny är " + gameObject.transform.parent.parent.tag + grannyBc2D);
            Rb2D.velocity = Vector2.zero;

            
        }

        Collider2D hit = Physics2D.OverlapBox(transform.position, thisBc2D.size, 0);
        if (hit != null)
        {
            Debug.Log("Sword träffar: " + hit.gameObject.tag);
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameObject.tag == "Sword" && collision.gameObject.tag != gameObject.transform.parent.parent.tag)
        {
            collision.gameObject.GetComponent<Player>().damageTaken(1);
        }
            
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        otherRb2D = other.GetComponent<Rigidbody2D>();

        //om det är fist eller foot - gör bara skada på other
        if (gameObject.tag == "Weapon" && other.tag != gameObject.transform.parent.tag)
        {
            Debug.Log("Weapon " + other.tag);
            other.GetComponent<Player>().damageTaken(1);

        }

        

    }

}


