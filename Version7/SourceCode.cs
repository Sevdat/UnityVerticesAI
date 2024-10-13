using System.Collections.Generic;
using System.Linq;
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
    public class renderAxis{
        public Axis axis;
        public Sphere origin,x,y,z,rotationAxis;

        public renderAxis(Axis axis){
            this.axis = axis;
            origin = new Sphere(axis.origin,1,new Color(1,1,1,0));
            x = new Sphere(axis.x,1,new Color(1,0,0,0));
            y = new Sphere(axis.y,1,new Color(0,1,0,0));
            z = new Sphere(axis.z,1,new Color(0,0,1,0));
            rotationAxis = new Sphere(axis.rotationAxis,1,new Color(0,0,0,0));
        }

        public void axisVisibility(bool onOrOff){
            origin.sphere.GetComponent<MeshRenderer>().enabled = onOrOff;
            x.sphere.GetComponent<MeshRenderer>().enabled = onOrOff;
            y.sphere.GetComponent<MeshRenderer>().enabled = onOrOff;
            z.sphere.GetComponent<MeshRenderer>().enabled = onOrOff;
            rotationAxis.sphere.GetComponent<MeshRenderer>().enabled = onOrOff;
        }
        public void updateAxis(){
            origin.setOrigin(axis.origin);
            x.setOrigin(axis.x);
            y.setOrigin(axis.y);
            z.setOrigin(axis.z);
        }
        public void updateRotationAxis(){
            rotationAxis.setOrigin(axis.rotationAxis);
        }
    }
    public class Axis {
        public renderAxis renderAxis;
        public Vector3 origin,x,y,z,rotationAxis;
        public float distance,worldAxisAngleX,rotationAxisAngleX;

        public Axis(){}
        public Axis(Vector3 origin, float distance){
            this.origin = origin;
            this.distance = (distance >0.1f)? distance:1f;
            x = origin + new Vector3(distance,0,0);
            y = origin + new Vector3(0,distance,0);
            z = origin + new Vector3(0,0,distance);
            rotationAxis = origin + new Vector3(0,distance,0);
            renderAxis = new renderAxis(this);
            worldAxisAngleX = 0;
            rotationAxisAngleX = 0;
        }
        
        public void moveAxis(Vector3 add){
            origin += add;
            x += add;
            y += add;
            z += add;
            rotationAxis += add;
            renderAxis.updateAxis();
            renderAxis.updateRotationAxis();
        }
        public void placeAxis(Vector3 newOrigin){
            Vector3 newPosition = newOrigin-origin;
            moveAxis(newPosition);
            renderAxis.updateAxis();
            renderAxis.updateRotationAxis();
        }
        public void scaleAxis(float newDistance){
            if (newDistance > 0f){
                distance = newDistance;
                x = origin + distanceFromOrign(x,origin);
                y = origin + distanceFromOrign(y,origin);
                z = origin + distanceFromOrign(z,origin);
                renderAxis.updateAxis();
            }
        }
        public void scaleRotationAxis(float newDistance){
            if (newDistance > 0f){
                distance = newDistance;
                rotationAxis = origin + distanceFromOrign(rotationAxis,origin);
                renderAxis.updateRotationAxis();
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
        internal Vector3 direction(Vector3 point,Vector3 origin){ 
            Vector3 v = point-origin;
            return v/length(v);
        }
        internal Vector3 distanceFromOrign(Vector3 point,Vector3 origin){
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
        internal float angleBetweenLines(Vector3 dir1,Vector3 dir2){
            float dotProduct = dot(dir1,dir2);
            float lengthProduct = length(dir1)*length(dir2);
            float check = (dotProduct>lengthProduct)? 1:dotProduct/lengthProduct;
            return Mathf.Acos(check);
        }
        internal Vector3 perpendicular(Vector3 lineOrigin, Vector3 lineDirection, Vector3 point){
            float amount = dot(point-lineOrigin,lineDirection);
            return lineOrigin+amount*lineDirection;
        }
        void rotateAxis(ref Vector3 x, ref Vector3 y,ref Vector3 z,Vector3 axis,float angle){
            Vector4 quat = angledAxis(angle,axis);
            x = quatRotate(x,origin,quat);
            y = quatRotate(y,origin,quat);
            z = quatRotate(z,origin,quat);
        }
        void axisAlignment(
            float worldAngleY, float worldAngleX, float localAngleY,
            Vector3 worldX,Vector3 worldY,
            ref Vector3 localX, ref Vector3 localY, ref Vector3 localZ
            ){
            rotateAxis(ref localX,ref localY,ref localZ,worldX,worldAngleY);
            rotateAxis(ref localX,ref localY,ref localZ,worldY,worldAngleX);
            rotateAxis(ref localX,ref localY,ref localZ,localY,localAngleY);
        }
        internal void getAngle(Vector3 point,Vector3 origin, Vector3 x, Vector3 y, Vector3 z, out float angleY,out float angleX){
            Vector3 dirX = direction(x,origin);
            Vector3 dirY = direction(y,origin);
            Vector3 dirZ = direction(z,origin);
            Vector3 dirH = direction(point,origin);
            Vector3 perpendicularOrigin = perpendicular(origin,dirY,point);

            float checkLength = length(point -perpendicularOrigin);
            Vector3 dirPerpOrg = (checkLength !=0)?direction(point,perpendicularOrigin):normalize(point);
            float angleSide = angleBetweenLines(dirX,dirPerpOrg);
            
            angleY = angleBetweenLines(dirY,dirH);
            angleX = (angleSide>Mathf.PI/2)? 
                2*Mathf.PI-angleBetweenLines(dirZ,dirPerpOrg):
                angleBetweenLines(dirZ,dirPerpOrg);
            if (angleY == 0f || angleY < Mathf.PI) angleX = worldAxisAngleX;
        }

        public void getWorldRotation(out float worldAngleY,out float worldAngleX,out float localAngleY){
            Vector3 worldX = origin + new Vector3(distance,0,0);
            Vector3 worldY = origin + new Vector3(0,distance,0);
            Vector3 worldZ = origin + new Vector3(0,0,distance);
            getAngle(y,origin,worldX,worldY,worldZ,out worldAngleY,out worldAngleX);
            
            Vector3 localX = origin + new Vector3(distance,0,0);
            Vector3 localY = origin + new Vector3(0,distance,0);
            Vector3 localZ = origin + new Vector3(0,0,distance);
            axisAlignment(
                worldAngleY,worldAngleX,0,
                worldX,worldY,ref localX,ref localY,ref localZ
                );   
            Vector3 dirX = direction(x,origin);
            Vector3 dirLocalX = direction(localX,origin);
            Vector3 dirZ = direction(z,origin);
            Vector3 dirLocalZ = direction(localZ,origin);
            float angleSide = angleBetweenLines(dirX,dirLocalX);

            localAngleY = (angleSide>Mathf.PI/2)? 
                2*Mathf.PI-angleBetweenLines(dirZ,dirLocalZ):
                angleBetweenLines(dirZ,dirLocalZ);
            
            if (worldAngleY == 0f || worldAngleY < Mathf.PI) worldAngleX = worldAxisAngleX;
        }
        public void setWorldRotation(float worldAngleY,float worldAngleX,float localAngleY){
            Vector3 worldX = origin + new Vector3(distance,0,0);
            Vector3 worldY = origin + new Vector3(0,distance,0);
            
            Vector3 localX = origin + new Vector3(distance,0,0);
            Vector3 localY = origin + new Vector3(0,distance,0);
            Vector3 localZ = origin + new Vector3(0,0,distance);
            
            axisAlignment(
                worldAngleY,worldAngleX,localAngleY,
                worldX,worldY,ref localX,ref localY,ref localZ
                );

            x = localX; y = localY; z = localZ;
            worldAxisAngleX = worldAngleX;
            renderAxis.updateAxis();
            renderAxis.updateRotationAxis();
        }
        public void moveRotationAxis(float addAngleY,float addAngleX){
            Vector4 rotY = angledAxis(addAngleY,x);
            Vector4 rotX = angledAxis(addAngleX,y);
            rotationAxis = quatRotate(rotationAxis,origin,rotY);
            rotationAxis = quatRotate(rotationAxis,origin,rotX);
            rotationAxisAngleX += addAngleX;
            renderAxis.updateRotationAxis();
        }
        public void setRotationAxis(float setAngleY,float setAngleX){
            Vector4 rotY = angledAxis(setAngleY,y);
            Vector4 rotX = angledAxis(setAngleX,x);
            Vector3 rotationOrigin = y;
            rotationOrigin = quatRotate(rotationOrigin,origin,rotY);
            rotationOrigin = quatRotate(rotationOrigin,origin,rotX);
            rotationAxis = rotationOrigin;
            rotationAxisAngleX = setAngleX;
            renderAxis.updateRotationAxis();
        }
        public void getRotationAxisAngle(out float angleY,out float angleX){
            getAngle(rotationAxis,origin,x,y,z,out angleY,out angleX);
            if (angleY == 0f || angleY < Mathf.PI) angleX = rotationAxisAngleX;
        }

        public void getWorldRotationInDegrees(out float worldAngleY,out float worldAngleX,out float localAngleY){
            float radianToDegree = 180/Mathf.PI;
            getWorldRotation(out worldAngleY, out worldAngleX, out localAngleY);
            worldAngleY *= radianToDegree;
            worldAngleX *= radianToDegree;
            localAngleY *= radianToDegree;
        }
        public void setWorldRotationInDegrees(float worldAngleY,float worldAngleX,float localAngleY){
            float degreeToRadian = Mathf.PI/180;
            worldAngleY *= degreeToRadian;
            worldAngleX *= degreeToRadian;
            localAngleY *= degreeToRadian;
            setWorldRotation(worldAngleY, worldAngleX, localAngleY);
        }
        public void moveRotationAxisInDegrees(float addAngleY,float addAngleX){
            float degreeToRadian = Mathf.PI/180;
            addAngleY *= degreeToRadian;
            addAngleX *= degreeToRadian;
            moveRotationAxis(addAngleY, addAngleX);
        }
        public void setRotationAxisInDegrees(float addAngleY,float addAngleX){
            float degreeToRadian = Mathf.PI/180;
            addAngleY *= degreeToRadian;
            addAngleX *= degreeToRadian;
            setRotationAxis(addAngleY, addAngleX);
        }
        public void getRotationAxisAngleInDegrees(out float addAngleY,out float addAngleX){
            float radianToDegree = 180/Mathf.PI;
            getRotationAxisAngle(out addAngleY, out addAngleX);
            addAngleY *= radianToDegree;
            addAngleX *= radianToDegree;
        }


        public Vector4 quat(float radian){
            return angledAxis(radian,rotationAxis);
        }
        public Vector4 quatInDegrees(float angle){
            float degreeToRadian = Mathf.PI/180;
            return angledAxis(angle*degreeToRadian,rotationAxis);
        }
        public void rotate(Vector4 quat,Vector3 rotationOrigin){
            origin = quatRotate(origin,rotationOrigin,quat);
            x = quatRotate(x,rotationOrigin,quat);
            y = quatRotate(y,rotationOrigin,quat);
            z = quatRotate(z,rotationOrigin,quat);
            rotationAxis = quatRotate(rotationAxis,rotationOrigin,quat);
            renderAxis.updateAxis();
            renderAxis.updateRotationAxis();
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
                float halfAngle = angle * 0.5f;
                float sinHalfAngle = Mathf.Sin(halfAngle);
                float w = Mathf.Cos(halfAngle);
                float x = normilized.x * sinHalfAngle;
                float y = normilized.y * sinHalfAngle;
                float z = normilized.z * sinHalfAngle;
                return new Vector4(x,y,z,w);
        }
        public Vector3 quatRotate(Vector3 point, Vector3 origin, Vector4 angledAxis){
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
            int index = availableKeys-1;
            int key = freeKeys[index];
            freeKeys.RemoveAt(index); 
            availableKeys -= 1;
            if (availableKeys-1<0) generateKeys();
            return key;
        }
        public void returnKey(int key){
            if (availableKeys < maxKeys){
                freeKeys.Add(key);
                availableKeys +=1;
            }
        }
        public void resetGenerator(int newMax){
            freeKeys.Clear();
            freeKeys.TrimExcess();
            maxKeys = newMax;
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
        public Body(Axis globalAxis, int amountOfJoints){
            this.globalAxis = globalAxis;
            bodyStructure = new Joint[amountOfJoints];
            keyGenerator = new KeyGenerator(amountOfJoints);
        }

        public void reviveBody(){
            if (keyGenerator.availableKeys == keyGenerator.maxKeys){
                keyGenerator = new KeyGenerator(keyGenerator.maxKeys);
                int key = keyGenerator.getKey();
                Connection connection = new Connection(key);
                Joint addJoint = new Joint(keyGenerator.increaseKeysBy, globalAxis,connection); 
                bodyStructure[key] = addJoint;
            } 
        }
        public void reActivateBody(){
            int size = keyGenerator.maxKeys - keyGenerator.availableKeys;
            for (int i = 0; i< size; i++){
                bodyStructure[i].connection.active = true;
            }
        }
        public void resizeArray(int amount){
            int availableKeys = keyGenerator.availableKeys;
            int limitCheck = availableKeys - amount;
            if(limitCheck < 0) {
                keyGenerator.setLimit(amount + Mathf.Abs(limitCheck));
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
        public Joint getFirstJoint(){
            Joint firstJoint = null;
            for (int i =0; i<keyGenerator.maxKeys; i++){
                Joint joint = bodyStructure[i];
                if (joint != null){
                    if (joint.connection.past.Count == 0){
                        firstJoint = joint;
                        break;
                    }
                }
            }
            return firstJoint;
        }
        public void optimizeBody(){
            Joint firstJoint = getFirstJoint();
            if (firstJoint != null) {
                firstJoint.connection.getFutureConnections(
                    out List<Joint> connectionTree, 
                    out _, 
                    out int treeSize
                );
                Joint[] orginizedJoints = new Joint[treeSize];
                for (int i = 0; i < treeSize; i++){
                    Joint joint = connectionTree[i];
                    joint.connection.indexInBody = i;
                    joint.pointCloud.optimizeCollisionSpheres();
                    orginizedJoints[i] = joint; 
                }
                bodyStructure = orginizedJoints;
                keyGenerator.resetGenerator(treeSize);
            } else reviveBody();
        }
    }

    public class Connection {
        public bool active = true; // for schematic simulations
        public int indexInBody;
        public Joint current;
        public List<Joint> past; 
        public List<Joint> future;

        public Connection(){}
        public Connection(int indexInBody){
            this.indexInBody = indexInBody;
            past = new List<Joint>();
            future = new List<Joint>();
        }
        public Connection(int indexInBody, List<Joint> past,List<Joint> future){
            this.indexInBody = indexInBody;
            this.past = past;
            this.future = future;
        }

        public void setActive(bool active){
            this.active = active;
        }
        public void setCurrent(int indexInBody){
            this.indexInBody = indexInBody;
        }
        public void setPast(List<Joint> past){
            this.past = past;
        }
        public void setFuture(List<Joint> future){
            this.future = future;
        }
        public void getFutureConnections(
            out List<Joint> connectionTree, 
            out List<Joint> connectionEnd,
            out int treeSize
            ){
            bool futureOnly = true;
            connectionTracker(
                futureOnly,
                out List<Joint> tree, out List<Joint> end,
                out int size
                );
            connectionTree = tree;
            connectionEnd = end;
            treeSize = size;
        }
        public void getPastConnections(
            out List<Joint> connectionTree, 
            out List<Joint> connectionEnd,
            out int treeSize
            ){
            bool pastOnly = false;
            connectionTracker(
                pastOnly,
                out List<Joint> tree, out List<Joint> end,
                out int size
                );
            connectionTree = tree;
            connectionEnd = end;
            treeSize = size;
        }
        public void connectJointTo(Joint newJoint){
            getFutureConnections( 
                out List<Joint> connectionTree,
                out _,
                out int treeSize
                );
            if (current.body != newJoint.body){
                newJoint.body.resizeArray(treeSize);
                disconnectFromPast();
                past.Clear();
                connectPastToFuture(newJoint);
                for (int i =0; i< treeSize;i++){
                    Joint joint = connectionTree[i];
                    joint.body.keyGenerator.returnKey(joint.connection.indexInBody);
                    joint.body.bodyStructure[joint.connection.indexInBody] = null;
                    joint.setBody(newJoint.body);
                    int key = newJoint.body.keyGenerator.getKey();
                    joint.connection.indexInBody = key;
                    newJoint.body.bodyStructure[key] = joint;
                }   
            } else if (!connectionTree.Contains(newJoint)) {
                disconnectFromPast();
                past.Clear();
                connectPastToFuture(newJoint);
            }
        }
        public void connectPastToFuture(Joint joint){
            List<Joint> connectTo = joint.connection.future;
            if (!past.Contains(joint)) past.Add(joint);
            if (!connectTo.Contains(current)) connectTo.Add(current);
        }
        public void connectFutureToPast(Joint joint){
            List<Joint> connectTo = joint.connection.past;
            if (!past.Contains(joint)) future.Add(joint);
            if (!connectTo.Contains(current)) connectTo.Add(current);
        }
        public void disconnectFromFuture(){
            bool futureOnly = true;
            disconnect(future,futureOnly);
        }
        public void disconnectFromPast(){
            bool pastOnly = false;
            disconnect(past,pastOnly);
        }
        void disconnect(List<Joint> joints, bool pastOrFuture){
            int size = joints.Count;
            if (pastOrFuture) 
                for (int i =0; i<size;i++){
                    joints[i].connection.future.Remove(current);
                }
                else 
                for (int i =0; i<size;i++){
                    joints[i].connection.past.Remove(current);
                }
        }
        void connectionTracker(
            bool pastOrFuture,
            out List<Joint> connectionTree, out List<Joint> connectionEnd,
            out int treeSize
            ){
            List<Joint> tree = new List<Joint>{current};  
            if (pastOrFuture) tree.AddRange(future); else tree.AddRange(past);               
            int size = tree.Count;
            List<Joint> end = new List<Joint>();
            for (int i=0; i< size; i++){
                List<Joint> tracker = pastOrFuture ?
                    tree[i].connection.future:
                    tree[i].connection.past;
                int trackerSize = tracker.Count;
                if (trackerSize > 0){
                    tree.AddRange(tracker);
                    size += trackerSize;
                } else {
                    end.Add(tree[i]);
                }
            }
            connectionTree = tree;
            connectionEnd = end;
            treeSize = size;
        }
    }

    public class Joint {
        public Body body;
        public Axis localAxis;
        public Connection connection;
        public PointCloud pointCloud;

        public Joint(){}
        public Joint(int amountOfKeys, Axis localAxis,Connection connection){
            pointCloud = new PointCloud(amountOfKeys);
            this.localAxis = localAxis;
            this.connection = connection;
            connection.current = this;
            pointCloud.joint = this;
        }

        public void setBody(Body body){
            this.body=body;
        }
        public void saveJoint( 
            out float globalX, out float globalY, out float distanceFromOrigin, 
            out float worldAngleY,out float worldAngleX,out float localAngleY
            ){
            Vector3 jointOrigin = localAxis.origin;
            Vector3 globalOrigin = body.globalAxis.origin;
            body.globalAxis.getAngle(
                jointOrigin,
                globalOrigin,body.globalAxis.x,body.globalAxis.y,body.globalAxis.z,
                out globalY,out globalX
                );
            distanceFromOrigin = body.globalAxis.length(jointOrigin-globalOrigin);
            localAxis.getWorldRotation(out worldAngleY,out worldAngleX,out localAngleY);
        }
        public void createJoint(){
            int key = body.keyGenerator.getKey();
            if (body.keyGenerator.maxKeys > body.bodyStructure.Length){
                int amount = body.keyGenerator.maxKeys - body.bodyStructure.Length;
                body.resizeArray(amount);
            }
            Connection connection = new Connection(key);
            connection.past.Add(this);
            Joint addJoint = new Joint(pointCloud.keyGenerator.increaseKeysBy, localAxis,connection); 
            body.bodyStructure[key] = addJoint;
        }
        public void deleteJoint(){
            bool checkMultiConnection = !(connection.past.Count >1 && connection.future.Count >1);
            if (checkMultiConnection){
                body.keyGenerator.returnKey(connection.indexInBody);
                int countPast = connection.past.Count;
                int countFuture = connection.past.Count;
                if (countPast != 0 && countFuture != 0){
                    if (countPast == 0){
                        connection.disconnectFromFuture();
                        connection.future.Clear();
                    } else if (countFuture == 0){
                        connection.disconnectFromPast();
                        connection.past.Clear();
                    } else {
                        connection.disconnectFromFuture();
                        connection.disconnectFromPast();
                        List<Joint> futureEnds = connection.past;
                        List<Joint> pastEnds = connection.future;
                        bool check = (futureEnds.Count == 1)? true:false;
                        for (int i = 0; i<futureEnds.Count;i++){
                            if (check) 
                                futureEnds[0].connection.connectFutureToPast(pastEnds[i]);
                            else
                                futureEnds[i].connection.connectFutureToPast(pastEnds[0]);
                        }
                    }
                }
                body.bodyStructure[connection.indexInBody] = null;
            }
        }
        public void rotateJointHierarchy(float radian){
            Vector4 quat = localAxis.quat(radian);
            rotateHierarchy(quat);
        }
        public void rotateJointHierarchyInDegrees(float angle){
            float degreeToRadian = Mathf.PI/180;
            Vector4 quat = localAxis.quat(angle*degreeToRadian);
            rotateHierarchy(quat);
        }
        void rotateHierarchy(Vector4 quat){
            Vector3 rotationOrigin = localAxis.origin;
            List<Joint> tree = new List<Joint>{this};
            int size = 1;
            for (int i=0; i< size; i++){
                Joint joint = tree[i];
                List<Joint> tracker = tree[i].connection.future;
                int trackerSize = tracker.Count;
                if (trackerSize > 0){
                    tree.AddRange(tracker);
                    size += trackerSize;
                }
                joint.localAxis.rotate(quat,rotationOrigin);
                int sphereCount = joint.pointCloud.collisionSpheres.Length;
                for (int j = 0; i<sphereCount; j++){
                    CollisionSphere collisionSphere = joint.pointCloud.collisionSpheres[j];
                    collisionSphere.setOrigin(
                        joint.localAxis.quatRotate(collisionSphere.sphere.origin,rotationOrigin,quat)
                        );
                }
            } 
        }
    }
    public class PointCloud {
        public Joint joint;
        public KeyGenerator keyGenerator;
        public CollisionSphere[] collisionSpheres;
        
        public PointCloud(){}
        public PointCloud(int amountOfSpheres){
            collisionSpheres = new CollisionSphere[amountOfSpheres];
            keyGenerator = new KeyGenerator(amountOfSpheres);
        }

        public void createSphere(float setAngleY,float setAngleX,float lengthFromOrigin,float sphereRadius){
            joint.localAxis.scaleRotationAxis(lengthFromOrigin);
            joint.localAxis.setRotationAxis(setAngleY,setAngleX);
            resizeArray(keyGenerator.increaseKeysBy);
            int sphereIndex = keyGenerator.getKey();
            Path path = new Path(joint.body,joint.connection.indexInBody,sphereIndex);
            collisionSpheres[sphereIndex] = 
                new CollisionSphere(path,joint.localAxis.rotationAxis,sphereRadius);
        }
        public void deleteSphere(int key){
            CollisionSphere remove = collisionSpheres[key];
            if(remove != null){
                keyGenerator.returnKey(key);
                collisionSpheres[key] = null;
            }
        }
        public void resizeArray(int amount){
            int availableKeys = keyGenerator.availableKeys;
            int limitCheck = availableKeys - amount;
            if(limitCheck < 0) {
                keyGenerator.setLimit(amount + Mathf.Abs(limitCheck));
                keyGenerator.generateKeys();
                int max = keyGenerator.maxKeys;
                int newSize = max + keyGenerator.increaseKeysBy;
                CollisionSphere[] newSphereArray = new CollisionSphere[newSize];
                for (int i = 0; i<max; i++){
                    CollisionSphere sphere = newSphereArray[i];
                    if (sphere != null) newSphereArray[i] = sphere;
                }
            }
        }
        public void optimizeCollisionSpheres(){
            int maxKeys = keyGenerator.maxKeys;
            int used = keyGenerator.availableKeys;
            CollisionSphere[] newCollision = new CollisionSphere[used];
            int count = 0;
            for (int j = 0; j<maxKeys; j++){
                CollisionSphere collision = collisionSpheres[j];
                if (collision != null){
                    collision.path.setJointKey(joint.connection.indexInBody);
                    collision.path.setCollisionSphereKey(count);
                    newCollision[count] = collision;
                    count++;
                }
            }
            collisionSpheres = newCollision;
            keyGenerator.resetGenerator(count);
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
        public Sphere sphere;

        public CollisionSphere(){}
        public CollisionSphere(Path path,Vector3 origin,float radius){
            this.path = path;
            sphere = new Sphere(origin,radius);
        }
        public void setOrigin(Vector3 newOrigin){
            sphere.setOrigin(newOrigin);
        }
        public void setRadius(float newRadius){
            sphere.setRadius(newRadius);
        }
    }
    public class Sphere{
        public Vector3 origin;
        public float radius;
        public Color color;
        public GameObject sphere;

        public Sphere(Vector3 origin, float radius){
            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.GetComponent<Collider>().enabled = false;
            setOrigin(origin);
            setRadius(radius);
        }
        public Sphere(Vector3 origin, float radius, Color color){
            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.GetComponent<Collider>().enabled = false;
            setOrigin(origin);
            setRadius(radius);
            setColor(color);
        }

        public void setOrigin(Vector3 newOrigin){
            origin = newOrigin;
            sphere.transform.position = newOrigin;
        }
        public void setRadius(float newRadius){
            radius = newRadius;
            sphere.transform.localScale = new Vector3(radius, radius, radius);
        }
        public void setColor(Color newColor){
            color = newColor;
            sphere.GetComponent<Renderer>().material.color = newColor;
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
