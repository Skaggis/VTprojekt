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
    public float offset;
    public bool followY;
    //LERP
    public float camSpeed;

    Vector3 TargetPos = new Vector3(0, 0, -10);
    Vector3 JumpPos = new Vector3(0,0,-10);

    /*
    https://stackoverflow.com/questions/35746459/moving-a-camera-from-its-current-position-to-a-specific-position-smoothly-in-un
    */

    // Start is called before the first frame update
    void Start()
    {
        //starta neutralt, f�ljer Y f�r terr�ng
   
        target = transform;
        followY = true;
        //player Ypos: -2.147894
       // offset = -2.147894f;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        //Vector3.Distance(a, b)
        offset = Vector3.Distance(target.transform.position, transform.position);
        print("Distance/offset to other: " + offset);
        //normalized
        Vector3 distanceVector = transform.position - target.position;
        Vector3 distanceVectorNormalized = distanceVector.normalized;
        Vector3 targetPosition = distanceVectorNormalized;
        transform.position = targetPosition;
        */
        //om ingen target angetts vid d�d,
        //�terg� till neutral
        if (target == null)
        {
            target = transform;
            
        }
        
         if (target != transform)
        {
            //offsettar ok, kan hoppa utan camr�relse
            //vill att kameran f�ljer med till andra niv�er!?
            offset = transform.position.y - target.position.y;
            //TargetPos f�ljer targets alla led, men �r offset:ad f�r att va i linje med cams neutrala startpos
            //f�ljer Y
            TargetPos = new Vector3(target.position.x, target.position.y + offset, -10f);
            //f�ljer INTE Y
            JumpPos = new Vector3(target.position.x, transform.position.y, -10f);
            //Debug.Log(TargetPos);

        }
        /*
        if (target == transform)
        {
            Debug.Log("no cam target");
        }
        */
        if (target != null && followY == false)
        {
            //f�lj target
            //Vector3 Interpolated value, equals to a + (b - a) * t
            //n�r den har target f�r den f�lja spelaren
            //notGrounded = followY FALSE
            transform.position = Vector3.Lerp(transform.position, JumpPos, camSpeed * (Time.deltaTime * 2));
            Debug.Log("Lerp2t followY F");

        }
        else if (target != null && followY == true)
        {
            //offset:a target.pos.y Ypos n�r P = target: -2.147894
            transform.position = Vector3.Lerp(transform.position, TargetPos, camSpeed * (Time.deltaTime * 2));
            Debug.Log("Lerp2t followY T");
        }

    }

    public void LockOn(Transform currentTarget)
    {
        //matas av DeathTracker i Manager
        //ska r�ra p� sig f�rst n�r target n�rmar sig kanten
        //f�r n�gon g� utanf�r bild? bara target l�st i bild?
        //target.position = new Vector3(currentTarget.position.x, transform.position.y, currentTarget.position.z);
        target = currentTarget;

    }

}
