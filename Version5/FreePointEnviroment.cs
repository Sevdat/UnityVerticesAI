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
    public List<Path> pathList;
    public List<CollisionPath> collisionList;
    }
    public struct Path{
        public string fullPath;
        public string lastPath;
    }
    public Path path(string fullPath,string lastPath){
        return new Path(){
            fullPath = fullPath,
            lastPath = lastPath
        };
    }
    public struct Collision{
        public string pathName;
        public string collidedFrom;
    }
    public Collision collision(string pathName,string collidedFrom){
        return new Collision(){
            pathName = pathName,
            collidedFrom = collidedFrom
        };
    }
    public struct CollisionPath{
        public Collision fromPath;
        public Collision toPath;
        public Vector3Int positionCoordinates;
        public CollisionPath(Collision fromPath,Collision toPath,Vector3Int positionCoordinates){
            this.fromPath= fromPath;
            this.toPath = toPath;
            this.positionCoordinates = positionCoordinates;
        }
    }
    public CollisionPath collisionDetection(Collision fromPath,Collision toPath,Vector3Int positionCoordinates){
        return new CollisionPath(){
            fromPath = fromPath,
            toPath = toPath,
            positionCoordinates = positionCoordinates
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
