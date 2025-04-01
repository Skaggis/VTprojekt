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
    public GameObject groundCollider;
    public int throwSpeed;
    private Rigidbody2D gCollRb2D;
   // public Rigidbody2D otherRb2D;
    public BoxCollider2D thisBc2D;
    public BoxCollider2D grannyBc2D;


    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(true);
        //gäller fist, foot OCH sword
        thisBc2D = gameObject.GetComponent<BoxCollider2D>();
        //otherRb2D = GameObject.Find("Player").GetComponent<Rigidbody2D>();
        

        //gäller enbart sword/equippable grandchild
        if (gameObject.tag == "Sword")
        {
            grannyBc2D = gameObject.transform.parent.parent.GetComponent<BoxCollider2D>();
            Physics2D.IgnoreCollision(thisBc2D, grannyBc2D, true);

            //har Rb2D + boxCollider
            groundCollider = this.gameObject.transform.GetChild(0).gameObject;
            groundCollider.SetActive(false);
            gCollRb2D = this.GetComponentInChildren<Rigidbody2D>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //otherRb2D = other.GetComponent<Rigidbody2D>();
        
        //om det är fist eller foot - gör bara skada på other
        if (gameObject.tag == "Weapon" && other.tag != gameObject.transform.parent.tag)
        {
            Debug.Log("Weapon " + other.transform.parent.tag);
            //other.GetComponent<Player>().damageTaken(1);
            other.GetComponentInParent<Player>().damageTaken(1);
        }

        if (gameObject.tag == "Sword")
        {
            Debug.Log("!"+ other.tag);
            other.GetComponentInParent<Player>().damageTaken(3);
        }


    }
    public void ThrowSwordNow()
    {
        StartCoroutine(ThrowSword());
    }
    private IEnumerator ThrowSword()
    {
        int frameDelay = 25; // Antal FixedUpdate-cykler att vänta, en cykel är 50 fps
        for (int i = 0; i < frameDelay; i++)
        {
            yield return new WaitForFixedUpdate(); // Väntar en physics frame
        }
        //kasta
        gCollRb2D.velocity = new Vector2(throwSpeed, 0);
        //svärdets pos blir Rb's pos
        gCollRb2D.transform.position = gameObject.transform.position;

        //de-parent
        this.gameObject.transform.parent = null;
        //static body?
        groundCollider.SetActive(true);

    }

}


