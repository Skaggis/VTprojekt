using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.PlayerSettings;
//using static UnityEngine.RuleTile.TilingRuleOutput;


public class Manager : MonoBehaviour
{
    public GameObject cam;
    public GameObject Player;
    private GameObject inst1;
    private GameObject inst2;
    public GameObject equip1;
    public GameObject equip2;
    public GameObject equipNeutral;
    
    public GameObject Sword;
    private GameObject swordInst1;
    private GameObject swordInst2;

    public Transform initialSpawnP1;
    public Transform initialSpawnP2;
    public Transform spawnP1;
    public Transform spawnP2;
    //spawnPos för inst = transform, gameobject för sword
    //public Transform spawnNeutral;
    //public GameObject layer;


    //this.gameObject.transform.parent.parent.GetComponent<Player>().inactivateChild(3); 
    // innan sword spawnas hurtBox.SetActive(false);

    // Start is called before the first frame update
    //Awake samma som start fast lite före
    void Awake()
    {
        
        StartCoroutine(Spawner());
        cam = GameObject.Find("Main Camera").gameObject;
        //halfPlayerHeight = GameObject.Find("Player").GetComponent<SpriteRenderer>().bounds.size.y / 2;

    }

    // Update is called once per frame
    void Update()
    {
        //se till att hela spriten syns
        /*
        Vector3 position = transform.position;

        float distance = transform.position.z - Camera.main.transform.position.z;
        float lowLeftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance)).x + halfPlayerHeight*2;
        float lowRightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distance)).x - halfPlayerHeight*2;
        float hiLeftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, distance)).y - halfPlayerHeight*2;
        float hiRightBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance)).y + halfPlayerHeight*2;

        position.x = Mathf.Clamp(position.x, lowLeftBorder, lowRightBorder);
        position.y = Mathf.Clamp(position.y, hiLeftBorder, hiRightBorder);
        transform.position = position;
        */
  
    }

