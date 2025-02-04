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
    public Vector3 offset;
    //zeros out velocity
    Vector3 velocity = Vector3.zero;
    //time to follow target
    public float smoothTime = .15f;

    /*
    https://stackoverflow.com/questions/35746459/moving-a-camera-from-its-current-position-to-a-specific-position-smoothly-in-un
    */

    // Start is called before the first frame update
    void Start()
    {
        //starta neutralt
        target = transform;
    }

    private void LateUpdate()
    {
        //hastighet(mjukhet i rörelse?
        /*
        transform.position = Vector3.SmoothDamp(transform.position, 
         target.position + offset, ref velocity, smoothTime);
        */
        transform.position = Vector3.SmoothDamp(transform.position,
        new Vector3(target.position.x, 0, -10f), ref velocity, smoothTime);
       
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
            transform.position = new Vector3(target.transform.position.x, 0, -10);
            /*
            MainCamera.transform.position = Vector3.Lerp(transform.position, TargetPosition.transform.position, speed * Time.deltaTime);
            MainCamera.transform.rotation = Quaternion.Lerp(transform.rotation, TargetPosition.transform.rotation, speed * Time.deltaTime);
            */
        }

    }

    public void LockOn(Transform currentTarget)
    {
        //matas av DeathTracker i Manager
        //ska röra på sig först när target närmar sig kanten
        target = currentTarget;
    }
}
