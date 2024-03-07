using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Body : MonoBehaviour
{
    // Start is called before the first frame update
    public static Vector3Int[] chest = new Vector3Int[1]{
         new Vector3Int(4,4,4)
         };
    void Start(){
        WorldBuilder.move(chest[0],-1,-1,6);
    }

    // Update is called once per frame
    float time = 0;
    void Update(){
        time += Time.deltaTime;
        if (time > 5f){
            print(time);
            time = 0;
        }
    }
}
