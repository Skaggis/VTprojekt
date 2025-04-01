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
    public GameObject equip;
    public GameObject Sword;
    private GameObject inst1;
    private GameObject inst2;


    public Transform initialSpawnP1;
    public Transform initialSpawnP2;
    public Transform spawnP1;
    public Transform spawnP2;

    // Start is called before the first frame update
    //Awake samma som start fast lite före
    void Awake()
    {
        StartCoroutine(Spawner());
        cam = GameObject.Find("Main Camera").gameObject;
        equip = GameObject.Find("equip").gameObject;

    }

    // Update is called once per frame
    void Update()
    {

        //se till att hela spriten syns
        /*
        Vector3 position = transform.position;

        float distance = transform.position.z - Camera.main.transform.position.z;
        float leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance)).x + halfPlayerHeight*2;
        float rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distance)).x - halfPlayerHeight*2;
        float upBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, distance)).y - halfPlayerHeight*2;
        float downBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance)).y + halfPlayerHeight*2;

        position.x = Mathf.Clamp(position.x, leftBorder, rightBorder);
        position.y = Mathf.Clamp(position.y, downBorder, upBorder);
        transform.position = position;
        */

    }

    IEnumerator Spawner()
    {
        Vector2 initialSpawnP1minusZ = initialSpawnP1.transform.position;
        Vector2 initialSpawnP2minusZ = initialSpawnP2.transform.position;
        Vector2 spawnP1minusZ = spawnP1.transform.position;
        Vector2 spawnP2minusZ = spawnP2.transform.position;

        bool initialSpawn = true;
        //initialSpawnPoint i bild
        if (inst1 == null && inst2 == null && initialSpawn == true)
        {
            
            inst1 = Instantiate(Player, initialSpawnP1minusZ, Quaternion.identity);
            inst1.GetComponent<Player>().isPlayer1 = true;
            //inst1.GetComponent<Player>().Bc2D.enabled = true;
            inst1.tag = "Player1";
            //testar spawna svärd på equip's pos
            GameObject equip = inst1.transform.Find("equip").gameObject;
            GameObject sword = Instantiate(Sword, equip.transform.position, Quaternion.identity);
            //parent:a och nollställ pos
            sword.transform.SetParent(equip.transform);
            sword.transform.localPosition = Vector3.zero;
            sword.transform.localScale = new Vector3(0.3f,0.3f,0);
            sword.transform.localRotation = Quaternion.identity;
            //ärv inte scale!
  

            //p2 spawnar
            inst2 = Instantiate(Player, initialSpawnP2minusZ, Quaternion.identity);
            inst2.transform.localScale = new Vector3(-inst2.transform.localScale.x, inst2.transform.localScale.y, inst2.transform.localScale.z);
            inst2.GetComponent<Player>().isPlayer1 = false;
            //inst2.GetComponent<Player>().Bc2D.enabled = true;
            inst2.tag = "Player2";



            initialSpawn = false;

            while (initialSpawn == false)
            {
                //initialSpawnPoint i bild
                if (inst1 == null && inst2 == null)
                {
                    yield return new WaitForSeconds(1);
                    initialSpawnP1minusZ = initialSpawnP1.transform.position;
                    inst1 = Instantiate(Player, initialSpawnP1minusZ, Quaternion.identity);
                    inst1.GetComponent<Player>().isPlayer1 = true;
                    //inst1.GetComponent<Player>().Bc2D.enabled = true;
                    inst1.tag = "Player1";
                    initialSpawnP2minusZ = initialSpawnP2.transform.position;
                    inst2 = Instantiate(Player, initialSpawnP2minusZ, Quaternion.identity);
                    inst2.GetComponent<Player>().isPlayer1 = false;
                   // inst2.GetComponent<Player>().Bc2D.enabled = true;
                    inst2.tag = "Player2";
                    inst2.transform.localScale = new Vector3(-inst2.transform.localScale.x, inst2.transform.localScale.y, inst2.transform.localScale.z);
                }

                if (inst1 == null)
                {
                    yield return new WaitForSeconds(3);
                    //spawnPoint utanför bild
                    //vänster om Viewport
                    spawnP1minusZ = spawnP1.transform.position;
                    inst1 = Instantiate(Player, spawnP1minusZ, Quaternion.identity);
                    inst1.GetComponent<Player>().isPlayer1 = true;
                   // inst1.GetComponent<Player>().Bc2D.enabled = true;
                    inst1.tag = "Player1";
                    

                }

                if (inst2 == null)
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
                }

                yield return null;
            }
        }
    }

    public void DeathTracker(GameObject objDestroy)
    {
        //kameran byter lock-on
        

        //om bara P1 finns
        if (inst2 == objDestroy)
        {
            //toggla sprite "RUN ->" 
            cam.GetComponent<Target>().LockOn(inst1.transform);
            Debug.Log("p1 target");

            //toggla sprite "RUN ->" 

        }
        //om bara P2 finns
        if (inst1 == objDestroy)
        {
            //toggla sprite "RUN ->" 
            cam.GetComponent<Target>().LockOn(inst2.transform);
            Debug.Log("p2 target");
        }

    }
}
