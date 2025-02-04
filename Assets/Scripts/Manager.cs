using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Manager : MonoBehaviour
{
    public Camera cam;
    public GameObject Player1;
    public GameObject Player2;
    private GameObject inst2;
    private GameObject inst1;



    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawner());
        //cam = GameObject.Find("Main Camera").gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        //komma åt viewport rect 
        Vector2 screenPoint = Camera.main.WorldToViewportPoint(Screen.width, Screen.height);
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(cam.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.width, Camera.main.farClipPlane / 2));
        }
    }
    /*
    void OnDrawGizmosSelected()
    {
        
        Vector3 p = Camera.main.ViewportToWorldPoint(new Vector2(cam.width, cam.hight, 1));
        Vector2 screenPoint = Camera.main.WorldToViewportPoint(point);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(p, 0.1F);
    }
    */
    IEnumerator Spawner()
    {
        bool initialSpawn = true;

        while (true)
        {
            //om båda dör och ska "initialSpawn:a" igen, skapa ny bool variabel
            //och släng in i villkor nedan
            //eller koord för initialspawn? kör samma koord igen?
            if (inst1 == null)
                {
                if (initialSpawn == false)
                {
                    //if initialSpawn = false, spawna helt t.v om camViewport?
                    yield return new WaitForSeconds(5);
                }
                //ändra till vänster sida i Viewport
                inst1 = Instantiate(Player1, new Vector3(-3, 0, 0), Quaternion.identity);
                inst1.GetComponent<Player>().isPlayer1 = true;
                Debug.Log("player1 spawned");
                    
                }
            
                if (inst2 == null)
                {
                if (initialSpawn == false)
                {
                    //if initialSpawn = false, spawna t.h om cam
                    yield return new WaitForSeconds(5);
                }
                //ändra till vänster sida i Viewport
                inst2 = Instantiate(Player2, new Vector3(+3, 0, 0), Quaternion.identity);
                inst2.transform.localScale = new Vector3(-inst2.transform.localScale.x, inst2.transform.localScale.y, inst2.transform.localScale.z);
                Debug.Log("player2 spawned");


            }
            initialSpawn = false;
                yield return null;
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
