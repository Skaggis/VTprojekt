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
    public GameObject Player1;
    public GameObject Player2;
    private GameObject inst1;
    private GameObject inst2;
    public Transform initialSpawnP1;
    public Transform initialSpawnP2;
    public Transform spawnP1;
    public Transform spawnP2;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawner());
        cam = GameObject.Find("Main Camera").gameObject;
        //cam = GetComponent<Camera>();

    }

    // Update is called once per frame
    void Update()
    {
        /*
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(new Vector3(Screen.width, Screen.height, Camera.main.farClipPlane / 2));
        //Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);


        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(screenPoint);
        }
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
            
            inst1 = Instantiate(Player1, initialSpawnP1minusZ, Quaternion.identity);
            inst1.GetComponent<Player>().isPlayer1 = true;
            Debug.Log("P1 spawned");

            inst2 = Instantiate(Player2, initialSpawnP2minusZ, Quaternion.identity);
            inst2.transform.localScale = new Vector3(-inst2.transform.localScale.x, inst2.transform.localScale.y, inst2.transform.localScale.z);
            Debug.Log("P2 spawned");

            initialSpawn = false;

            while (initialSpawn == false)
            {
                //initialSpawnPoint i bild
                if (inst1 == null && inst2 == null)
                {
                    yield return new WaitForSeconds(1);
                    initialSpawnP1minusZ = initialSpawnP1.transform.position;
                    inst1 = Instantiate(Player1, initialSpawnP1minusZ, Quaternion.identity);
                    inst1.GetComponent<Player>().isPlayer1 = true;
                    initialSpawnP2minusZ = initialSpawnP2.transform.position;
                    inst2 = Instantiate(Player2, initialSpawnP2minusZ, Quaternion.identity);
                    inst2.transform.localScale = new Vector3(-inst2.transform.localScale.x, inst2.transform.localScale.y, inst2.transform.localScale.z);
                }

                if (inst1 == null)
                {
                    yield return new WaitForSeconds(3);
                    //spawnPoint utanför bild
                    //vänster om Viewport
                    spawnP1minusZ = spawnP1.transform.position;
                    inst1 = Instantiate(Player1, spawnP1minusZ, Quaternion.identity);
                    inst1.GetComponent<Player>().isPlayer1 = true;
                    Debug.Log("P1 spawned");

                }

                if (inst2 == null)
                {
                    yield return new WaitForSeconds(3);
                    //spawnPoint utanför bild
                    //höger om Viewport
                    spawnP2minusZ = spawnP2.transform.position;
                    inst2 = Instantiate(Player2, spawnP2minusZ, Quaternion.identity);
                    inst2.transform.localScale = new Vector3(-inst2.transform.localScale.x, inst2.transform.localScale.y, inst2.transform.localScale.z);
                    Debug.Log("P2 spawned");
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
            cam.GetComponent<Target>().LockOn(inst1.transform);
            Debug.Log("p1 target");

            //toggla sprite "RUN ->" 

            //börjar följa först när spelaren närmar sig kanten av viewport
            //funkar inte
            /*
            if (inst1.transform.position.x == 7)
            {
                cam.GetComponent<Target>().LockOn(inst1.transform);
            }
            */

        }
        //om bara P2 finns
        if (inst1 == objDestroy)
        {
            //toggla sprite "RUN ->" 

            cam.GetComponent<Target>().LockOn(inst2.transform);
            Debug.Log("p2 target");
        }

        Debug.Log("destroyed" + objDestroy.name);
        Destroy(objDestroy);


    }
}
