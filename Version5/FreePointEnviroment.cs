using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreePointEnviroment : MonoBehaviour
{
    public GameObject originalObject;
    public static GameObject dynamicClone;
    public static GameObject staticClone;
    public static GameObject cloneHierarchy;
    // Start is called before the first frame update

    public struct Body{
        public Dictionary<Vector3Int,List<bodyInfo>> pathDictionary;
    }
    public struct bodyInfo{
        public Path pathData;
        public CollisionPath collisionData;
    }
    public struct Path{
        public string fullPath,lastPath,cloneNum;
    }
    public Path path(string fullPath,string lastPath,string cloneNum){
        return new Path(){
            fullPath = fullPath,
            lastPath = lastPath,
            cloneNum = cloneNum
        };
    }
    public struct CollisionPath{
        public Path fromPath;
        public Path toPath;
        public CollisionPath(Path fromPath,Path toPath){
            this.fromPath = fromPath;
            this.toPath = toPath;
        }
    }
    public CollisionPath collisionPath(Path fromPath,Path toPath){
        return new CollisionPath(){
            fromPath = fromPath,
            toPath = toPath,
        };
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
