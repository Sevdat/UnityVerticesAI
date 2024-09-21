using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceCode:MonoBehaviour {

    public class SphericalOctTree {
        public SphericalOctTree sphereOctTree;
        public int depth;
        public Vector3 origin;
        public List<CollisionSphere> root;
        public List<CollisionSphere> a,b,c,d;
        public List<CollisionSphere> e,f,g,h;
    }

    public class World {
        public Body[] bodiesInWorld;
        public SphericalOctTree sphereOctTree;
        public KeyGenerator keyGenerator;
    }

    public class Axis {
        public Vector3 origin,x,y,z,rotationAxis;
        public float distance;

        public Axis(){}
        public Axis(Vector3 origin, float distance){
            this.origin = origin;
            this.distance = (distance >0.1f)? distance:1f;
            x = origin + new Vector3(1f,0,0)*distance;
            y = origin + new Vector3(0,1f,0)*distance;
            z = origin + new Vector3(0,0,1f)*distance;
            rotationAxis = origin + new Vector3(0,1f,0)*distance;
        }
        
        public void moveAxis(Vector3 add){
            origin += add;
            x += add;
            y += add;
            z += add;
            rotationAxis += add;
        }
        public void placeAxis(Vector3 newOrigin){
            Vector3 newPosition = newOrigin-origin;
            moveAxis(newPosition);
        }
        public void scale(float newDistance){
            if (newDistance > 0.1f){
                distance = newDistance;
                x = origin + distanceFromOrign(x,origin);
                y = origin + distanceFromOrign(y,origin);
                z = origin + distanceFromOrign(z,origin);
                rotationAxis = origin + distanceFromOrign(rotationAxis,origin);
            }
        }
        public float length(Vector3 vectorDirections){
            float radius = Mathf.Sqrt(
                Mathf.Pow(vectorDirections.x,2.0f)+
                Mathf.Pow(vectorDirections.y,2.0f)+
                Mathf.Pow(vectorDirections.z,2.0f)
            );
            return radius;
        }
        public Vector3 direction(Vector3 point,Vector3 origin){ 
            Vector3 v = point-origin;
            float radius = length(v);
            if (radius == 0) v = point-this.origin;
            return v/length(v);
        }
        public Vector3 distanceFromOrign(Vector3 point,Vector3 origin){
            return direction(point,origin)*distance;
        }
        public Vector3 normalize(Vector3 vec){    
            float radius = length(vec);
            if (radius > 0){
                vec.x /= radius;
                vec.y /= radius;
                vec.z /= radius;
            }
            return vec;
        }
        float dot(Vector3 vec1,Vector3 vec2){
            return vec1.x*vec2.x+vec1.y*vec2.y+vec1.z*vec2.z;
        }
        float angleBetweenLines(Vector3 dir1,Vector3 dir2){
            float dotProduct = dot(dir1,dir2);
            float lengthProduct = length(dir1)*length(dir2);
            float check = (dotProduct>lengthProduct)? 1:dotProduct/lengthProduct;
            return Mathf.Acos(check);
        }
        Vector3 perpendicular(Vector3 lineOrigin, Vector3 lineDirection, Vector3 point){
            float amount = dot(point-lineOrigin,lineDirection);
            return lineOrigin+amount*lineDirection;
        }
        void rotateAxis(ref Vector3 x, ref Vector3 y,ref Vector3 z,Vector3 axis,float angle){
            Vector4 quat = angledAxis(angle,axis);
            x = rotate(x,quat);
            y = rotate(y,quat);
            z = rotate(z,quat);
        }
        void rotateLocalAxis(float worldAngleY,float worldAngleX,float localAngleY,Vector3 worldX,Vector3 worldY,ref Vector3 localX,ref Vector3 localY,ref Vector3 localZ){
            rotateAxis(ref localX,ref localY,ref localZ,worldX,worldAngleY);
            rotateAxis(ref localX,ref localY,ref localZ,worldY,worldAngleX);
            rotateAxis(ref localX,ref localY,ref localZ,localY,localAngleY);
        }
        public void setAxis(float worldAngleY,float worldAngleX,float localAngleY){
            Vector3 worldX = origin + new Vector3(distance,0,0);
            Vector3 worldY = origin + new Vector3(0,distance,0);
            
            Vector3 localX = origin + new Vector3(distance,0,0);
            Vector3 localY = origin + new Vector3(0,distance,0);
            Vector3 localZ = origin + new Vector3(0,0,distance);
            
            rotateLocalAxis(
                worldAngleY,worldAngleX,localAngleY,
                worldX,worldY,ref localX,ref localY,ref localZ
                );
            print(localY);

            x = localX; y = localY; z = localZ;
        }
        public float angleFromPerpendicularOrigin(Vector3 lineOrigin, Vector3 lineDirection, Vector3 point, Vector3 dir, Vector3 directionSide){
            Vector3 perpendicularOrigin = perpendicular(lineOrigin,lineDirection,point);
            Vector3 perpendicularDirection = direction(point,perpendicularOrigin);
            float angleSide = angleBetweenLines(dir,perpendicularDirection);
            return (angleSide>Mathf.PI/2)? 
                2*Mathf.PI-angleBetweenLines(directionSide,perpendicularDirection):
                angleBetweenLines(directionSide,perpendicularDirection);
        }
        public void findAngle(out float worldAngleY,out float worldAngleX,out float localAngleY){
            Vector3 worldX = origin + new Vector3(distance,0,0);
            Vector3 worldY = origin + new Vector3(0,distance,0);
            Vector3 worldZ = origin + new Vector3(0,0,distance);
            Vector3 worldDirX = direction(worldX,origin);
            Vector3 worldDirY = direction(worldY,origin);
            Vector3 worldDirZ = direction(worldZ,origin);
   
            Vector3 dirY = direction(y,origin);
            worldAngleY = angleBetweenLines(dirY,worldDirY);
            
            worldAngleX = angleFromPerpendicularOrigin(origin,worldDirY,y,worldDirX,worldDirZ);
            
            Vector3 localX = origin + new Vector3(distance,0,0);
            Vector3 localY = origin + new Vector3(0,distance,0);
            Vector3 localZ = origin + new Vector3(0,0,distance);
            float degrees = 180 / Mathf.PI;
            rotateLocalAxis(
                worldAngleY*degrees,worldAngleX*degrees,0,
                worldX,worldY,ref localX,ref localY,ref localZ
                );   
            Vector3 dirX = direction(x,origin);
            Vector3 dirLocalX = direction(localX,origin);
            localAngleY = angleBetweenLines(dirX,dirLocalX);
        }
        public void rotateRotationAxis(float addAngleX,float addAngleY){
            Vector4 rotX = angledAxis(addAngleX,x);
            Vector4 rotY = angledAxis(addAngleY,y);
            rotationAxis = rotate(rotationAxis,rotX);
            rotationAxis = rotate(rotationAxis,rotY);
        }
        public void getRotationAxisAngle(out float angleX, out float angleY){
            Vector3 dirX = direction(x,origin);
            Vector3 dirY = direction(y,origin);
            Vector3 dirZ = direction(z,origin);
            Vector3 dirH = direction(rotationAxis,origin);
            Vector3 perpendicularOrigin = perpendicular(origin,dirY,rotationAxis);

            Vector3 dirPerpOrg = direction(rotationAxis,perpendicularOrigin);
            float angleSide = angleBetweenLines(dirX,dirPerpOrg);

            angleX = angleBetweenLines(dirY,dirH);
            angleY = (angleSide>Mathf.PI/2)? 
                2*Mathf.PI-angleBetweenLines(dirZ,dirPerpOrg):
                angleBetweenLines(dirZ,dirPerpOrg);
        }
        
        public Vector4 quatMul(Vector4 q1, Vector4 q2) {
            float w = q1.w * q2.w - q1.x * q2.x - q1.y * q2.y - q1.z * q2.z;
            float x = q1.w * q2.x + q1.x * q2.w + q1.y * q2.z - q1.z * q2.y;
            float y = q1.w * q2.y - q1.x * q2.z + q1.y * q2.w + q1.z * q2.x;
            float z = q1.w * q2.z + q1.x * q2.y - q1.y * q2.x + q1.z * q2.w;
            return new Vector4(x, y, z, w);
        }
        public Vector4 angledAxis(float angle,Vector3 rotationAxis){
                Vector3 normilized = normalize(rotationAxis - origin); 
                float halfAngle = angle * 0.5f * (Mathf.PI/180.0f);
                float sinHalfAngle = Mathf.Sin(halfAngle);
                float w = Mathf.Cos(halfAngle);
                float x = normilized.x * sinHalfAngle;
                float y = normilized.y * sinHalfAngle;
                float z = normilized.z * sinHalfAngle;
                return new Vector4(x,y,z,w);
        }
        public Vector3 rotate(Vector3 point,Vector4 angledAxis){
            Vector3 pointDirection = point - origin;     
            Vector4 rotatingVector = new Vector4(pointDirection.x, pointDirection.y, pointDirection.z,0);
            Vector4 inverseQuat = new Vector4(-angledAxis.x,-angledAxis.y,-angledAxis.z,angledAxis.w);
            Vector4 rotatedQuaternion = quatMul(quatMul(angledAxis,rotatingVector), inverseQuat);
            return origin + new Vector3(rotatedQuaternion.x,rotatedQuaternion.y,rotatedQuaternion.z);
        }
    }

    public class KeyGenerator{
        public int maxKeys;
        public int availableKeys;
        public int increaseKeysBy;
        public List<int> freeKeys;

        public KeyGenerator(){}
        public KeyGenerator(int amountOfKeys){
            freeKeys = new List<int>();
            increaseKeysBy = amountOfKeys;
            generateKeys();
            maxKeys = amountOfKeys;
        }

        public void generateKeys(){
            for(int i = 0; i < increaseKeysBy; i++){
                freeKeys.Add(maxKeys+increaseKeysBy-i);
            }
            availableKeys += increaseKeysBy;
            maxKeys += increaseKeysBy;
        }
        public void setLimit(int newLimit){
            if(newLimit > 0){
                increaseKeysBy = newLimit;
            }
        }
        public int getKey(){
            int key = freeKeys[availableKeys];
            freeKeys.RemoveAt(availableKeys);
            availableKeys -= 1;
            return key;
        }
        public void returnKey(int key){
            freeKeys.Add(key);
            availableKeys +=1;
        }
        public void resetGenerator(int maxKeys){
            freeKeys.Clear();
            this.maxKeys = maxKeys;
            availableKeys = 0;
        }
    }

    public class Body {
        public World world;
        public int worldKey;
        public Axis globalAxis;
        public Joint[] bodyStructure;
        public KeyGenerator keyGenerator;

        public Body(){}
        public Body(int worldKey, Axis globalAxis, int amountOfJoints){
            this.worldKey = worldKey;
            this.globalAxis = globalAxis;
            bodyStructure = new Joint[amountOfJoints];
            keyGenerator = new KeyGenerator(amountOfJoints);
        }

        public void resizeJoints(int amount){
            int availableKeys = keyGenerator.availableKeys;
            int maxKeys = keyGenerator.maxKeys;
            int limitCheck = availableKeys + amount;
            if(limitCheck > maxKeys) {
                keyGenerator.setLimit(limitCheck - availableKeys);
                keyGenerator.generateKeys();
                int max = keyGenerator.maxKeys;
                int newSize = max + keyGenerator.increaseKeysBy;
                Joint[] newJointArray = new Joint[newSize];
                for (int i = 0; i<max; i++){
                    Joint joint = bodyStructure[i];
                    if (joint != null){
                        newJointArray[i] = joint;
                    }
                }
            }
        }
        public void returnJointKey(int key){
            Joint remove = bodyStructure[key];
            if(remove != null){
                keyGenerator.returnKey(key);
                bodyStructure[key] = null;
            }
        }
        public void optimizeBody(){
            Joint firstJoint = null;
            for (int i =0; i<keyGenerator.maxKeys; i++){
                Joint joint = bodyStructure[i];
                if (joint != null){
                    firstJoint = joint;
                    break;
                }
            }
            firstJoint.getPastConnections(
                out _, 
                out List<Joint> jointOrigin, 
                out _, out _, out _
                );
            if (jointOrigin.Count == 1){
                firstJoint = jointOrigin[0];
                firstJoint.getFutureConnections(
                out List<Joint> connectionTree, 
                out _, 
                out int treeSize, out _, out int smallestKey
                );
                Joint[] orginizedJoints = new Joint[treeSize];
                for (int i = 0; i < treeSize; i++){
                    Joint joint = connectionTree[i];
                    int newIndex = joint.connection.current - smallestKey;
                    joint.connection.current = newIndex;
                    orginizedJoints[newIndex] = joint;
                }
            bodyStructure = orginizedJoints;
            keyGenerator.resetGenerator(treeSize);
            }
        }
    }

    public class Connection {
        public int current;
        public List<Joint> past; 
        public List<Joint> future;

        public Connection(){}
        public Connection(int current, List<Joint> past,List<Joint> future){
            this.current = current;
            this.past = past;
            this.future = future;
        }
        public void setCurrent(int current){
            this.current = current;
        }
        public void setPast(List<Joint> past){
            this.past = past;
        }
        public void setFuture(List<Joint> future){
            this.future = future;
        }
    }

    public class Joint {
        public Body body;
        public Axis localAxis;
        public Connection connection;
        public CollisionSphere[] collisionSpheres;
        public KeyGenerator keyGenerator;

        public Joint(){}
        public Joint(int amountOfKeys, Axis localAxis,Connection connection){
            collisionSpheres = new CollisionSphere[amountOfKeys];
            keyGenerator = new KeyGenerator(amountOfKeys);
            this.localAxis = localAxis;
            this.connection = connection;
        }

        public void setBody(Body body){
            this.body=body;
        }
        public void getFutureConnections(
            out List<Joint> connectionTree, 
            out List<Joint> connectionEnd,
            out int treeSize,out int biggestKey,out int smallestKey
            ){
            bool futureOnly = true;
            connectionTracker(
                futureOnly,
                out List<Joint> tree, out List<Joint> end,
                out int size, out int biggest, out int smallest
                );
            connectionTree = tree;
            connectionEnd = end;
            treeSize = size;
            biggestKey = biggest;
            smallestKey = smallest;
        }
        public void getPastConnections(
            out List<Joint> connectionTree, 
            out List<Joint> connectionEnd,
            out int treeSize,out int biggestKey,out int smallestKey
            ){
            bool pastOnly = false;
            connectionTracker(
                pastOnly,
                out List<Joint> tree, out List<Joint> end,
                out int size, out int biggest, out int smallest
                );
            connectionTree = tree;
            connectionEnd = end;
            treeSize = size;
            biggestKey = biggest;
            smallestKey = smallest;
        }
        public void connectJointTo(Joint newJoint){
            getFutureConnections( 
                out List<Joint> connectionTree,
                out _,
                out int treeSize,out _,out _
                );
            if (body != newJoint.body){
                newJoint.body.resizeJoints(treeSize);
                disconnectPast();
                connectPastTo(newJoint);
                for (int i =0; i< treeSize;i++){
                    Joint joint = connectionTree[i];
                    joint.body.returnJointKey(joint.connection.current);
                    joint.setBody(newJoint.body);
                    joint.connection.current = newJoint.keyGenerator.getKey();
                    newJoint.body.bodyStructure[joint.connection.current] = joint;
                }   
            } else if (!connectionTree.Contains(newJoint)) {
                disconnectPast();
                connectPastTo(newJoint);
            }
        }
        void connectFutureTo(Joint joint){
            List<Joint> connectTo = joint.connection.past;
            if (!connectTo.Contains(this)) connectTo.Add(this);
            if (!connection.future.Contains(joint)) connection.future.Add(joint);
        }
        void connectPastTo(Joint joint){
            List<Joint> connectTo = joint.connection.future;
            if (!connectTo.Contains(this)) connectTo.Add(this);
            if (!connection.past.Contains(joint)) connection.past.Add(joint);
        }
        public void disconnectFuture(){
            bool futureOnly = true;
            disconnect(connection.future,futureOnly);
            connection.future.Clear();
        }
        public void disconnectPast(){
            bool pastOnly = false;
            disconnect(connection.past,pastOnly);
            connection.past.Clear();
        }
        void disconnect(List<Joint> joints, bool pastOrFuture){
            int size = joints.Count;
            if (pastOrFuture) 
                for (int i =0; i<size;i++){
                    joints[i].connection.future.Remove(this);
                }
                else 
                for (int i =0; i<size;i++){
                    joints[i].connection.past.Remove(this);
                }
        }
        void connectionTracker(
            bool pastOrFuture,
            out List<Joint> connectionTree, out List<Joint> connectionEnd,
            out int treeSize, out int biggestKey,out int smallestKey
            ){
            Joint[] joints = body.bodyStructure;
            List<Joint> tree = new List<Joint>{this};  
            if (pastOrFuture) 
                tree.AddRange(connection.future); 
                else tree.AddRange(connection.past);               
            int size = tree.Count;
            int biggest = 0;
            int smallest = connection.current;
            List<Joint> end = new List<Joint>();
            for (int i=0; i< size; i++){
                List<Joint> tracker = pastOrFuture ?
                    joints[tree[i].connection.current].connection.future:
                    joints[tree[i].connection.current].connection.past;
                int trackerSize = tracker.Count;
                if (trackerSize > 0){
                    for(int e = 0; e < trackerSize; e++){
                        Joint joint = tracker[e];
                        int current = joint.connection.current;
                        if (current > biggest) biggest = current;
                        if (current < smallest) smallest = current;
                        tree.Add(joint);
                        size++;
                    };
                } else {
                    end.Add(tree[i]);
                }
            }
            connectionTree = tree;
            connectionEnd = end;
            treeSize = size;
            biggestKey = biggest;
            smallestKey = smallest;
        }
        public void optimizeCollisionSpheres(){
            int maxKeys = keyGenerator.maxKeys;
            int used = keyGenerator.availableKeys;
            CollisionSphere[] newCollision = new CollisionSphere[used];
            int collisionCount = 0;
            for (int j = 0; j<maxKeys; j++){
                CollisionSphere collision = collisionSpheres[j];
                if (collision != null){
                    collision.path.setJointKey(connection.current);
                    collision.path.setCollisionSphereKey(collisionCount);
                    newCollision[collisionCount] = collision;
                    collisionCount++;
                }
            }
            collisionSpheres = newCollision;
            keyGenerator.resetGenerator(collisionCount);
        }
    }

    public class Path {
        public Body body;
        public int jointKey;
        public int collisionSphereKey;

        public Path(){}
        public Path(Body body, int jointKey, int collisionSphereKey){
            this.body=body;
            this.jointKey=jointKey;
            this.collisionSphereKey=collisionSphereKey;
        }

        public void setBody(Body body){
            this.body=body;
        }
        public void setJointKey(int jointKey){
            this.jointKey = jointKey;
        }
        public void setCollisionSphereKey(int collisionSphereKey){
            this.collisionSphereKey = collisionSphereKey;
        }
    }

    public class CollisionSphere {
        public Path path;
        public Vector3 origin;
        public float radius;

        public CollisionSphere(){}
        public CollisionSphere(Path path,Vector3 origin,float radius){
            this.path = path;
            this.origin = origin;
            this.radius = radius;
        }
    }

    public class Triangle {
        public int a,b,c;
        public void setAll(int a,int b,int c){
            this.a = a;
            this.b = b;
            this.c = c;
        }
    }

    public class BodyMesh {
        public List<Vector3> vertex;
        public List<Triangle> indices;
        public void setAll(List<Vector3> vertex,List<Triangle> indices){
            this.vertex = vertex;
            this.indices = indices;
        }
    }

    public class Timer{
        public float time;
        public void setAll(float time){
            this.time = time;
        }
        public void add(float time){
            this.time += time;
        }
    }
}
