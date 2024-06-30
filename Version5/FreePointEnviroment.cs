using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreePointEnviroment : MonoBehaviour
{
    public GameObject originalObject;
    public static GameObject dynamicClone;
    public static GameObject staticClone;
    public static GameObject cloneHierarchy;

    public Dictionary<Vector3Int,AtomicGrid> world;
    public int dictionarySize;
    public int dictionaryLimit;

    public class AtomicGrid{
        public Vector3Int worldPosition;
        public List<Joint> bodyList;
        public int oldSize,newSize,minIndexChange;
    }

    public struct Body{
        public List<Joint> jointList;
    }
    public struct Joint{
        public string fullPath;
        public int indexInWorldList;
        public Vector3 from,to;
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
