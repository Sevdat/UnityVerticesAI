using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreePointEnviroment : MonoBehaviour
{
    public GameObject originalObject;
    public static GameObject dynamicClone;
    public static GameObject staticClone;
    public static GameObject cloneHierarchy;
    public World world;

    public class World {
        public int vector3Count,vector3CountLimit;
        public Dictionary<string,List<Joint>> bodies;
        public struct Joint{
            public string name;
            public int indexInWorldList;
            public int indexInJointList,connectedTo;
            public Vector3 position;
            public Vector3 localX,localY,localZ;
            public List<Vector3> meshShape;
        }
        
        public Dictionary<Vector3Int,AtomicGrid> collisionDetection;
        public class AtomicGrid {
            public List<Joint> bodyList;
            public int minIndexChange;
        }
        public void body(string name,List<Joint>jointList){
            if (!bodies.ContainsKey(name) && vector3Count<vector3CountLimit){
                bodies[name] = jointList;
            }
        }
        public Joint joint(
                string name,
                int indexInWorldList,
                int indexInJointList,int connectedTo,
                Vector3 position,
                Vector3 localX,Vector3 localY,Vector3 localZ,
                List<Vector3> meshShape
                ){
                    return new Joint(){
                        name = name,
                        indexInWorldList = indexInWorldList,
                        indexInJointList = indexInJointList,
                        connectedTo = connectedTo,
                        position = position,
                        localX = localX,
                        localY = localY,
                        localZ = localZ,
                        meshShape = meshShape
                    };
        }
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
        world = new World();
    }

    // Update is called once per frame
    void Update()
    {

                    
    }
}
