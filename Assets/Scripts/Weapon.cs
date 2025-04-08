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
    //public GameObject Player;
    //public GameObject groundCollider;
    //public GameObject hurtBox;
    public int throwSpeed;
    private Rigidbody2D gCollRb2D;
    //public Rigidbody2D otherRb2D;
    public BoxCollider2D thisBc2D;
    //public BoxCollider2D grannyBc2D;


    // Start is called before the first frame update
    void Start()
    {
        //gäller de med scriptet - fist, foot, sword
        gameObject.SetActive(true);
        thisBc2D = gameObject.GetComponent<BoxCollider2D>();
        //gäller swords child
        gCollRb2D = this.GetComponentInChildren<Rigidbody2D>();

        //alla svärd -> checka om parents lager är 6 eller 7
        if (this.gameObject.transform.parent.gameObject.layer == 6)
        {
            this.gameObject.layer = 6;
            //sword groundColl
            this.gameObject.transform.GetChild(0).gameObject.layer = 6;
        }
        else if (this.gameObject.transform.parent.gameObject.layer == 7)
        {
            this.gameObject.layer = 7;
            this.gameObject.transform.GetChild(0).gameObject.layer = 7;
        }


        #region gammal prevent collision
        /*
        //gäller enbart sword/equippable grandchild
        if (gameObject.tag == "Sword")
        {
            grannyBc2D = gameObject.transform.parent.parent.GetComponent<BoxCollider2D>();
            Physics2D.IgnoreCollision(thisBc2D, grannyBc2D, true);

            //har Rb2D + boxCollider
            groundCollider = this.gameObject.transform.GetChild(0).gameObject;
            groundCollider.SetActive(false);
            gCollRb2D = this.GetComponentInChildren<Rigidbody2D>();
            Debug.Log("thisBc2D: " + thisBc2D + " grannyBc2D: " + grannyBc2D);
        }
        */
        #endregion
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //otherRb2D = other.GetComponent<Rigidbody2D>();
        
        //om det är fist eller foot - gör bara skada på other
        if (gameObject.tag == "Weapon")
        {
            //Debug.Log(gameObject.tag + other.transform.parent.tag);
            //other.GetComponent<Player>().damageTaken(1);
            other.GetComponentInParent<Player>().damageTaken(1);
        }

        if (gameObject.tag == "Sword")
        {
            //Debug.Log(gameObject.tag + other.transform.parent.tag);
            other.GetComponentInParent<Player>().damageTaken(3);
            //static för att kunna passera om svärdet ligger på marken
            //gCollRb2D.bodyType = RigidbodyType2D.Static;

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

        //MÅSTE utgå från parents transform som startpos, även efter de-parenting
        //player scale -5 eller 5
        if (gameObject.transform.lossyScale.x < 0)
        {
            //kasta vänster
            gCollRb2D.velocity = new Vector2(throwSpeed * -1, 0);
        }
        else if (gameObject.transform.lossyScale.x > 0)
        {
            //kasta höger
            gCollRb2D.velocity = new Vector2(throwSpeed, 0);
        }
    
        //svärdets pos blir Rb's pos
        gCollRb2D.transform.position = gameObject.transform.position;
        //de-parent
        this.gameObject.transform.parent = null;
        //static body?

        //groundCollider.SetActive(true);
        Destroy(gameObject, .5f);

        //aktivera hurtbox?

    }

}


