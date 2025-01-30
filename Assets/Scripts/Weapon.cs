using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Weapon : MonoBehaviour
{
    public GameObject Player1;
    public GameObject Player2;

    // Start is called before the first frame update
    void Start()
    {
        //Weapon.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player1" || other.tag == "Player2")
        {
            
            other.GetComponent<Player>().damageTaken(1);

        }

    }
}
