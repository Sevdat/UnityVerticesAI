using System.Collections.Generic;
using UnityEngine;

public class VertexVisualizer : MonoBehaviour
{
    public GameObject fbx;
    public SceneBuilder sceneBuilder;
    public class SceneBuilder:SourceCode{
        public Body body;
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
            public List<Joint> future; // do comments

            public AssembleJoints(int jointIndex){
                this.jointIndex = jointIndex;
                bakedMeshIndex = new List<BakedMeshIndex>();
            }

        }
        public void loadModelToBody(GameObject topParent){
            List<BakedMesh> bakedMeshes = new List<BakedMesh>();
            Dictionary<GameObject,AssembleJoints> dictionary = new Dictionary<GameObject,AssembleJoints>();
            List<GameObject> tree = new List<GameObject>(){topParent};
            int jointIndex = 0;
            for (int i = 0; i < tree.Count; i++){
                GameObject root = tree[i];
                tree.AddRange(allChildrenInParent(root));
                SkinnedMeshRenderer skin = root.GetComponent<SkinnedMeshRenderer>();
                if (!dictionary.ContainsKey(root)) dictionary[root] = new AssembleJoints(jointIndex);
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
                joint.localAxis.alignRotationTo(gameObject, out _, out _, out _);
                int pointCloudSize = assembleJoints.bakedMeshIndex.Count;
                joint.pointCloud = new PointCloud(joint,pointCloudSize);
                for (int i = 0;i < pointCloudSize;i++){
                    CollisionSphere collisionSphere = new CollisionSphere(joint,i,assembleJoints.bakedMeshIndex[i]);
                    collisionSphere.bakedMeshIndex = assembleJoints.bakedMeshIndex[i];
                    collisionSphere.bakedMeshIndex.updatePoint();
                    joint.pointCloud.collisionSpheres[i] = collisionSphere;
                }
                body.bodyStructure[indexInBody] = joint;
            }
        }
    }
    void Start() {
        sceneBuilder= new SceneBuilder();
        sceneBuilder.loadModelToBody(fbx);
    }
    void LateUpdate() {
        sceneBuilder.body.updatePhysics();
    }
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