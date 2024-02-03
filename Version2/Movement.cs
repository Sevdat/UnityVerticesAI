using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Movement : MonoBehaviour
{
    public GameObject player;
    public Camera cam;
    public CinemachineFreeLook cinemachineCam;
    bool doubleReposition = true;
    bool singleReposition = true;
    float screenWidth = 0;
    float touchCount = 0;
    public static float rightOriginX = 0f;
    public static float rightOriginY = 0f;
    public static float leftOriginX = 0;
    public static float leftOriginY = 0;
    public static float singleOriginX = 0;
    public static float singleOriginY = 0;
    public static float moveX = 0;
    public static float moveY = 0;
    public static float moveZ = 0;
    public static float side = 0;
    public static float singleX = 0;
    public static float singleY = 0;
    public static Touch touchRight;
    public static Touch touchLeft;
    public static Touch touchSingle;


    
void Start(){
    screenWidth = (Screen.height>Screen.width)? 
        screenWidth = Screen.height/2:Screen.width/2;
}

void singleTouch(){
    
    touchSingle = Input.GetTouch(0);

        if (singleReposition) {
        singleOriginX = touchSingle.position.x;
        singleOriginY = touchSingle.position.y;
        }

    if (touchSingle.phase == TouchPhase.Stationary || touchSingle.phase == TouchPhase.Moved) {
        singleX = touchSingle.deltaPosition.x/20; 
        singleY = touchSingle.deltaPosition.y/100; 
    }
    
    moveX = 0;
    moveY = 0;
    moveZ = 0;
    side = 0;
    singleReposition = false;
    doubleReposition = true; 
}

void doubleTouch(){
    
    touchRight = Input.GetTouch(0);
    touchLeft = Input.GetTouch(1);
    touchRight = 
        (touchRight.position.x>screenWidth) ? 
            touchRight:touchLeft;
    touchLeft = 
        (touchLeft.position.x<screenWidth) ? 
            touchLeft:touchRight;
    bool leftBool = 
            touchRight.phase == TouchPhase.Stationary 
                || touchRight.phase == TouchPhase.Moved;
    bool rightBool = 
            touchLeft.phase == TouchPhase.Stationary 
                || touchLeft.phase == TouchPhase.Moved;
                
    if (doubleReposition) {
        rightOriginX = touchRight.position.x;
        rightOriginY = touchRight.position.y;
        leftOriginY = touchLeft.position.y;
        leftOriginX = touchLeft.position.x;
        }

    if (leftBool || rightBool) {
        moveX = touchRight.deltaPosition.x;
        moveY = touchRight.deltaPosition.y;
        if (RenderScript.mobility) cinemachineCam.m_XAxis.Value += (touchRight.position.x - rightOriginX)/200;
        if (RenderScript.mobility) cinemachineCam.m_YAxis.Value -= (touchRight.position.y - rightOriginY)/20000; 
        moveZ = touchLeft.deltaPosition.y;
        side = touchLeft.deltaPosition.x;
        if (RenderScript.mobility) 
            player.transform.position 
                += cam.transform.forward*((touchLeft.position.y - leftOriginY)/6000);
        if (RenderScript.mobility) 
            player.transform.position 
                += cam.transform.right*((touchLeft.position.x - leftOriginX)/6000); 
        }

    singleX = 0;
    singleY = 0;
    singleReposition = true;
    doubleReposition = false;
}

    void Update(){
        touchCount = (Input.touchCount>0) ? Input.touchCount:0;
        if (touchCount>1) doubleTouch(); 
            else if (touchCount>0) singleTouch(); else {
                    moveX = 0;
                    moveY = 0;
                    moveZ = 0;
                    side = 0;
                    singleX = 0;
                    singleY = 0;
                    singleReposition = true;
                    doubleReposition = true;
            }
    }
}
