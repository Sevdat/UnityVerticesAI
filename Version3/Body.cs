using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Body : MonoBehaviour
{
    // Start is called before the first frame update
    static Vector3[] chest = new Vector3[20];
    class lol:WorldBuilder{
        public static void loli(){
            int x = dimensionX/2;
            int y = dimensionY/2;
            int z = dimensionZ/2;
            for (int i = 0; i <chest.Length;i++){
                x = boundry(x,1,dimensionX);
                y = boundry(y,1,dimensionY);
                z = boundry(z,1,dimensionX);
                int lol = vecToInt(x,y,z);
                Vector3 lol2 = new Vector3(x,y,z);
                cloneCreator(lol,lol2,true);
            }
        }
    }
    void Start(){
        lol.loli();
    }

    // Update is called once per frame
    void Update(){
        
    }
}
