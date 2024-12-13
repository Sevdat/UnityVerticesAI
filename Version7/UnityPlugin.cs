using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;
using System;
using System.Linq;

public class VertexVisualizer : MonoBehaviour
{
    public GameObject fbx;
    public SceneBuilder sceneBuilder;
    public class SceneBuilder:SourceCode{
        public Body body;
        public int decreasePoints = 5;
        List<GameObject> allChildrenInParent(GameObject topParent){
            List<GameObject> allChildren = new List<GameObject>();
            for (int i = 0; i < topParent.transform.childCount; i++){
                allChildren.Add(topParent.transform.GetChild(i).gameObject);
            }
            return allChildren;
        }
        class AssembleJoints{
            public int jointIndex;
            public List<BakedMeshIndex> bakedMeshIndex;
            public List<GameObject> futureConnections; // do comments

            public AssembleJoints(int jointIndex,List<GameObject> futureConnections){
                this.jointIndex = jointIndex;
                bakedMeshIndex = new List<BakedMeshIndex>();
                this.futureConnections = futureConnections;
            }

        }
        public void loadModelToBody(GameObject topParent){
            List<BakedMesh> bakedMeshes = new List<BakedMesh>();
            Dictionary<GameObject,AssembleJoints> dictionary = new Dictionary<GameObject,AssembleJoints>();
            List<GameObject> tree = new List<GameObject>(){topParent};
            int jointIndex = 0;
            for (int i = 0; i < tree.Count; i++){
                GameObject root = tree[i];
                List<GameObject> futureConnections = allChildrenInParent(root);
                tree.AddRange(futureConnections);
                SkinnedMeshRenderer skin = root.GetComponent<SkinnedMeshRenderer>();
                if (!dictionary.ContainsKey(root)) dictionary[root] = new AssembleJoints(jointIndex,futureConnections);
                if (skin) bakedMeshes.Add(new BakedMesh(skin));

                jointIndex++;
            }
            body = new Body(0);
            body.bakedMeshes = bakedMeshes;
            body.arraySizeManager(dictionary.Count);
            for (int i = 0; i<bakedMeshes.Count; i++){
                BakedMesh bakedMesh = bakedMeshes[i];
                for (int j = 0; j < bakedMesh.vertices.Length; j++){
                    dictionary[bakedMesh.getGameObject(j)].bakedMeshIndex.Add(new BakedMeshIndex(i,j));
                }
            }
            foreach (GameObject gameObject in dictionary.Keys){
                AssembleJoints assembleJoints = dictionary[gameObject];
                int indexInBody = assembleJoints.jointIndex;
                Joint joint = new Joint(body,indexInBody,gameObject);
                joint.localAxis.placeAxis(gameObject.transform.position);
                joint.localAxis.alignRotationTo(gameObject, out float angle, out Vector3 axis, out Vector4 quat);
                joint.localAxis.rotate(quat,gameObject.transform.position);
                int pointCloudSize = assembleJoints.bakedMeshIndex.Count;
                joint.pointCloud = new PointCloud(joint,pointCloudSize);
                for (int i = 0;i < pointCloudSize;i++){
                    if (i%decreasePoints == 0){
                    CollisionSphere collisionSphere = new CollisionSphere(joint,i,assembleJoints.bakedMeshIndex[i]);
                    collisionSphere.bakedMeshIndex = assembleJoints.bakedMeshIndex[i];
                    collisionSphere.bakedMeshIndex.updatePoint();
                    joint.pointCloud.collisionSpheres[i] = collisionSphere;
                    }
                }
                body.bodyStructure[indexInBody] = joint;
            }
            foreach (GameObject gameObject in dictionary.Keys){
                AssembleJoints assembleJoints = dictionary[gameObject];
                Joint joint = body.bodyStructure[assembleJoints.jointIndex];
                List<GameObject> list = assembleJoints.futureConnections;
                for (int i = 0; i<list.Count;i++){
                    int index = dictionary[list[i]].jointIndex;
                    Joint future = body.bodyStructure[index];
                    joint.connection.connectThisFutureToPast(future,out _, out _);
                }
            }

        }
    }
    public class TravelInFilesAndFolders{
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string[] contents;
        public void deleteLast(){
            string[] p = path.Split("/");
            if (p.Length>1){
                path = "";
                for (int i = 0;i<p.Length;i++){
                    addLast(p[i]);
                }
            }
        }
        public void addLast(string str){
            path += str;
        }
        public string[] getFolders(){
            return Directory.GetDirectories(path);
        }
        public string[] getFiles(){
            return Directory.GetFiles(path);
        }
    }
    void Start()
    {

        // string appDataPath = System.Environment.GetEnvironmentVariable("LOCALAPPDATA");
        
        // string firefoxPath = System.IO.Path.Combine(appDataPath, @"Mozilla Firefox\firefox.exe");
        // // Path to the Firefox executable

        // // URL or file you want to open
        // string url = "https://www.google.com";
        // Process.Start(firefoxPath, url);
    }
    // void Start() {
    //     sceneBuilder= new SceneBuilder();
    //     sceneBuilder.loadModelToBody(fbx);
    // }
    // void LateUpdate() {
    //     sceneBuilder.body.updatePhysics();
    // }
    // void Start(){
    //     SkinnedMeshRenderer skinnedMeshRenderer = fbx.GetComponent<SkinnedMeshRenderer>();
    //     Mesh mesh = skinnedMeshRenderer.sharedMesh;
    //     Vector3[] vertices = mesh.vertices;
    //     BoneWeight[] boneWeights = mesh.boneWeights; 
    //     Transform[] bones = skinnedMeshRenderer.bones;

