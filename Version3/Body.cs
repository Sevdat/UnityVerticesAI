using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3[] chest = new Vector3[20];
    void Start(){
        int x = WorldBuilder.dimensionX/2;
        int y = WorldBuilder.dimensionY/2;
        int z = WorldBuilder.dimensionZ/2;
        for (int i = 0; i <chest.Length;i++){
            x = WorldBuilder.boundry(x,1,WorldBuilder.dimensionX);
            y = WorldBuilder.boundry(y,1,WorldBuilder.dimensionY);
            z = WorldBuilder.boundry(z,1,WorldBuilder.dimensionZ);
            int lol = WorldBuilder.vecToInt(x,y,z);
            Vector3 lol2 = new Vector3(x,y,z);
            WorldBuilder.cloneCreator(lol,lol2,true);
            y+=1;
            z+=1;
            x+=1;
        }
    }

    // Update is called once per frame
    void Update(){
        
    }
}
