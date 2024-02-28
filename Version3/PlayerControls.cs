using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    // Start is called before the first frame update
    void movement()
    {
        if (Input.GetKey("w")){
           transform.position += Camera.main.transform.forward/20f;
        }
        if (Input.GetKey("s")){
            transform.position += -Camera.main.transform.forward/20f;
        }
        if (Input.GetKey("d")){
            transform.position += Camera.main.transform.right/20f;
        }
        if (Input.GetKey("a")){
            transform.position += -Camera.main.transform.right/20f;
        }   
        if (Input.GetKey("e")){
            transform.position += Camera.main.transform.up/20f;
        }
        if (Input.GetKey("q")){
            transform.position += -Camera.main.transform.up/20f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!WorldBuilder.disablePlayerControls) movement();
        
    }
}
