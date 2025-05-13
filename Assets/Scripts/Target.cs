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
    private float offset;
    public float camSpeed;
    private Player playerScript;
    Vector3 TargetPos = new Vector3(0, 0, -10);

    // Start is called before the first frame update
    void Start()
    {
        //starta neutralt
        target = transform;

    }

    // Update is called once per frame
    void Update()
    {
       
        //om ingen target angetts vid d�d,
        //�terg� till "neutral"
        if (target == null)
        {
            target = transform;
            
        }

        //f�ljer INTE Y n�r man �r i luften
        if (target != transform && !playerScript.isGrounded)//!isGrounded
        {
            //f�ljer INTE Y
            TargetPos = new Vector3(target.position.x, transform.position.y, -10f);

        }
        else if (target != transform && playerScript.isGrounded)//isGrounded
        {
            //TargetPos f�ljer targets alla led, men �r offset:ad f�r att va i linje med cams neutrala startpos, f�ljer Y
            TargetPos = new Vector3(target.position.x, target.position.y + offset, -10f);

        }
        if (target != transform)
        {
            //camSpeed = camSpeed * 2;
            transform.position = Vector3.Lerp(transform.position, TargetPos, camSpeed * (Time.deltaTime));

        }

    }

    public void LockOn(Transform currentTarget)
    {
        //matas av DeathTracker i Manager
        //ska r�ra p� sig f�rst n�r target n�rmar sig kanten
        //f�r n�gon g� utanf�r bild? bara target l�st i bild?

        target = currentTarget;

        //???
        playerScript = currentTarget.GetComponent<Player>();
        
        //camSpeed = camSpeed / 2;

        offset = transform.position.y - target.position.y;

    }

}