    //     for (int i = 0; i < vertices.Length; i++)
    //     {
    //         Vector3 localVertex = vertices[i];
    //         Vector3 worldPosition = skinnedMeshRenderer.transform.TransformPoint(localVertex);
    //         BoneWeight boneWeight = boneWeights[i];
    //         GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //         cube.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    //         cube.transform.position = worldPosition;
    //         Debug.Log($"Vertex {i} {worldPosition}");
    //         if (boneWeight.weight0 > 0)
    //         {
    //             Debug.Log($"Bone 0 {bones[boneWeight.boneIndex0].name}");
    //         }
    //         if (boneWeight.weight1 > 0)
    //         {
    //             Debug.Log($"Bone 1 {bones[boneWeight.boneIndex1].name}");
    //         }
    //         if (boneWeight.weight2 > 0)
    //         {
    //             Debug.Log($"Bone 2 {bones[boneWeight.boneIndex2].name}");
    //         }
    //         if (boneWeight.weight3 > 0)
    //         {
    //             Debug.Log($"Bone 3 {bones[boneWeight.boneIndex3].name}");
    //         }
    //     }
    // }
    //--------------------------------------
    // private GameObject[] cubes;
    // private Mesh bakedMesh;
    // SkinnedMeshRenderer skin;
    // void Start() {
    //     skin = fbx.GetComponent<SkinnedMeshRenderer>();
    //     Mesh mesh = skin.sharedMesh;
    //     Vector3[] vertices = mesh.vertices;

    //     cubes = new GameObject[vertices.Length];
    //     bakedMesh = new Mesh();

    //     for (int i = 0; i < vertices.Length; i++){
    //         GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //         cube.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    //         cubes[i] = cube;
    //     }
    // }

    // void LateUpdate() {
    //     skin.BakeMesh(bakedMesh); // Bake the current state of the skinned mesh
    //     Vector3[] vertices = bakedMesh.vertices;

    //     for (int i = 0; i < vertices.Length; i++){
    //         Vector3 worldPosition = skin.transform.TransformPoint(vertices[i]);
    //         cubes[i].transform.position = worldPosition;
    //     }
    // }
}