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
    float currentPoint2X = 0f;
    float currentPoint2Y = 0f;
    float currentPointX = 0;
    float currentPointY = 0;
    public static float moveX = 0;
    public static float moveY = 0;
    public static float moveZ = 0;
    public static float side = 0;
    public static float singleX = 0;
    public static float singleY = 0;
    float screenWidth = 0;
    Touch touchLeft;
    Touch touchRight;
    Touch touchSingle;


    
void Start(){
    screenWidth = (Screen.height>Screen.width)? 
        screenWidth = Screen.height/2:Screen.width/2;
}

void singleTouch(){
    
    touchSingle = Input.GetTouch(0);
    if (touchSingle.phase == TouchPhase.Stationary || touchSingle.phase == TouchPhase.Moved) {
        singleX = touchSingle.deltaPosition.x/100; 
        singleY = touchSingle.deltaPosition.y/100; 
    }
    
    moveX = 0;
    moveY = 0;
    moveZ = 0;
    side = 0;
    reposition = false; 
}

void doubleTouch(){
    
    touchLeft = Input.GetTouch(0);
    touchRight = Input.GetTouch(1);
    touchLeft = 
        (touchLeft.position.x>screenWidth) ? 
            Input.GetTouch(0):Input.GetTouch(1);
    touchRight = 
        (touchRight.position.x<screenWidth) ? 
            Input.GetTouch(1):Input.GetTouch(0);
    bool leftBool = 
            touchLeft.phase == TouchPhase.Stationary 
                || touchLeft.phase == TouchPhase.Moved;
    bool rightBool = 
            touchRight.phase == TouchPhase.Stationary 
                || touchRight.phase == TouchPhase.Moved;
                
    if (reposition) {
        currentPointX = touchLeft.position.x;
        currentPointY = touchLeft.position.y;
        currentPoint2Y = touchRight.position.y;
        currentPoint2X = touchRight.position.x;
        }

    if (leftBool || rightBool) {
        moveX = touchLeft.deltaPosition.x/20;
        moveY = touchLeft.deltaPosition.y/100;
        if (RenderScript.mobility) cinemachineCam.m_XAxis.Value += moveX;
        if (RenderScript.mobility) cinemachineCam.m_YAxis.Value -= moveY/15; 

        moveZ = touchRight.deltaPosition.y/300;
        side = touchRight.deltaPosition.x/300;
        if (RenderScript.mobility) player.transform.position +=  cam.transform.forward*moveZ;
        if (RenderScript.mobility) player.transform.position +=  cam.transform.right*side;  
        }

    singleX = 0;
    singleY = 0;
    reposition = true;
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
                    reposition = false;
            }
    }
}
