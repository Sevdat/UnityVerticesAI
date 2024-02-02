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
    float xLimit = 3.3f;
    float yLimit = 0.3f;
    float currentPoint2X = 0f;
    float currentPoint2Y = 0f;
    float currentPointX = 0;
    float currentPointY = 0;
    bool changePointSingle = false;
    bool changePointDouble = false;
    public static float moveX = 0;
    public static float moveY = 0;
    public static float moveZ = 0;
    public static float side = 0;

    
void Start(){
}

    void singleTouch(){
        Touch touch = Input.GetTouch(0); // get first touch since touch count is greater than zero
        if (changePointSingle == false) {
            currentPointX = touch.position.x;
            currentPointY = touch.position.y;
            }
        moveX = Mathf.Clamp((touch.position.x - currentPointX)/175, -xLimit,xLimit); 
        moveY = Mathf.Clamp((touch.position.y - currentPointY)/3300, -yLimit,yLimit);
        changePointSingle = true;
    }

    void doubleTouch(){
        Touch touch = Input.GetTouch(0); // get first touch since touch count is greater than zero
        Touch touch2 = Input.GetTouch(1);
        if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved) 
        {
            if (changePointDouble == false) {
                currentPointX = touch.position.x;
                currentPointY = touch.position.y;
                currentPoint2Y = touch2.position.y;
                currentPoint2X = touch2.position.x;
                }
            moveX = Mathf.Clamp((touch.position.x - currentPointX)/175, -xLimit,xLimit); 
            moveY = Mathf.Clamp((touch.position.y - currentPointY)/3300, -yLimit,yLimit);
            if (RenderScript.mobility && Mathf.Abs(moveX) > 0.001f) cinemachineCam.m_XAxis.Value += moveX;
            if (RenderScript.mobility && Mathf.Abs(moveY) > 0.001f) cinemachineCam.m_YAxis.Value -= moveY/15; 
            
            moveZ = Mathf.Clamp((touch2.position.y - currentPoint2Y)/4000, -xLimit,xLimit);
            side = Mathf.Clamp((touch2.position.x - currentPoint2X)/4000, -yLimit,yLimit);
            if (RenderScript.mobility && Mathf.Abs(moveZ) > 0.001f) forkLiftMovement.transform.position +=  cam.transform.forward*moveZ;
            if (RenderScript.mobility && Mathf.Abs(side) > 0.001f) forkLiftMovement.transform.position +=  cam.transform.right*side;  
            changePointDouble = true;
	    }
    }
    void Update(){
        
        if (Input.touchCount == 1) singleTouch();
            else 
            if (Input.touchCount == 2) doubleTouch(); 
            else {
                changePointSingle = false;
                changePointDouble = false;
                moveX = 0;
                moveY = 0;
                moveZ = 0;
                side = 0;  
            }
    }

}
