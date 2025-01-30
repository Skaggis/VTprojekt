using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Manager : MonoBehaviour
{
    public GameObject cam;
    public GameObject Player1;
    public GameObject Player2;
    private GameObject inst2;
    private GameObject inst1;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawner());
        cam = GameObject.Find("Main Camera").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Spawner()
    {
        bool initialSpawn = true;

        while (true)
        {
            //om båda dör och ska "initialSpawn:a" igen, sapa ny bool variabel
            //och släng in i villkor nedan
            if (inst1 == null)
                {
                if (initialSpawn == false)
                {
                    yield return new WaitForSeconds(5);
                }
                
                inst1 = Instantiate(Player1, new Vector3(-1, 0, 0), Quaternion.identity);
                inst1.GetComponent<Player>().isPlayer1 = true;
                Debug.Log("player1 spawned");
                    
                }
            
                if (inst2 == null)
                {
                if (initialSpawn == false)
                {
                    yield return new WaitForSeconds(5);
                }
                
                inst2 = Instantiate(Player2, new Vector3(+1, 0, 0), Quaternion.identity);
                Debug.Log("player2 spawned");
                
                }
                initialSpawn = false;
                yield return null;
        }
        
    }

    public void DeathTracker(GameObject objDestroy)
    {
        //kameran byter lock-on - lager?
        //"neutral", player1 eller player2

        Debug.Log("destroyed" + objDestroy.name);
        Destroy(objDestroy);
        
    }
}
