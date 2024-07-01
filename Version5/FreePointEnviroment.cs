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
        public Dictionary<string,List<Joint>> allBodies;
        public struct Joint {
            public string name; 
            public int jointIndex;
            public int[] connectedToJointIndex;
            public Atom atomicJoint;
            public Vector3 x,y,z;
            public List<Atom> atomicStructure;
        }
        public struct Atom { 
            public int indexInList;
            public Vector3 position;
        }
        
        public Dictionary<Vector3Int,AtomicGrid> collisionDetection;
        public struct AtomicGrid {
            public Dictionary<string, JointIndex> atomsInGrid;
        }
        public struct JointIndex{
            public Dictionary<int,Atom> jointsInGrid;
        }
        public void addBody(string name,List<Joint>jointList){
            if (!allBodies.ContainsKey(name) && vector3Count<vector3CountLimit){
                allBodies[name] = jointList;
            }
        }
        // public Joint joint(
        //         string name,
        //         int indexInWorldList,
        //         int indexInJointList,int connectedTo,
        //         Vector3 position,
        //         Vector3 localX,Vector3 localY,Vector3 localZ,
        //         List<Vector3> meshShape
        //         ){
        //             return new Joint(){
        //                 name = name,
        //                 indexInWorldList = indexInWorldList,
        //                 indexInJointList = indexInJointList,
        //                 connectedTo = connectedTo,
        //                 position = position,
        //                 localX = localX,
        //                 localY = localY,
        //                 localZ = localZ,
        //                 meshShape = meshShape
        //             };
        // }
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
