using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour{
    public GameObject rocket;
    public float zOffset = 20;
    public float xOffset = -12;
    public float yOffset = 8;

    // Update is called once per frame
    void Update(){
        // Get the position of the rocket.
        Vector3 rocketPos = rocket.transform.position;
        rocketPos.z += zOffset;
        rocketPos.x += xOffset;
        rocketPos.y += yOffset;
        transform.position = rocketPos;
    }
}
