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
    //equipNeutral borde vara vec2 f�r att inte �rva skit?
    //public GameObject equipNeutral;

    public GameObject RunP1;
    public GameObject RunP2;    

    public GameObject Sword;
    public int swordPop;
    public GameObject swordInst1;
    public GameObject swordInst2;
    private GameObject swordInst;

    public Transform initialSpawnP1;
    public Transform initialSpawnP2;
    public Transform spawnP1;
    public Transform spawnP2;
    public Transform spawnSwordP1;
    public Transform spawnSwordP2;
    //spawnPos f�r inst = transform, gameobject f�r sword
    //neutral �r ej neutral
    public Transform equipNeutral;
    //private GameObject kinRig;

    //Start is called before the first frame update
    //Awake samma som start fast lite f�re
    void Awake()
    {
        
        StartCoroutine(Spawner());
        cam = GameObject.Find("Main Camera").gameObject;
  
        RunP1 = cam.transform.GetChild(0).GetChild(0).gameObject;
        RunP1.SetActive(false);
        RunP2 = cam.transform.GetChild(0).GetChild(1).gameObject;
        RunP2.SetActive(false);

        //kinRig = Player.transform.GetChild(4).gameObject;
        //kinRig.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (inst1 == null && inst2 == null)
        {
            //neutralisera Target
            cam.GetComponent<Target>().LockOn(cam.transform);
            Debug.Log("no target");
            RunP2.SetActive(false);
            RunP1.SetActive(false);
            //neutral target funkar men
        }
        //h�mta swordInst1 & 2 f�r att kunna checka om de ska spawna

        //swordInst1 = inst1.gameObject.transform.GetChild(0).GetChild(0);
        //swordInst1 = GameObject.FindWithTag("Player1").gameObject.transform.GetChild(0).GetChild(0);

        //swordInst2 = GameObject.FindWithTag("Player2").gameObject;
        //gameObject.transform.GetChild(0).gameObject.layer = 6;

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
        //m�ste skicka en uppdaterad "neutral" pos
        Vector2 equipNeutralminusZ = equipNeutral.transform.position;
    }

    IEnumerator Spawner()
    {
        Vector2 initialSpawnP1minusZ = initialSpawnP1.transform.position;
        Vector2 initialSpawnP2minusZ = initialSpawnP2.transform.position;
        Vector2 spawnP1minusZ = spawnP1.transform.position;
        Vector2 spawnP2minusZ = spawnP2.transform.position;
        //Vector2 spawnSwordP1minusZ = spawnSwordP1.transform.position;
        //Vector2 spawnSwordP2minusZ = spawnSwordP2.transform.position;
        //Vector2 equipNeutralminusZ = equipNeutral.transform.position;

        // equip1 = inst1.transform.Find("equip").gameObject;
        // equip2 = inst2.transform.Find("equip").gameObject;

        bool initialSpawn = true;

        // playerScript.goal m�ste va false f�r att man ska f� spawna
        //l�ttare att skicka bool till bool? vilken inst �r det som har playerscript atm?
        //if (playerScript.goal)
        //initialSpawnPoint i bild
        if (inst1 == null && inst2 == null /*&& swordInst1 == null && swordInst2 == null && */&& initialSpawn == true)
        {
            //p1 spawnar
            inst1 = Instantiate(Player, initialSpawnP1minusZ, Quaternion.identity);
            inst1.GetComponent<Player>().isPlayer1 = true;
            //inst1.GetComponent<Player>().Bc2D.enabled = true;
            inst1.tag = "Player1";
            inst1.layer = 6;
            //equip1-variabel enbart f�r pos att spawna sv�rd p� h�r,
            //equip1 f�r layer tilldelat i Player
            equip1 = inst1.transform.GetChild(0).gameObject;
            
            //p2 spawnar
            inst2 = Instantiate(Player, initialSpawnP2minusZ, Quaternion.identity);
            inst2.transform.localScale = new Vector3(-inst2.transform.localScale.x, inst2.transform.localScale.y, inst2.transform.localScale.z);
            inst2.GetComponent<Player>().isPlayer1 = false;
            //inst2.GetComponent<Player>().Bc2D.enabled = true;
            inst2.tag = "Player2";
            inst2.layer = 7;
            equip2 = inst2.transform.GetChild(0).gameObject;

            //kinRig = inst2.transform.GetChild(4).gameObject;
            //kinRig.layer = 9;
            //kinRig.SetActive(true);

            //instSword(swordInst1, equip1);
            //instSword(swordInst2, equip2);

            initialSpawn = false;

            while (initialSpawn == false)
            {
                //v�nta 3 sek f�r att hinna checka om b�da ska spawna I BILD, n�r target = neutral
                //de ska inte hinna spawna utanf�r bild f�r att programmet m�rkt att EN �r d�d
                yield return new WaitForSeconds(3);

                //initialSpawnPoint i bild
                if (inst1 == null && inst2 == null/* && swordInst1 == null && swordInst2 == null*/)
                {
                    //Debug.Log("3 sek har passerat");
                    //yield return new WaitForSeconds(1);

                    initialSpawnP1minusZ = initialSpawnP1.transform.position;
                    inst1 = Instantiate(Player, initialSpawnP1minusZ, Quaternion.identity);
                    inst1.GetComponent<Player>().isPlayer1 = true;
                    //inst1.GetComponent<Player>().Bc2D.enabled = true;
                    inst1.tag = "Player1";
                    inst1.layer = 6;
                    equip1 = inst1.transform.GetChild(0).gameObject;
                    

                    initialSpawnP2minusZ = initialSpawnP2.transform.position;
                    inst2 = Instantiate(Player, initialSpawnP2minusZ, Quaternion.identity);
                    inst2.transform.localScale = new Vector3(-inst2.transform.localScale.x, inst2.transform.localScale.y, inst2.transform.localScale.z);
                    inst2.GetComponent<Player>().isPlayer1 = false;
                   // inst2.GetComponent<Player>().Bc2D.enabled = true;
                    inst2.tag = "Player2";
                    inst2.layer = 7;
                    equip2 = inst2.transform.GetChild(0).gameObject;

                   // instSword(swordInst1, equip1);
                    //instSword(swordInst2, equip2);
                }
                ////////////////////////////////////////////////////
                //om P1 d�tt ska den spawnas med sv�rd i n�ven
                if (inst1 == null /*&& swordInst1 == null*/)
                {
                    
                    //Debug.Log("inst1 == null");
                    yield return new WaitForSeconds(3);
                    //spawnPoint utanf�r bild
                    //v�nster om Viewport
                    spawnP1minusZ = spawnP1.transform.position;
                    inst1 = Instantiate(Player, spawnP1minusZ, Quaternion.identity);
                    inst1.GetComponent<Player>().isPlayer1 = true;
                    //inst1.GetComponent<Player>().Bc2D.enabled = true;
                    inst1.tag = "Player1";
                    inst1.layer = 6;
                    equip1 = inst1.transform.GetChild(0).gameObject;
                   //instSword(swordInst1, equip1);
                    

                }
                /*
                //om P1 kastat sitt sv�rd ska sv�rd spawnas utanf�r bild
                if (inst1 != null && swordInst1 == null)
                {
                    
                    yield return new WaitForSeconds(3);
                    //�r det denna som �r fel?
                    swordInst1 = Instantiate(Sword, spawnSwordP1minusZ, Quaternion.identity);
                    Debug.Log("swordInst1 spawnar neutralt");
                    // swordInst1.transform.localPosition = Vector3.zero;
                    swordInst1.transform.localScale = new Vector3(0.3f * Player.transform.localScale.x, 0.3f * Player.transform.localScale.y, 0) ;
                    swordInst1.transform.localRotation = Quaternion.identity;
                
                }
                */
                //om P2 d�tt ska den spawnas med sv�rd
                if (inst2 == null/*&& swordInst2 == null*/)
                {
                    
                    yield return new WaitForSeconds(3);
                    //spawnPoint utanf�r bild
                    //h�ger om Viewport
                    spawnP2minusZ = spawnP2.transform.position;
                    inst2 = Instantiate(Player, spawnP2minusZ, Quaternion.identity);
                    inst2.transform.localScale = new Vector3(-inst2.transform.localScale.x, inst2.transform.localScale.y, inst2.transform.localScale.z);
                    inst2.GetComponent<Player>().isPlayer1 = false;
                   // inst2.GetComponent<Player>().Bc2D.enabled = true;
                    inst2.tag = "Player2";
                    inst2.layer = 7;
                    equip2 = inst2.transform.GetChild(0).gameObject;
                    //instSword(swordInst2, equip2);
                    
                }
                /*
                //om P2 kastat sitt sv�rd ska sv�rd spawnas utanf�r bild
                if (inst2 != null && swordInst2 == null)
                {
                    yield return new WaitForSeconds(3);
                    //�r det denna som �r fel?
                    swordInst2 = Instantiate(Sword, spawnSwordP2minusZ, Quaternion.identity);
                    Debug.Log("swordInst2 spawnar neutralt");
                    // hamnar h�gt upp,d�r on collision
                    // swordInst1.transform.localPosition = Vector3.zero;
                    swordInst2.transform.localScale = new Vector3(0.3f * Player.transform.localScale.x, 0.3f * Player.transform.localScale.y, 0);
                    swordInst2.transform.localRotation = Quaternion.identity;
                }
                //////////////////////////////////////////////////////////
                
                //kolla bara om swordInst (neutral) finns?
                if (swordInst1 == null || swordInst2 == null) 
                {
                    //g�r v�ldigt fort, byt till spawnPoint utanf�r bild?
                    yield return new WaitForSeconds(3);
                    
                    swordInst = Instantiate(Sword, spawnSwordP1minusZ, Quaternion.identity);
                    Debug.Log("swordInst spawnar neutralt");
                    
                    //swordInst.transform.localPosition = Vector3.zero;
                    swordInst.transform.localScale = new Vector3(0.3f * Player.transform.localScale.x, 0.3f * Player.transform.localScale.y, 0);
                    swordInst.transform.localRotation = Quaternion.identity;
                }
                */
                yield return null;
            }
        }
    }

    public IEnumerator WaitBeforeToggle()
    {
        yield return new WaitForSeconds(1);
    }
    public void DeathTracker(GameObject objDestroy)
    {
        //StartCoroutine(WaitBeforeToggle());
        //kameran byter lock-on
        //om bara P1 finns
        if (inst2 == objDestroy)
        {
            //skicka med ett l�ngre offset-v�rde f�r att det �r h�r den byter, inte "h�nge med"?
            cam.GetComponent<Target>().LockOn(inst1.transform);
            //Debug.Log("p1 target");
            //StartCoroutine(WaitBeforeToggle());
            RunP2.SetActive(false);
            RunP1.SetActive(true);

            //swordPop--;
            //SwordTracker(swordInst2);

        }
        //om bara P2 finns
        if (inst1 == objDestroy)
        {
            cam.GetComponent<Target>().LockOn(inst2.transform);
            //Debug.Log("p2 target");
            RunP1.SetActive(false);
            RunP2.SetActive(true);

            //swordPop--;
            //SwordTracker(swordInst1);

        }

    }
    //instSword anv�nds enbart f�r sv�rd som spawnar i n�ven
    void instSword(GameObject swordInst, GameObject equipTransform)
    {

        if (swordPop < 2)
        {
            swordInst = Instantiate(Sword, equipTransform.transform.position, Quaternion.identity);
            swordPop++;
            //Debug.Log("swordPop: " + swordPop);
            //parent:a och nollst�ll pos//�rv inte scale!
            swordInst.transform.SetParent(equipTransform.transform);
            //h�mta parentLayer, assigna?
            swordInst.transform.localPosition = Vector3.zero;
            swordInst.transform.localScale = new Vector3(0.3f, 0.3f, 0);
            swordInst.transform.localRotation = Quaternion.identity;
        }


    }
    
    public void SwordTracker(GameObject objDestroy)
    {
        if (swordInst1 == objDestroy)
        {
            swordPop--;
            new WaitForSecondsRealtime(3);
           // instSword(swordInst1, equip1);
        }
        //Missing swordInst2 ref, spawnar �nd� ingen ny
        if (swordInst2 == objDestroy)
        {
            swordPop--;
            new WaitForSecondsRealtime(3);
           // instSword(swordInst2, equip2);
        }
    }

   

}
