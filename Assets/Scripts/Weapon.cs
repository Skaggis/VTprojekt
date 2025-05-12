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
    public GameObject Manager;
    public int throwSpeed;
    private Rigidbody2D gCollRb2D;
    public BoxCollider2D thisBc2D;
    
    // Start is called before the first frame update
    void Start()
    {
        //gäller de med scriptet - fist, foot, sword
        gameObject.SetActive(true);
        thisBc2D = gameObject.GetComponent<BoxCollider2D>();
        //gäller swords child
        gCollRb2D = this.GetComponentInChildren<Rigidbody2D>();
        Manager = GameObject.FindWithTag("Manager");

        //alla svärd -> checka om parents lager är 6 eller 7
        /*
        if (this.gameObject.transform.parent == null)
        {
            this.gameObject.layer = 0;
        }
        else if (this.gameObject.transform.parent.gameObject.layer == 6)
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
        */


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

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //otherRb2D = other.GetComponent<Rigidbody2D>();
        
        //om det är fist eller foot - gör bara skada på other
        if (gameObject.tag == "Weapon" && other.gameObject.layer != 3 && other.isTrigger)
        {
            //Debug.Log(gameObject.tag + other.transform.parent.tag);
            //other.GetComponent<Player>().damageTaken(1);
            Debug.Log("jag "+this.gameObject.name + "krock med "+other.gameObject.name);
            other.GetComponentInParent<Player>().damageTaken(1);

        }
        //om svärdet kollide:ar när den har tilldelat lager (parent)
       if (gameObject.tag == "Sword" && other.gameObject.layer != 3 && other.isTrigger)
        {
            //Debug.Log(gameObject.tag + other.transform.parent.tag);
            //rad 91 får null ref
            Debug.Log("jag " + this.gameObject.name + "krock med " + other.gameObject.name);
            other.GetComponentInParent<Player>().damageTaken(3);
            //static för att kunna passera om svärdet ligger på marken
            //gCollRb2D.bodyType = RigidbodyType2D.Static;

        }
        //om svärdet kollide:ar när den är neutral pick-up

        //DEN FÅR INTE KROCKA MED ANNAT SVÄRD I DETTA LÄGE
        /*
        if (gameObject.tag == "Sword" && gameObject.layer == 0)
        {
            //aktiverar ej sitt child, zero gravity
            gameObject.transform.GetChild(0).gameObject.SetActive(false);

            //parenta enbart om Equip inte redan har barn?
            //out of bounds -> funktion m foreach ist? return eller null
            if (other.transform.GetChild(0).transform.GetChild(0) == null)
            {
                gameObject.transform.SetParent(other.transform.GetChild(0).transform);
                gameObject.transform.localPosition = Vector3.zero;
                //assigna layer för svärd och dess groundColl sker i PlayerScript
                other.GetComponent<Player>().sword = gameObject;
            }
            else
            {
                Debug.Log("equip aldready equipped");
            }
            
        }*/

    }
    public void ThrowSwordNow()
    {
        StartCoroutine(ThrowSword());
    }
    private IEnumerator ThrowSword()
    {

        //kastar åt fel håll när man plockat upp pick-up

        int frameDelay = 25; // Antal FixedUpdate-cykler att vänta, en cykel är 50 fps
        for (int i = 0; i < frameDelay; i++)
        {
            yield return new WaitForFixedUpdate(); // Väntar en physics frame
        }

        //MÅSTE utgå från parents transform som startpos, även efter de-parenting
        //player scale -5 eller 5
        if (gameObject.transform.lossyScale.x < 0)
        {
            Debug.Log(gameObject.transform.lossyScale.x + "mindre än 0");
            //kasta vänster
            gCollRb2D.velocity = new Vector2(throwSpeed * -1, 0);
            //rotation?
            //gCollRb2D.transform.RotateAround(gCollRb2D.transform.position, Vector3.up, 20 * Time.deltaTime);
            gameObject.transform.Rotate(Vector3.up, -180 * Time.deltaTime);

        }
        else if (gameObject.transform.lossyScale.x > 0)
        {
            //kasta höger
            //rätt håll en gång,ibland?
           // Debug.Log(gameObject.transform.lossyScale.x + "större än 0");
            gCollRb2D.velocity = new Vector2(throwSpeed, 0);            
            //rotation?

        }
    
        //svärdets pos blir Rb's pos, hänger med i farten
        gCollRb2D.transform.position = gameObject.transform.position;
        //gCollRb2D.transform.rotation = gameObject.transform.rotation;

        //de-parent
        this.gameObject.transform.parent = null;
        //static body?

        //groundCollider.SetActive(true);
        Destroy(gameObject, 1f);
        //kalla på ny Spawna svärd-funktion???
        Manager.GetComponent<Manager>().swordPop--;
        Manager.GetComponent<Manager>().SwordTracker(gameObject);
        

    }

}


