using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3[] chest = new Vector3[10];
    void Start(){
        for (int i = 0; i <chest.Length;i++){
            int x = WorldBuilder.dimensionX/2;
            int y = (WorldBuilder.dimensionY/2)+i;
            int z = WorldBuilder.dimensionZ/2;
            int[] a = WorldBuilder.boundry(x,y,z);
            x = a[0];
            y = a[1];
            z = a[2];
            Vector3 lol2 = new Vector3(x,y,z);
            int lol = WorldBuilder.vecToInt(x,y,z);
            WorldBuilder.cloneCreator(lol,lol2,true);
        }
    }

    // Update is called once per frame
    void Update(){
        
    }
}
