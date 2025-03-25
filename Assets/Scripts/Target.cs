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
    public Vector3 offset;
    public bool followY;
    //zeros out velocity
    Vector3 velocity = Vector3.zero;

    //LERP
    public float camSpeed;


    /*
    https://stackoverflow.com/questions/35746459/moving-a-camera-from-its-current-position-to-a-specific-position-smoothly-in-un
    */

    // Start is called before the first frame update
    void Start()
    {
        //kameran ska inte kunna röra sig när båda spawnar,
        //eller: spelarna kan inte röra sig utanför "viewport"

        //starta neutralt
    
        //new Vector3(target.position.x, 0, -10f);
        target = transform;
        followY = true;
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
            Debug.Log("no cam target");
        }
        else if (target != null && followY == false)
        {
            //följ target
            //Vector3 Interpolated value, equals to a + (b - a) * t
            //när den har target får den inte ha nollat Y-led, behöver följa spelaren men inte när den hoppar
            transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x, 0, -10f), camSpeed * (Time.deltaTime*2));
            Debug.Log("Lerp2target");
            


        }
        else if (target != null && followY == true)
        {
            //func ist
            transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x, target.position.y, -10f), camSpeed * (Time.deltaTime * 2));
        }
        /*
        transform.position = Vector3.SmoothDamp(transform.position,
        new Vector3(target.position.x, 0, -10f), ref velocity, smoothTime);
        */

    }

    public void LockOn(Transform currentTarget)
    {
        //matas av DeathTracker i Manager
        //ska röra på sig först när target närmar sig kanten
        //får någon gå utanför bild? bara target låst i bild?

        target = currentTarget;

    }

}
