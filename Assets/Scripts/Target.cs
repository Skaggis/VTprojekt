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
        //kameran ska inte kunna r�ra sig n�r b�da spawnar,
        //eller: spelarna kan inte r�ra sig utanf�r "viewport"

        //starta neutralt
    
        //new Vector3(target.position.x, 0, -10f);
        target = transform;
        followY = true;
    }

    //LateUpdate is called after all Update functions have been called
    private void LateUpdate()
    {
        //hastighet(mjukhet i r�relse?
        //transform.position = Vector3.SmoothDamp(transform.position,
        //new Vector3(target.position.x, 0, -10f), ref velocity, smoothTime);
       
    }
    // Update is called once per frame
    void Update()
    {
        //om ingen target angetts vid d�d,
        //�terg� till neutral
        if (target == null)
        {
            target = transform;
            Debug.Log("no cam target");
        }
        else if (target != null && followY == false)
        {
            //f�lj target
            //Vector3 Interpolated value, equals to a + (b - a) * t
            //n�r den har target f�r den inte ha nollat Y-led, beh�ver f�lja spelaren men inte n�r den hoppar
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
        //ska r�ra p� sig f�rst n�r target n�rmar sig kanten
        //f�r n�gon g� utanf�r bild? bara target l�st i bild?

        target = currentTarget;

    }

}
