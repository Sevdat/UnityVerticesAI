using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballVisible : MonoBehaviour
{
    // Start is called before the first frame update
    int index;
    int value;
    bool checkOnce = true;
    bool success;
    void Start()
    { 
        success = int.TryParse(gameObject.name, out index);
        if (success)
        transform.position = WorldBuilder.worldDimensions[index];
    }
    // Update is called once per frame
    void Update()
    {
        value = WorldBuilder.ballLocations[index];
        if (success && checkOnce && value == 49) {
            GetComponent<MeshFilter>().mesh = WorldBuilder.ballMesh;
            checkOnce = false;
        } else if (success && !checkOnce && value == 48) {
            GetComponent<MeshFilter>().mesh.Clear();
            checkOnce = true;
            }
    }
}
