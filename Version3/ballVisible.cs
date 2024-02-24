DONT USE THIS METHOD. WHEN THE NUMBERS EXCEED 10 EACH OF THEM WILL CALL
AND CRASH UNITY. IT WAS A NICE ATTEMPT THOUGH))

// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class ballVisible : MonoBehaviour
// {
//     // Start is called before the first frame update
//     int index;
//     bool value;
//     bool checkOnce = true;
//     bool success;
//     void Start()
//     { 
//         success = int.TryParse(gameObject.name, out index);
//         if (success){
//         transform.position = WorldBuilder.worldDimensions[index];
//         value = WorldBuilder.bitArray[index];
//         }
//         if (success && !value) 
//             GetComponent<MeshFilter>().mesh.Clear();
            
//     }
//     // Update is called once per frame
//     void Update()
//     {
//         if (success) value = WorldBuilder.bitArray[index];
//         if (success && checkOnce && value) {
//             GetComponent<MeshFilter>().mesh = WorldBuilder.ballMesh;
//             checkOnce = false;
//         } else if (success && !checkOnce && !value) {
//             GetComponent<MeshFilter>().mesh.Clear();
//             checkOnce = true;
//             }
//     }
// }
