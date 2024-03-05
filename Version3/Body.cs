using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3[] chest;
    void Start(){
        WorldBuilder.vecToInt(
            WorldBuilder.dimensionX/2,
            WorldBuilder.dimensionX/2,
            WorldBuilder.dimensionZ/2
            );
    }

    // Update is called once per frame
    void Update(){
        
    }
}
