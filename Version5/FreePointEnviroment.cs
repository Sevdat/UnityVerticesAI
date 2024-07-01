using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreePointEnviroment : MonoBehaviour
{
    public GameObject originalObject;
    public static GameObject dynamicClone;
    public static GameObject staticClone;
    public static GameObject cloneHierarchy;

    public class World {
        public Dictionary<Vector3Int,AtomicGrid> world;
        public int worldSize,worldSizeLimit;
            public class AtomicGrid {
                public List<Joint> bodyList;
                public int minIndexChange;
            }
    }

    public class BodiesInWorld {
        public Dictionary<string,Body> bodies;
        public int bodyCount,bodyCountLimit;
        public struct Body {
            public List<Joint> jointList;
            public int jointCount,jointCountLimit;
        }
    }
    public struct Joint{
        public string name,pathFromZero;
        public int indexInWorldList,indexInJointList,connectedTo;
        public Vector3 from,to;
        public Vector3 x,y,z;
        public List<Vector3> meshShape;
    }
    
    public static void createObject(string name,Vector3 vec){
        GameObject clone = Instantiate(
            staticClone, cloneHierarchy.transform
            );
        clone.name = $"{name}";
        clone.transform.position = vec;   
    }
    void Start()
    {
        cloneHierarchy = new GameObject(){
                name = "cloneHierarchy",
                isStatic = true
            };
        dynamicClone = originalObject;
        staticClone = originalObject;
        staticClone.isStatic = true;
        createObject("lol",new Vector3(10,10,10));
        // interpolate bewteewn skeleton joint points
    }

    // Update is called once per frame
    void Update()
    {

                    
    }
}
