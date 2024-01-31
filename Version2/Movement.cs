using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Movement : MonoBehaviour
{
        // Start is called before the first frame update
    public GameObject forkLiftMovement;
    public Camera cam;
    public CinemachineFreeLook cinemachineCam;

    // Update is called once per frame
    float xLimit = 2.3f;
    float zLimit = 2.3f;
    float speed = 0.5f;
    float currentPointX = 0;
    float currentPointY = 0;
    bool changePoint = false;
    public static float moveX = 0;
    public static float moveY = 0;
    
void Start(){
}

    
    void FixedUpdate(){
        
if (Input.touchCount > 0) {
	Touch touch = Input.GetTouch(0); // get first touch since touch count is greater than zero
	if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved) 
	{
        if (changePoint == false) {
            currentPointX = touch.position.x;
            currentPointY = touch.position.y;
            }
        moveX = Mathf.Clamp((touch.position.x - currentPointX)/Screen.width, -xLimit,xLimit); 
        moveY = Mathf.Clamp((touch.position.y - currentPointY)/Screen.height, -zLimit,zLimit);
        //if (Mathf.Abs(moveX) > 0.005f) cinemachineCam.m_XAxis.Value += moveX; 
        changePoint = true;
	}
} else {
    changePoint = false;
    moveX = 0;
    
}
        

    }

}
