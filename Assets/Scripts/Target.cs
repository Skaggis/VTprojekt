using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Target : MonoBehaviour
{
    public Transform target;
    //SMOOTH DAMP
    //time to follow target
    public float smoothTime = 5f;
    //public Vector3 offset;
    //zeros out velocity
    Vector3 velocity = Vector3.zero;
    //LERP
    public int camSpeed = 2;


    /*
    https://stackoverflow.com/questions/35746459/moving-a-camera-from-its-current-position-to-a-specific-position-smoothly-in-un
    */

    // Start is called before the first frame update
    void Start()
    {
        //starta neutralt
        target = transform;
            //new Vector3(target.position.x, 0, -10f);
    }

    //LateUpdate is called after all Update functions have been called
    private void LateUpdate()
    {
        //hastighet(mjukhet i rörelse?
        //transform.position = Vector3.SmoothDamp(transform.position,
        //new Vector3(target.position.x, 0, -10f), ref velocity, smoothTime);
       
    }
    // Update is called once per frame
    void Update()
    {
        //om ingen target angetts vid död,
        //återgå till neutral
        if (target == null)
        {
            target = transform;
            // off center när den återgår
            Debug.Log("no cam target");
        }
        else
        {
            //följ target
            /*   
            transform.position = Vector3.SmoothDamp(transform.position,
            new Vector3(target.position.x, 0, -10f), ref velocity, smoothTime);
            */
            transform.position = Vector2.Lerp(transform.position, new Vector3(target.position.x, 0, -10f), camSpeed * Time.deltaTime);
            Debug.Log("Lerp target"+target);


        }

    }

public void LockOn(Transform currentTarget)
{
//matas av DeathTracker i Manager
//ska röra på sig först när target närmar sig kanten
target = currentTarget;
}

}
