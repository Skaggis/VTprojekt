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
    //public GameObject hurtBox;
    public int throwSpeed;
    private Rigidbody2D gCollRb2D;
    //public Rigidbody2D otherRb2D;
    public BoxCollider2D thisBc2D;
    public BoxCollider2D grannyBc2D;


    // Start is called before the first frame update
    void Start()
    {
       //Player = GameObject.Find("Player").GetComponent<Player>();
        gameObject.SetActive(true);
        //gäller fist, foot OCH sword
        thisBc2D = gameObject.GetComponent<BoxCollider2D>();
        //otherRb2D = GameObject.Find("Player").GetComponent<Rigidbody2D>();

        //if "weapon" parent bc2d = senare ignore,
        //eller layers???

        //gäller enbart sword/equippable grandchild
        if (gameObject.tag == "Sword")
        {
            grannyBc2D = gameObject.transform.parent.parent.GetComponent<BoxCollider2D>();
            Physics2D.IgnoreCollision(thisBc2D, grannyBc2D, true);
            Debug.Log(grannyBc2D, thisBc2D);
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
        if (gameObject.tag == "Weapon")
        {
            Debug.Log(gameObject + other.transform.parent.tag);
            //other.GetComponent<Player>().damageTaken(1);
            other.GetComponentInParent<Player>().damageTaken(1);
        }

        if (gameObject.tag == "Sword")
        {

            other.GetComponentInParent<Player>().damageTaken(3);
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
        //aktivera parents Hitbox! när player trycker på kast-knappen?
        //this.gameObject.GetComponentInParent<Player>().activeChild(3);
        //this.gameObject.transform.parent.parent.GetComponent<Player>().activeChild(3);
        //de-parent
        this.gameObject.transform.parent = null;
        //static body?
        //hur kan nedan va off men velociteten funkar?
        //groundCollider.SetActive(true);
        Destroy(gameObject, .5f);

        //aktivera hurtbox?

    }

}


