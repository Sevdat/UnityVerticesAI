using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Movement : MonoBehaviour
{
        // Start is called before the first frame update
    public GameObject player;
    public Camera cam;
    public CinemachineFreeLook cinemachineCam;

    // Update is called once per frame
    float xLimit = 3.3f;
    float yLimit = 0.3f;
    bool reposition = true;
    float rightOriginX = 0f;
    float rightOriginY = 0f;
    float leftOriginX = 0;
    float leftOriginY = 0;
    public static float moveX = 0;
    public static float moveY = 0;
    public static float moveZ = 0;
    public static float side = 0;
    public static float singleX = 0;
    public static float singleY = 0;
    float screenWidth = 0;
    Touch touchRight;
    Touch touchLeft;
    Touch touchSingle;


    
void Start(){
    screenWidth = (Screen.height>Screen.width)? 
        screenWidth = Screen.height/2:Screen.width/2;
}

void singleTouch(){
    
    touchSingle = Input.GetTouch(0);
    if (touchSingle.phase == TouchPhase.Stationary || touchSingle.phase == TouchPhase.Moved) {
        singleX = touchSingle.deltaPosition.x/20; 
        singleY = touchSingle.deltaPosition.y/100; 
    }
    
    moveX = 0;
    moveY = 0;
    moveZ = 0;
    side = 0;
    reposition = true; 
}

void doubleTouch(){
    
    touchRight = Input.GetTouch(0);
    touchLeft = Input.GetTouch(1);
    touchRight = 
        (touchRight.position.x>screenWidth) ? 
            Input.GetTouch(0):Input.GetTouch(1);
    touchLeft = 
        (touchLeft.position.x<screenWidth) ? 
            Input.GetTouch(1):Input.GetTouch(0);
    bool leftBool = 
            touchRight.phase == TouchPhase.Stationary 
                || touchRight.phase == TouchPhase.Moved;
    bool rightBool = 
            touchLeft.phase == TouchPhase.Stationary 
                || touchLeft.phase == TouchPhase.Moved;
                
    if (reposition) {
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
    reposition = false;
}

    void Update(){
        if (Input.touchCount>1) doubleTouch(); 
            else if (Input.touchCount>0) singleTouch(); else {
                    moveX = 0;
                    moveY = 0;
                    moveZ = 0;
                    side = 0;
                    singleX = 0;
                    singleY = 0;
                    reposition = true;
            }
    }
}