    IEnumerator Spawner()
    {
        Vector2 initialSpawnP1minusZ = initialSpawnP1.transform.position;
        Vector2 initialSpawnP2minusZ = initialSpawnP2.transform.position;
        Vector2 spawnP1minusZ = spawnP1.transform.position;
        Vector2 spawnP2minusZ = spawnP2.transform.position;
     
        // equip1 = inst1.transform.Find("equip").gameObject;
        // equip2 = inst2.transform.Find("equip").gameObject;

    bool initialSpawn = true;
        //initialSpawnPoint i bild
        if (inst1 == null && inst2 == null && swordInst1 == null && swordInst2 == null && initialSpawn == true)
        {
            //p1 spawnar
            inst1 = Instantiate(Player, initialSpawnP1minusZ, Quaternion.identity);
            inst1.GetComponent<Player>().isPlayer1 = true;
            //inst1.GetComponent<Player>().Bc2D.enabled = true;
            inst1.tag = "Player1";
            inst1.layer = 6;
            //equip1-variabel enbart för pos att spawna svärd på här,
            //equip1 får layer tilldelat i Playerscript via: 
            //child.gameObject.layer = gameObject.layer;
            equip1 = inst1.transform.GetChild(0).gameObject;
            
            //p2 spawnar
            inst2 = Instantiate(Player, initialSpawnP2minusZ, Quaternion.identity);
            inst2.transform.localScale = new Vector3(-inst2.transform.localScale.x, inst2.transform.localScale.y, inst2.transform.localScale.z);
            inst2.GetComponent<Player>().isPlayer1 = false;
            //inst2.GetComponent<Player>().Bc2D.enabled = true;
            inst2.tag = "Player2";
            inst2.layer = 7;
            equip2 = inst2.transform.GetChild(0).gameObject;

            instSword(swordInst1, equip1);
            instSword(swordInst2, equip2);

            initialSpawn = false;

            while (initialSpawn == false)
            {
                //initialSpawnPoint i bild
                if (inst1 == null && inst2 == null && swordInst1 == null && swordInst2 == null)
                {
                    yield return new WaitForSeconds(1);
                    

                    initialSpawnP1minusZ = initialSpawnP1.transform.position;
                    inst1 = Instantiate(Player, initialSpawnP1minusZ, Quaternion.identity);
                    inst1.GetComponent<Player>().isPlayer1 = true;
                    //inst1.GetComponent<Player>().Bc2D.enabled = true;
                    inst1.tag = "Player1";
                    equip1 = inst1.transform.GetChild(0).gameObject;

                    initialSpawnP2minusZ = initialSpawnP2.transform.position;
                    inst2 = Instantiate(Player, initialSpawnP2minusZ, Quaternion.identity);
                    inst2.transform.localScale = new Vector3(-inst2.transform.localScale.x, inst2.transform.localScale.y, inst2.transform.localScale.z);
                    inst2.GetComponent<Player>().isPlayer1 = false;
                   // inst2.GetComponent<Player>().Bc2D.enabled = true;
                    inst2.tag = "Player2";
                    equip2 = inst2.transform.GetChild(0).gameObject;

                    instSword(swordInst1, equip1);
                    instSword(swordInst2, equip2);
                }

                if (inst1 == null && swordInst1 == null)
                {
                    //Debug.Log("inst1 == null");
                    yield return new WaitForSeconds(3);
                    //spawnPoint utanför bild
                    //vänster om Viewport
                    spawnP1minusZ = spawnP1.transform.position;
                    inst1 = Instantiate(Player, spawnP1minusZ, Quaternion.identity);
                    inst1.GetComponent<Player>().isPlayer1 = true;
                   // inst1.GetComponent<Player>().Bc2D.enabled = true;
                    inst1.tag = "Player1";
                    equip1 = inst1.transform.GetChild(0).gameObject;
                    instSword(swordInst1, equip1);
  
                }

                if (inst2 == null && swordInst2 == null)
                {
                    yield return new WaitForSeconds(3);
                    //spawnPoint utanför bild
                    //höger om Viewport
                    spawnP2minusZ = spawnP2.transform.position;
                    inst2 = Instantiate(Player, spawnP2minusZ, Quaternion.identity);
                    inst2.transform.localScale = new Vector3(-inst2.transform.localScale.x, inst2.transform.localScale.y, inst2.transform.localScale.z);
                    inst2.GetComponent<Player>().isPlayer1 = false;
                   // inst2.GetComponent<Player>().Bc2D.enabled = true;
                    inst2.tag = "Player2";
                    equip2 = inst2.transform.GetChild(0).gameObject;
                    instSword(swordInst2, equip2);
                }
                /*
                if (swordInst1 == null)
                {
                   // yield return new WaitForSeconds(3);
                    instSword(swordInst1, equipNeutral);
                   //Player -> inactivateChild(3) (inaktivera hurtbox)
                }
                
                else if (swordInst2 == null)
                {
                    //yield return new WaitForSeconds(3);
                    instSword(swordInst2, spawnNeutral);
                }
                */
                yield return null;
            }
        }
    }
    void instSword(GameObject swordInst, GameObject equipTransform)
    {

        //null ref?
        swordInst = Instantiate(Sword, equipTransform.transform.position, Quaternion.identity);
        //parent:a och nollställ pos//ärv inte scale!
        swordInst.transform.SetParent(equipTransform.transform);
        //hämta parentLayer, assigna?
        swordInst.transform.localPosition = Vector3.zero;
        swordInst.transform.localScale = new Vector3(0.3f, 0.3f, 0);
        swordInst.transform.localRotation = Quaternion.identity;
        /*
        if (equipTransform.gameObject.layer == 6)
        {
            swordInst.transform.gameObject.layer = 6;
        }
        else if (equipTransform.gameObject.layer == 7)
        {
            swordInst.transform.gameObject.layer = 7;
        }
        */
    }

    public void DeathTracker(GameObject objDestroy)
    {
        //kameran byter lock-on
        //om bara P1 finns
        if (inst2 == objDestroy)
        {
            //toggla sprite "RUN ->" 
            cam.GetComponent<Target>().LockOn(inst1.transform);
            //Debug.Log("p1 target");

            //toggla sprite "RUN ->" 

        }
        //om bara P2 finns
        if (inst1 == objDestroy)
        {
            //toggla sprite "RUN ->" 
            cam.GetComponent<Target>().LockOn(inst2.transform);
            //Debug.Log("p2 target");
        }

    }
}
