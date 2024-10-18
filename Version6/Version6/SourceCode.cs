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
    public class RenderAxis{
        public Axis axis;
        public Sphere origin,x,y,z,rotationAxis;
        public bool created = false;

        public RenderAxis(Axis axis){
            this.axis = axis;
            createAxis();
        }
        public void createAxis(){
            if (!created){
                origin = new Sphere(axis.origin,1,new Color(1,1,1,0));
                x = new Sphere(axis.x,1,new Color(1,0,0,0));
                y = new Sphere(axis.y,1,new Color(0,1,0,0));
                z = new Sphere(axis.z,1,new Color(0,0,1,0));
                rotationAxis = new Sphere(axis.rotationAxis,1,new Color(0,0,0,0));
                updateAxis();
                updateRotationAxis();
                created = true;
            }
        }

        public void deleteAxis(){
            if (created){
                origin.destroySphere();
                x.destroySphere();
                y.destroySphere();
                z.destroySphere();
                rotationAxis.destroySphere();
                created = false;
            }
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
        public RenderAxis renderAxis;
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
            renderAxis = new RenderAxis(this);
            worldAxisAngleX = 0;
            rotationAxisAngleX = 0;
        }
        
        public void moveAxis(Vector3 add){
            origin += add;
            x += add;
            y += add;
            z += add;
            rotationAxis += add;
            if (renderAxis.created){
                renderAxis.updateAxis();
                renderAxis.updateRotationAxis();
            }
        }
        public void placeAxis(Vector3 newOrigin){
            Vector3 newPosition = newOrigin-origin;
            moveAxis(newPosition);
            if (renderAxis.created){
                renderAxis.updateAxis();
                renderAxis.updateRotationAxis();
            }
        }
        public void scaleAxis(float newDistance){
            if (newDistance > 0f){
                distance = newDistance;
                x = origin + distanceFromOrign(x,origin);
                y = origin + distanceFromOrign(y,origin);
                z = origin + distanceFromOrign(z,origin);
                if (renderAxis.created){
                    renderAxis.updateAxis();
                }
            }
        }
        public void scaleRotationAxis(float newDistance){
            if (newDistance > 0f){
                distance = newDistance;
                rotationAxis = origin + distanceFromOrign(rotationAxis,origin);
                if (renderAxis.created){
                    renderAxis.updateRotationAxis();
                }
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
            print(angleY*180/Mathf.PI);
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
            if (renderAxis.created){
                renderAxis.updateAxis();
                renderAxis.updateRotationAxis();
            }
        }
        public void getRotationAxis(out float angleY,out float angleX){
            getAngle(rotationAxis,origin,x,y,z,out angleY,out angleX);
            if (angleY == 0f || angleY < Mathf.PI) angleX = rotationAxisAngleX;
        }
        public void setRotationAxis(float setAngleY,float setAngleX){
            Vector4 rotY = angledAxis(setAngleY,y);
            Vector4 rotX = angledAxis(setAngleX,x);
            rotationAxis = quatRotate(rotationAxis,origin,rotX);
            rotationAxis = quatRotate(rotationAxis,origin,rotY);
            rotationAxisAngleX = setAngleX;
            if (renderAxis.created){
                renderAxis.updateRotationAxis();
            }
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
            if (renderAxis.created){
                renderAxis.updateAxis();
                renderAxis.updateRotationAxis();
            }
        }
        public void getRotationAxisInDegrees(out float addAngleY,out float addAngleX){
            float radianToDegree = 180/Mathf.PI;
            getRotationAxis(out addAngleY, out addAngleX);
            addAngleY *= radianToDegree;
            addAngleX *= radianToDegree;
        }
        public void setRotationAxisInDegrees(float setAngleY,float setAngleX){
            float degreeToRadian = Mathf.PI/180;
            setAngleY *= degreeToRadian;
            setAngleX *= degreeToRadian;
            setRotationAxis(setAngleY, setAngleX);
            if (renderAxis.created){
                renderAxis.updateRotationAxis();
            }
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
                freeKeys.Add(maxKeys+increaseKeysBy-i-1);
            }
            availableKeys += increaseKeysBy;
            maxKeys += increaseKeysBy;
        }
        public void setIncreaseKeysBy(int newLimit){
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
            reviveBody();
        }

        public void reviveBody(){
            if (keyGenerator.availableKeys == keyGenerator.maxKeys){
                int key = keyGenerator.getKey();
                Connection connection = new Connection(key);
                Axis axis = new Axis(globalAxis.origin,globalAxis.distance);
                Joint addJoint = new Joint(keyGenerator.increaseKeysBy, axis, connection); 
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
                int oldLimit = keyGenerator.increaseKeysBy;
                int oldMax = keyGenerator.maxKeys;
                keyGenerator.setIncreaseKeysBy(Mathf.Abs(limitCheck) + oldLimit);
                keyGenerator.generateKeys();
                int newMax = keyGenerator.maxKeys;
                Joint[] newJointArray = new Joint[newMax];
                for (int i = 0; i<oldMax; i++){
                    Joint joint = bodyStructure[i];
                    if (joint != null){
                        newJointArray[i] = joint;
                    }
                }
                bodyStructure = newJointArray;
                keyGenerator.setIncreaseKeysBy(oldLimit);
            }
        }
        public List<Joint> getPastEnds(){
            List<Joint> joints = new List<Joint>();
            for (int i =0; i<keyGenerator.maxKeys; i++){
                Joint joint = bodyStructure[i];
                if (joint != null){
                    if (joint.connection.past.Count == 0){
                        joints.Add(joint);
                    }
                }
            }
            return joints;
        }
        public void optimizeBody(){
            bool getOnlyActive = true;
            bool getFuture = true;
            List<Joint> firstJoint = getPastEnds();
            if (firstJoint != null) {
                tracker(
                    firstJoint,
                    getOnlyActive, getFuture,
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
            }
        }
        internal void tracker(
            List<Joint> joints, bool pastOrFuture, bool getOnlyActive,
            out List<Joint> tree, out List<Joint> end, out int treeSize
            ){
            treeSize = joints.Count;
            end = new List<Joint>();
            for (int i=0; i< treeSize; i++){
                List<Joint> tracker = joints[i].connection.nextConnections(pastOrFuture,getOnlyActive);
                int trackerSize = tracker.Count;
                if (trackerSize > 0){
                    joints.AddRange(tracker);
                    treeSize += trackerSize;
                } else {
                    end.Add(joints[i]);
                }
            }
            resetUsed(joints,treeSize);
            tree = joints;
        }
        internal void resetUsed(List<Joint> joints, int size){
            for (int i = 0; i<size;i++){
                joints[i].connection.used = false;
            }
        }
    }

    public class Connection {
        public bool active = true, used = false;
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
            bool getOnlyActive,
            out List<Joint> connectionTree, 
            out List<Joint> connectionEnd,
            out int treeSize
            ){
            bool futureOnly = true;
            connectionTracker(
                futureOnly,getOnlyActive,
                out List<Joint> tree, out List<Joint> end,
                out int size
                );
            connectionTree = tree;
            connectionEnd = end;
            treeSize = size;
        }
        public void getPastConnections(
            bool getOnlyActive,
            out List<Joint> connectionTree, 
            out List<Joint> connectionEnd,
            out int treeSize
            ){
            bool pastOnly = false;
            connectionTracker(
                pastOnly, getOnlyActive,
                out List<Joint> tree, out List<Joint> end,
                out int size
                );
            connectionTree = tree;
            connectionEnd = end;
            treeSize = size;
        }
        public void connectJointTo(Joint newJoint){
            bool getOnlyActive = false;
            getFutureConnections( 
                getOnlyActive,
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
            if (!future.Contains(joint)) future.Add(joint);
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
        public List<Joint> nextConnections(bool pastOrFuture, bool getOnlyActive){
            List<Joint> connectedJoints = new List<Joint>();
            List<Joint> joints = pastOrFuture? future:past;
            int listSize = joints.Count;
            for (int j = 0;j<listSize;j++){
                Joint joint = joints[j];
                bool used = joint.connection.used;
                if (getOnlyActive && joint.connection.active && !used){
                    connectedJoints.Add(joint);
                    joint.connection.used = true;
                } else if (!getOnlyActive && !used){
                    connectedJoints.Add(joint);
                    joint.connection.used = true;
                }
            }
            return connectedJoints;
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
            bool pastOrFuture, bool getOnlyActive,
            out List<Joint> tree, out List<Joint> end,
            out int treeSize
            ){
            tree = new List<Joint>{current};  
            tree.AddRange(nextConnections(pastOrFuture,getOnlyActive));
            current.body.tracker(
                tree, pastOrFuture, getOnlyActive,
                out tree, out end, out treeSize
                );
        }
    }

    public class Joint {
        public Body body;
        public Axis localAxis;
        public Connection connection;
        public PointCloud pointCloud;

        public Joint(){}
        public Joint(int amountOfSpheres, Axis localAxis,Connection connection){
            pointCloud = new PointCloud(amountOfSpheres);
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
            int countPast = connection.past.Count;
            int countFuture = connection.past.Count;
            bool checkMultiConnection = !(countPast >1 && countFuture >1);
            if (checkMultiConnection){
                body.keyGenerator.returnKey(connection.indexInBody);
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
                        bool check = (countFuture == 1)? true:false;
                        for (int i = 0; i<futureEnds.Count;i++){
                            if (check) 
                                futureEnds[0].connection.connectFutureToPast(pastEnds[i]);
                            else
                                futureEnds[i].connection.connectFutureToPast(pastEnds[0]);
                        }
                    }
                }
                pointCloud.deleteAllSpheres();
                body.bodyStructure[connection.indexInBody] = null;
            }
        }
        public void rotateJointHierarchy(float radian,bool rotateOnlyActive){
            Vector4 quat = localAxis.quat(radian);
            rotateHierarchy(quat,rotateOnlyActive);
        }
        public void rotateJointHierarchyInDegrees(float angle,bool rotateOnlyActive){
            float degreeToRadian = Mathf.PI/180;
            Vector4 quat = localAxis.quat(angle*degreeToRadian);
            rotateHierarchy(quat,rotateOnlyActive);
        }
        void rotateHierarchy(Vector4 quat,bool rotateOnlyActive){
            Vector3 rotationOrigin = localAxis.origin;
            List<Joint> tree = new List<Joint>{this};
            int size = 1;       
            for (int i = 0; i<size;i++){
                Joint joint = tree[i];
                List<Joint> joints = tree[i].connection.nextConnections(false,rotateOnlyActive);
                tree.AddRange(joints);
                size += joints.Count;
                joint.localAxis.rotate(quat,rotationOrigin);
                joint.pointCloud.rotateSpheres(quat,rotationOrigin);
            }
            body.resetUsed(tree,size);
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
            resizeArray(1);
            int sphereIndex = keyGenerator.getKey();
            Path path = new Path(joint.body,joint.connection.indexInBody,sphereIndex);
            collisionSpheres[sphereIndex] = 
                new CollisionSphere(path,joint.localAxis.rotationAxis,sphereRadius);
        }
        public void deleteSphere(int key){
            CollisionSphere remove = collisionSpheres[key];
            if(remove != null){
                keyGenerator.returnKey(key);
                collisionSpheres[key].sphere.destroySphere();
                collisionSpheres[key] = null;
            }
        }
        public void deleteAllSpheres(){
            int size = collisionSpheres.Length;
            for (int i = 0; i<size;i++){
                deleteSphere(i);
            }
        }
        public void resizeArray(int amount){
            int availableKeys = keyGenerator.availableKeys;
            int limitCheck = availableKeys - amount;
            if(limitCheck < 0) {
                int oldLimit = keyGenerator.increaseKeysBy;
                int oldMax = keyGenerator.maxKeys;
                keyGenerator.setIncreaseKeysBy(Mathf.Abs(limitCheck) + oldLimit);
                keyGenerator.generateKeys();
                int newMax = keyGenerator.maxKeys;
                CollisionSphere[] newCollisionSpheresArray = new CollisionSphere[newMax];
                for (int i = 0; i<oldMax; i++){
                    CollisionSphere collisionSphere = collisionSpheres[i];
                    if (collisionSphere != null){
                        newCollisionSpheresArray[i] = collisionSphere;
                    }
                }
                collisionSpheres = newCollisionSpheresArray;
                keyGenerator.setIncreaseKeysBy(oldLimit);
            }
        }
        public void rotateSpheres(Vector4 quat,Vector3 rotationOrigin){
            int sphereCount = collisionSpheres.Length;
            for (int i = 0; i<sphereCount; i++){
                CollisionSphere collisionSphere = collisionSpheres[i];
                collisionSphere.setOrigin(
                    joint.localAxis.quatRotate(collisionSphere.sphere.origin,rotationOrigin,quat)
                );
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
        public void destroySphere(){
            Destroy(sphere);
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
