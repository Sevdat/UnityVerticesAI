using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreePointEnviroment : MonoBehaviour
{
    public GameObject originalObject;
    public static GameObject dynamicClone;
    public static GameObject staticClone;
    public static GameObject cloneHierarchy;
    public Dictionary<Vector3Int,AtomicInfo> atomicGrid;
    public Dictionary<string,AtomicInfo> body;

    public class AtomicInfo{
        public List<BodyInfo> bodyList;
        public int oldSize,newSize,minIndexChange;
    }
    public struct BodyInfo{
        string fullPath;
        public int indexInList;
        public Vector3 oldPosition;
        public Vector3 newPosition;
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
