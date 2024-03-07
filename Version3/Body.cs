using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Body : MonoBehaviour
{
    // Start is called before the first frame update
    public static int[] chest = new int[3]{4,4,4};
    void Start(){
        WorldBuilder.move(chest,0,-1,-1,5);
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
