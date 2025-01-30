using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Target : MonoBehaviour
{
    public Transform player1;
    public Transform player2;
    public Transform target;
    public Vector3 offset;
    //zeros out velocity
    Vector3 velocity = Vector3.zero;
    //time to follow target
    public float smoothTime = .15f;


    // Start is called before the first frame update
    void Start()
    {

    }
    private void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, 
            new Vector3(target.position.x, 0, -10f), ref velocity, 0.2f);
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log(player1.position);
       transform.position = new Vector3(player1.position.x + 4, 0, -10); // Camera follows the player1 but 4 to the right
       //transform.position = new Vector3(player1.position.x + offset.x, 0, -10);
    }
}
