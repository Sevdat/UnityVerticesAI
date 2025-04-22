using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class VertexVisualizer : MonoBehaviour
{
    public GameObject fbx;
    public static GameObject staticTerminal;
    public SceneBuilder sceneBuilder;

    public class BakedMesh {
        internal SkinnedMeshRenderer skinnedMeshRenderer;
        internal Mesh mesh;
        Transform transform;
        Transform[] bones;
        BoneWeight[] boneWeights; 
        public Vector3[] vertices;

        public BakedMesh(SkinnedMeshRenderer skinnedMeshRenderer){
            this.skinnedMeshRenderer=skinnedMeshRenderer;
            mesh = new Mesh(){
                vertices = new Vector3[skinnedMeshRenderer.sharedMesh.vertices.Length]
            };
            bakeMesh();
        }
        public void bakeMesh(){
            skinnedMeshRenderer.BakeMesh(mesh);
            vertices = mesh.vertices;
            bones = skinnedMeshRenderer.bones;
            boneWeights = skinnedMeshRenderer.sharedMesh.boneWeights;
            transform = skinnedMeshRenderer.transform;
        }
        public Vector3 worldPosition(int index){
            return transform.TransformPoint(vertices[index]);
        }
        public GameObject getGameObject(int index){
            BoneWeight boneWeight = boneWeights[index];
            return bones[boneWeight.boneIndex0].gameObject;
        }
    }
    public class AxisData{
        public Transform transform;
        public int jointIndex;
        public Vector4 quat;
    
        public AxisData(){}
        public AxisData(Transform transform,int jointIndex){
            this.transform = transform;
            this.jointIndex = jointIndex;
            quat = getQuat();
        }
        public Vector3 getPosition(){
            return transform.position;
        }
        public Vector4 getQuat(){
            return new Vector4(
                transform.rotation.x,
                transform.rotation.y,
                transform.rotation.z,
                transform.rotation.w
            );
        }
    }
    
    public class SceneBuilder:SourceCode{
        public Body body;
        internal List<BakedMesh> bakedMeshes = new List<BakedMesh>();
        internal AxisData globalAxis;
        internal AxisData[] localAxis;
        GameObject processedFBX;
        Mesh mesh;
        MeshFilter meshFilter;

        public SceneBuilder(GameObject fbxGameObject){ 
            processedFBX = new GameObject(fbxGameObject.name);
            loadModelToBody(fbxGameObject);
            body.sendToGPU.init();
            mesh = new Mesh();
            meshFilter = processedFBX.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            MeshRenderer meshRenderer = processedFBX.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Standard"));
            processedFBX.transform.position = processedFBX.transform.position;
            drawMesh(body.sendToGPU.vertices,body.sendToGPU.triangles);
        }

        class AssembleJoints {
            public int jointIndex;
            public List<BakedMeshIndex> bakedMeshIndex;
            public List<GameObject> futureConnections;
            public List<List<int>> triangles;

            public AssembleJoints(int jointIndex,List<GameObject> futureConnections){
                this.jointIndex = jointIndex;
                bakedMeshIndex = new List<BakedMeshIndex>();
                this.futureConnections = futureConnections;
                triangles = new List<List<int>>();
            }
        }

        List<GameObject> allChildrenInParent(GameObject topParent){
            List<GameObject> allChildren = new List<GameObject>();
            for (int i = 0; i < topParent.transform.childCount; i++){
                allChildren.Add(topParent.transform.GetChild(i).gameObject);
            }
            return allChildren;
        }
        void createHierarchy(List<GameObject> tree,Dictionary<GameObject,AssembleJoints> dictionary,List<BakedMesh> bakedMeshes){
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
            List<MeshData> meshDatas = new List<MeshData>();
            foreach (BakedMesh bakedMesh in bakedMeshes){
                meshDatas.Add(new MeshData(bakedMesh.mesh.vertices,bakedMesh.mesh.triangles));
            }
            body.bakedMeshes = meshDatas;
            body.arraySizeManager(dictionary.Count);
        }
        void createMeshAccessForSpheres(List<BakedMesh> bakedMeshes,Dictionary<GameObject,AssembleJoints> dictionary){
            for (int i = 0; i<bakedMeshes.Count; i++){
                BakedMesh bakedMesh = bakedMeshes[i];
                for (int j = 0; j < bakedMesh.vertices.Length; j++){
                    dictionary[bakedMesh.getGameObject(j)].bakedMeshIndex.Add(new BakedMeshIndex(i,j));
                }
            }
        }
        void createTrianglesForPointClouds(List<BakedMesh> bakedMeshes,Dictionary<GameObject,AssembleJoints> dictionary){
            int count = 0;
            foreach (BakedMesh bakedMesh in bakedMeshes){
                int[] trianglesInMesh = bakedMesh.mesh.triangles;
                if (trianglesInMesh.Length>3){
                    for (int i = 0; i < trianglesInMesh.Length; i += 3){
                        int vertexIndex1 = trianglesInMesh[i];
                        int vertexIndex2 = trianglesInMesh[i + 1];
                        int vertexIndex3 = trianglesInMesh[i + 2];
                        GameObject gameObject1 = bakedMesh.getGameObject(vertexIndex1);
                        GameObject gameObject2 = bakedMesh.getGameObject(vertexIndex2);
                        GameObject gameObject3 = bakedMesh.getGameObject(vertexIndex3);
                        if (gameObject1 == gameObject2 && gameObject3 == gameObject2){
                            if (bakedMeshes.Count>dictionary[gameObject1].triangles.Count){
                                for (int j = 0;j<bakedMeshes.Count;j++){
                                    dictionary[gameObject1].triangles.Add(new List<int>());
                                }
                            }
                            List<int> trianglesForSphere = dictionary[gameObject1].triangles[count];
                            trianglesForSphere.Add(vertexIndex1);
                            trianglesForSphere.Add(vertexIndex2);
                            trianglesForSphere.Add(vertexIndex3); 
                        }
                    }
                }
                count++;
            }
        }
        void renewedKeysForTriangles(PointCloud pointCloud, List<List<int>> triangles){
            CollisionSphere[] collisionspheres = pointCloud.collisionSpheres;
            Dictionary<int, Dictionary<int,int>> dictionary = new Dictionary<int, Dictionary<int,int>>();
            int count = 0;
            foreach (CollisionSphere collisionSphere in collisionspheres){
                BakedMeshIndex bakedMesh = collisionSphere.bakedMeshIndex;
                if (!dictionary.ContainsKey(bakedMesh.indexInBakedMesh)) 
                    dictionary[bakedMesh.indexInBakedMesh] = new Dictionary<int,int>();
                
                if (!dictionary[bakedMesh.indexInBakedMesh].ContainsKey(bakedMesh.indexInVertex)){ 
                    dictionary[bakedMesh.indexInBakedMesh][bakedMesh.indexInVertex] = count;
                    count++;
                }
            }
            
            int size = 0;
            foreach (List<int> triangleList in triangles) size += triangleList.Count;
            pointCloud.triangles = new int[size];
            size = 0;
            for (int i = 0; i<triangles.Count;i++){
                List<int> triangleList = triangles[i];
                for (int j = 0; j<triangleList.Count;j++){
                    pointCloud.triangles[j+size] = dictionary[i][triangleList[j]];
                }
                size += triangleList.Count;
            }
        }
        void createPointCloud(Dictionary<GameObject,AssembleJoints> dictionary){
            foreach (GameObject gameObject in dictionary.Keys){
                AssembleJoints assembleJoints = dictionary[gameObject];
                int indexInBody = assembleJoints.jointIndex;
                Transform transform = gameObject.transform;
                Vector4 quat = new Vector4(
                    transform.rotation.x,
                    transform.rotation.y,
                    transform.rotation.z,
                    transform.rotation.w
                    );
                UnityAxis unityAxis = new UnityAxis(transform.position,quat);
                Joint joint = new Joint(body,indexInBody,unityAxis);
                joint.localAxis.placeAxis(gameObject.transform.position);
                joint.localAxis.rotate(quat,gameObject.transform.position);
                int pointCloudSize = assembleJoints.bakedMeshIndex.Count;
                joint.pointCloud = new PointCloud(joint);
                joint.pointCloud.collisionSpheres = new CollisionSphere[pointCloudSize];
                for (int i = 0;i < pointCloudSize;i++){
                    CollisionSphere collisionSphere = new CollisionSphere(joint,i,assembleJoints.bakedMeshIndex[i]);
                    collisionSphere.bakedMeshIndex = assembleJoints.bakedMeshIndex[i];
                    collisionSphere.bakedMeshIndex.setPoint();
                    joint.pointCloud.collisionSpheres[i] = collisionSphere;
                }
                renewedKeysForTriangles(joint.pointCloud,assembleJoints.triangles);
                body.bodyStructure[indexInBody] = joint;
            }
        }
        void createConnections(Dictionary<GameObject,AssembleJoints> dictionary){
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
        public void loadModelToBody(GameObject topParent){
            Vector4 quat = new Vector4(
                topParent.transform.rotation.x,
                topParent.transform.rotation.y,
                topParent.transform.rotation.z,
                topParent.transform.rotation.w
                );
            UnityAxis globalAxis = new UnityAxis(topParent.transform.position,quat);
            body = new Body(0, globalAxis);
            body.globalAxis.placeAxis(globalAxis.origin);
            Dictionary<GameObject,AssembleJoints> dictionary = new Dictionary<GameObject,AssembleJoints>();
            List<GameObject> tree = new List<GameObject>(){topParent};
            createHierarchy(tree,dictionary,bakedMeshes);
            createMeshAccessForSpheres(bakedMeshes,dictionary);
            createTrianglesForPointClouds(bakedMeshes,dictionary);
            createPointCloud(dictionary);
            createConnections(dictionary);
            int size = dictionary.Count;
            localAxis = new AxisData[size];

            foreach (GameObject gameObject in dictionary.Keys){
                AssembleJoints assembleJoints = dictionary[gameObject];
                int index = assembleJoints.jointIndex;
                Transform transform = gameObject.transform;
                localAxis[index] = new AxisData(transform,index);
            }
        }
        public void updateMeshData(){
            List<MeshData> meshDatas = new List<MeshData>();
            foreach (BakedMesh bakedMesh in bakedMeshes){
                bakedMesh.bakeMesh();
                meshDatas.Add(new MeshData(bakedMesh.mesh.vertices,bakedMesh.mesh.triangles));
            }
            body.bakedMeshes = meshDatas;
        }
        void drawMesh(Vector3[] vertices,int[] triangles){
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.UploadMeshData(false);
        }
        public void updateBodyPositions(){
            for (int i = 0; i<localAxis.Length;i++){
                int index = localAxis[i].jointIndex;
                Vector3 origin = localAxis[i].getPosition();
                Vector4 quat = localAxis[i].getQuat();
                UnityAxis unityAxis = body.bodyStructure[index].unityAxis;
                unityAxis.origin = origin;
                unityAxis.quat = quat;
            }
        }
        public void updateUnityData(){
            updateMeshData();
            updateBodyPositions();
        }
        public void updateBody(){
            body.updatePhysics();
            body.sendToGPU.updateArray();
        }
        public void drawBody(){
            drawMesh(body.sendToGPU.vertices,body.sendToGPU.triangles);
        }
    }

    public class Terminal{
        GameObject terminal;
        PathScript path;
        SpecialFolderScript specialFolder;
        FolderScript folders;
        FileScript files;
        public Terminal(){
            terminal = Resources.Load("Terminal") as GameObject;
            terminal = Instantiate(terminal);
            path = getTerminalWindow(0).GetComponent<PathScript>();
            specialFolder = getTerminalWindow(1).GetComponent<SpecialFolderScript>();
            path.specialFolder = specialFolder;
            specialFolder.pathScript = path;

            folders = getTerminalWindow(2).GetComponent<FolderScript>();
            path.folders = folders;
            folders.pathScript = path;

            files = getTerminalWindow(3).GetComponent<FileScript>();
            path.files = files;
            files.pathScript = path;

            specialFolder.specialFolderPaths();
        }
        Transform getTerminalWindow(int index){
            return terminal.transform.GetChild(index).GetChild(0).GetChild(0);
        }
    }
    // Terminal terminal;
    // void Start(){
    //     terminal = new Terminal();
        
    //     // print(terminal.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<ItemButton>());
    //     // string appDataPath = System.Environment.GetEnvironmentVariable("LOCALAPPDATA");
        
    //     // string firefoxPath = System.IO.Path.Combine(appDataPath, @"Mozilla Firefox\firefox.exe");
    //     // // Path to the Firefox executable

    //     // // URL or file you want to open
    //     // string url = "https://www.google.com";
    //     // Process.Start(firefoxPath, url);
    // }
    MultiThread multiThread;
    static ConcurrentQueue<MultiThread> resultQueue = new ConcurrentQueue<MultiThread>();

    public class MultiThread {
        public bool isRunning, updateBody;
        public SceneBuilder sceneBuilder;
        public MultiThread(SceneBuilder sceneBuilder){
            this.sceneBuilder = sceneBuilder;
            isRunning = true;
            updateBody = true;
            runBackgroundTask();
        }
        public async Task runBackgroundTask(){
            while (isRunning){
                if (updateBody){
                    await Task.Run(() =>{
                        sceneBuilder.updateBody();
                    });
                    stopUpdate();
                }
                await Task.Delay(10); // Simulate delay
            }
        }
        public void wait(){
            isRunning = false;
        }
        public void StopTask(){
            isRunning = false;
        } 
        public void updateUnityData(){
            sceneBuilder.drawBody();
            sceneBuilder.updateUnityData();
            updateBody = true;
        }
        public void stopUpdate(){
            updateBody = false;
            resultQueue.Enqueue(this);
            print(resultQueue.Count);
        }
    }

    void OnDestroy(){
        multiThread?.StopTask();
    }

    void OnApplicationQuit(){
        multiThread?.StopTask();
    }

    // void Start(){
    //     sceneBuilder = new SceneBuilder(fbx);
    //     multiThread = new MultiThread(sceneBuilder);
    // }
    // void LateUpdate() {
    //     DateTime old = DateTime.Now;
    //     while (resultQueue.TryDequeue(out MultiThread result)){
    //         result.updateUnityData();
    //     }
    //     print(DateTime.Now - old);
    // }

    // long memoryBefore;
    // void Start() {
    //     sceneBuilder = new SceneBuilder(fbx);
    //     memoryBefore = Process.GetCurrentProcess().WorkingSet64;

    //     // sceneBuilder.body.bakedMeshes = null; 
    // }
    // void LateUpdate() {
    //     DateTime old = DateTime.Now;
    //     sceneBuilder.updateBodyPositions();
    //     sceneBuilder.updateUnityData();
    //     sceneBuilder.updateBody();
    //     sceneBuilder.drawBody();
    //     // print(DateTime.Now - old);
    // }

    // int count = 0;
    // int time = 0;
    // public void readTextFiles(){
    //     if (time > 1){
    //         print(count);
    //         sceneBuilder.body.editor.reader(count);
    //         count++;
    //         if (count>11) count = 0;
    //         time = 0;
    //     } else time++;
    // }

}