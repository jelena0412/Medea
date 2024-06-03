using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow: MonoBehaviour
{
    public bool movingY;
    public float FollowSpeed = 2f;
    public Transform target;
    //Logika gde kamera prati poziciju igraca po x i y osi (ukoliko je movingY true) i za Y osu ima mali
    //offset da podigne kameru malo nagore za 2
    private void Update()
    {
        if (target == null) return;

        if(movingY)
        {
            Vector3 newPos = new Vector3(target.position.x, target.position.y + 2, -10f);
            transform.position = Vector3.Slerp(transform.position, newPos, FollowSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 newPos = new Vector3(target.position.x, transform.position.y, -10f);
            transform.position = Vector3.Slerp(transform.position, newPos, FollowSpeed * Time.deltaTime);
        }
        
    }
}
