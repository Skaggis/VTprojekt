using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
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
     * https://stackoverflow.com/questions/35746459/moving-a-camera-from-its-current-position-to-a-specific-position-smoothly-in-un
     * */
    // Start is called before the first frame update
    void Start()
    {
        //starta neutralt
        target = transform;
    }
    private void LateUpdate()
    {
        //hastighet(mjukhet i rörelse?
        transform.position = Vector3.SmoothDamp(transform.position, 
         new Vector3(0, 0, -10f), ref velocity, 0.2f);
    }
    // Update is called once per frame
    void Update()
    {
        //om ingen atrget angetts vid död,
        //återgå till neutral
        if (target == null)
        {
            target = transform;
        }
        else
        {
            //följ target
            transform.position = new Vector3(target.transform.position.x, 0, -10);

        }

    }

    public void LockOn(Transform currentTarget)
    {
        //matas av DeathTracker i Manager
        target = currentTarget;
    }
}
