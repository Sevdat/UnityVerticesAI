using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
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
        public Sphere origin,x,y,z,spinPast,spinFuture,movePast,moveFuture;
        public bool created = false;

        public RenderAxis(Axis axis){
            this.axis = axis;
            origin = new Sphere(axis.origin,1,new Color(1,1,1,0));
            x = new Sphere(axis.x,1,new Color(1,0,0,0));
            y = new Sphere(axis.y,1,new Color(0,1,0,0));
            z = new Sphere(axis.z,1,new Color(0,0,1,0));
            spinPast = new Sphere(y.origin,1,new Color(1,0,1,1));
            spinFuture = new Sphere(y.origin,1,new Color(0,0,0,0));
            movePast = new Sphere(axis.origin,1,new Color(1.0f, 0.64f, 0.0f,0.0f));
            moveFuture = new Sphere(axis.origin,1,new Color(1.0f, 0.64f, 0.0f,0.0f));
            created = true;
        }
        public void createAxis(){
            if (!created){
                origin.restoreSphere();
                x.restoreSphere();
                y.restoreSphere();
                z.restoreSphere();
                spinPast.restoreSphere();
                spinFuture.restoreSphere();
                movePast.restoreSphere();
                moveFuture.restoreSphere();
                axis.spinPast.sphere = spinPast;
                axis.spinFuture.sphere = spinFuture;
                axis.movePast.sphere = movePast;
                axis.moveFuture.sphere = moveFuture;
                updateAxis();
                created = true;
            }
        }
        public void deleteAxis(){
            if (created){
                origin.destroySphere();
                x.destroySphere();
                y.destroySphere();
                z.destroySphere();
                spinPast.destroySphere();
                spinFuture.destroySphere();
                movePast.destroySphere();
                moveFuture.destroySphere();
                created = false;
            }
        }
        public void updateAxis(){
            origin.setOrigin(axis.origin);
            x.setOrigin(axis.x);
            y.setOrigin(axis.y);
            z.setOrigin(axis.z);
        }
    }
    public class AroundAxis{
        public Sphere sphere;
        public Axis axis;
        public float angleY, sensitivitySpeedY, sensitivityAccelerationY,
                     angleX, sensitivitySpeedX, sensitivityAccelerationX,
                     distance, speed, acceleration;
        
        public AroundAxis(){}
        public AroundAxis(Axis axis, Sphere sphere){
            this.sphere = sphere;
            this.axis = axis;
            angleY = 0; angleX = 0;
            sensitivitySpeedY = 0; sensitivityAccelerationY = 1;
            sensitivitySpeedX = 0; sensitivityAccelerationX = 1;
            speed = 0; acceleration = 1;
            distance = axis.length(sphere.origin-axis.origin);
            sphere.setOrigin(axis.origin + new Vector3(0,distance,0));
        }
        public void scale(float newDistance){
            if (newDistance>0){
                distance = Mathf.Abs(newDistance);
                Vector3 add = (sphere.origin != axis.origin) ? axis.distanceFromOrigin(sphere.origin,axis.origin,distance) : new Vector3(0,0,0);
                sphere.origin = axis.origin + add;
            } else {
                distance = 0;
                sphere.origin = axis.origin;
            }
            if (axis.renderAxis.created){
                updateAxis();
            }
        }

        public void get(){
            bool over180 = (angleY>Mathf.PI)? true:false;
            axis.getPointAroundOrigin(sphere.origin,out float tempAngleY, out float tempAngleX);
            if (!float.IsNaN(angleY)&& !float.IsNaN(angleY)){
                angleY = tempAngleY;
                if (!(angleY == 0f || angleY == Mathf.PI)) angleX = tempAngleX;
                if (over180) {
                    angleY = 2*Mathf.PI-angleY;
                    angleX = (Mathf.PI+angleX)%(2*Mathf.PI);
                    };
            }
        }

        void set(float angleY,float angleX){
            sphere.origin = axis.setPointAroundOrigin(angleY,angleX,distance);
            this.angleY = axis.convertTo360(angleY);
            this.angleX = axis.convertTo360(angleX);
            if (axis.renderAxis.created) updateAxis();
            
        }
        public void resetOrigin(){
            set(angleY,angleX);
        }
        public void getInRadians(out float angleY,out float angleX){
            get();
            angleY = this.angleY;
            angleX = this.angleX;
        }
        public void setInRadians(float angleY,float angleX){
            set(angleY, angleX);
        }
        public void getInDegrees(out float angleY,out float angleX){
            float radianToDegree = 180/Mathf.PI;
            get();
            angleY = this.angleY*radianToDegree;
            angleX = this.angleX*radianToDegree;
        }
        public void setInDegrees(float angleY,float angleX){
            float degreeToRadian = Mathf.PI/180;
            angleY *= degreeToRadian;
            angleX *= degreeToRadian;
            set(angleY, angleX);
        }

        public Vector4 quat(float radian){
            return axis.angledAxis(radian,sphere.origin);
        }
        public Vector4 quatInDegrees(float angle){
            float degreeToRadian = Mathf.PI/180;
            return axis.angledAxis(angle*degreeToRadian,sphere.origin);
        }
         
        public void rotationY(){
            float abs = Mathf.Abs(angleY) % (2*Mathf.PI);
            float speedY = Mathf.Sign(sensitivitySpeedY)*(Mathf.Abs(sensitivitySpeedY)%(2*Mathf.PI));
            angleY = axis.convertTo360(abs + speedY);
            set(angleY,angleX);
        }

        public void rotationX(){
            float speedX = Mathf.Sign(sensitivitySpeedX)*(Mathf.Abs(sensitivitySpeedX)%(2*Mathf.PI));
            angleX = axis.convertTo360(angleX + speedX);
            set(angleY,angleX);
        }
        
        public void updatePhysics(bool move){
            speed *= acceleration;
            if (move) distance = speed;
            sensitivitySpeedY *= sensitivityAccelerationY;
            sensitivitySpeedX *= sensitivityAccelerationX;
        }
        public void updateAxis(){
            sphere.setOrigin(sphere.origin);
        }
    }
    public class Axis {
        public Body body;
        public RenderAxis renderAxis;
        public Vector3 origin,x,y,z;
        public float axisDistance;

        public float worldAngleY,worldSpeedY,
                     worldAngleX,worldSpeedX,
                     localAngleY,localSpeedY;
                     
        public AroundAxis spinPast,spinFuture,movePast,moveFuture;
        public Axis(){}
        public Axis(Body body,Vector3 origin, float distance){
            this.body = body;
            this.origin = origin;
            axisDistance = (distance >0.1f)? distance:1f;
            x = origin + new Vector3(distance,0,0);
            y = origin + new Vector3(0,distance,0);
            z = origin + new Vector3(0,0,distance);
            worldAngleY = 0; worldAngleX = 0; localAngleY = 0; 
            worldSpeedY = 0;
            worldSpeedX = 0;
            localSpeedY = 0;
            renderAxis = new RenderAxis(this);
            spinPast = new AroundAxis(this, renderAxis.spinPast);
            spinFuture = new AroundAxis(this, renderAxis.spinFuture);
            movePast = new AroundAxis(this, renderAxis.movePast);
            moveFuture = new AroundAxis(this, renderAxis.moveFuture);
        }
        
        public void moveAxis(Vector3 add){
            origin += add;
            x += add;
            y += add;
            z += add;
            spinPast.sphere.origin += add;
            spinFuture.sphere.origin += add;
            movePast.sphere.origin += add;
            moveFuture.sphere.origin += add;
            if (renderAxis.created){
                renderAxis.updateAxis();
                spinFuture.updateAxis();
                spinPast.updateAxis();
                movePast.updateAxis();
                moveFuture.updateAxis();
            }
        }
        public Vector3 placeAxis(Vector3 newOrigin){
            Vector3 newPosition = newOrigin-origin;
            moveAxis(newPosition);
            if (renderAxis.created){
                renderAxis.updateAxis();
                spinFuture.updateAxis();
                spinPast.updateAxis();
                movePast.updateAxis();
                moveFuture.updateAxis();
            }
            return newPosition;
        }
        public void scaleAxis(float newDistance){
            if (newDistance > 0f){
                axisDistance = newDistance;
                x = origin + distanceFromOrigin(x,origin,axisDistance);
                y = origin + distanceFromOrigin(y,origin,axisDistance);
                z = origin + distanceFromOrigin(z,origin,axisDistance);
                if (renderAxis.created){
                    renderAxis.updateAxis();
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
        internal Vector3 distanceFromOrigin(Vector3 point,Vector3 origin, float distance){
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
            float check = (Mathf.Abs(dotProduct)>lengthProduct)? MathF.Sign(dotProduct):dotProduct/lengthProduct;
            return Mathf.Acos(check);
        }
        internal Vector3 perpendicular(Vector3 lineOrigin, Vector3 lineDirection, Vector3 point){
            float amount = dot(point-lineOrigin,lineDirection);
            return lineOrigin+amount*lineDirection;
        }
        public Vector3 setPointAroundOrigin(float angleY,float angleX, float distance){
            Vector3 point = y;
            angleY = convertTo360(angleY);
            angleX = convertTo360(angleX);
            Vector4 rotY = angledAxis(angleY,x);
            Vector4 rotX = angledAxis(angleX,y);
            point = quatRotate(point,origin,rotY);
            point = quatRotate(point,origin,rotX);
            point = origin + distanceFromOrigin(point,origin,Mathf.Abs(distance));
            return point;
        }
        
        public void getPointAroundOrigin(Vector3 point, out float angleY,out float angleX){
            getAngle(point,origin,x,y,z,out angleY,out angleX);
        }
        internal Vector4 rotateAxis(ref Vector3 x, ref Vector3 y,ref Vector3 z,Vector3 axis,float angle){
            Vector4 quat = angledAxis(angle,axis);
            x = quatRotate(x,origin,quat);
            y = quatRotate(y,origin,quat);
            z = quatRotate(z,origin,quat);
            return quat;
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
        internal float convertTo360(float angle){
            return (angle<0)? (2*Mathf.PI - (Mathf.Abs(angle) % (2*Mathf.PI))) : Mathf.Abs(angle) % (2*Mathf.PI);
        }

        internal void getAngle(Vector3 point,Vector3 origin, Vector3 x, Vector3 y, Vector3 z, out float yAngle,out float xAngle){
            Vector3 dirX = direction(x,origin);
            Vector3 dirY = direction(y,origin);
            Vector3 dirZ = direction(z,origin);
            Vector3 dirH = direction(point,origin);
            yAngle = angleBetweenLines(dirY,dirH);

            if (float.IsNaN(yAngle)) xAngle = float.NaN; else {   
                Vector3 perpendicularOrigin = perpendicular(origin,dirY,point);
                float checkLength = length(point -perpendicularOrigin);
                Vector3 dirPerpOrg = (checkLength !=0)?direction(point,perpendicularOrigin):normalize(point);
                float angleSide = angleBetweenLines(dirX,dirPerpOrg);          
                xAngle = (angleSide>Mathf.PI/2)? 
                    2*Mathf.PI-angleBetweenLines(dirZ,dirPerpOrg):
                    angleBetweenLines(dirZ,dirPerpOrg);
            }
        }

        public void getWorldRotation(){
            Vector3 worldX = origin + new Vector3(axisDistance,0,0);
            Vector3 worldY = origin + new Vector3(0,axisDistance,0);
            Vector3 worldZ = origin + new Vector3(0,0,axisDistance);
            bool over180 = (worldAngleY>Mathf.PI)? true:false;
            worldAngleX = 0;
            localAngleY = 0;
            getAngle(y,origin,worldX,worldY,worldZ,out worldAngleY,out float tempWorldAngleX);
            if (!(worldAngleY == 0f || worldAngleY == Mathf.PI)) worldAngleX = tempWorldAngleX;
                
            Vector3 localX = origin + new Vector3(axisDistance,0,0);
            Vector3 localY = origin + new Vector3(0,axisDistance,0);
            Vector3 localZ = origin + new Vector3(0,0,axisDistance);
            axisAlignment(
                worldAngleY,worldAngleX,0,
                worldX,worldY,ref localX,ref localY,ref localZ
                );
            Vector3 dirLocalX = direction(localX,origin);
            Vector3 dirZ = direction(z,origin);
            Vector3 dirLocalZ = direction(localZ,origin);
            float angleSide = angleBetweenLines(dirZ,dirLocalX);
            localAngleY = (angleSide>Mathf.PI/2)? 
                2*Mathf.PI-angleBetweenLines(dirZ,dirLocalZ):
                angleBetweenLines(dirZ,dirLocalZ);
            if (over180) {
                worldAngleY = 2*Mathf.PI-worldAngleY;
                worldAngleX = convertTo360(Mathf.PI+worldAngleX);
                localAngleY = convertTo360(Mathf.PI+localAngleY);
                };
        }
        public void setWorldRotation(float worldAngleY,float worldAngleX,float localAngleY){
            Vector3 worldX = origin + new Vector3(axisDistance,0,0);
            Vector3 worldY = origin + new Vector3(0,axisDistance,0);
            
            Vector3 localX = origin + new Vector3(axisDistance,0,0);
            Vector3 localY = origin + new Vector3(0,axisDistance,0);
            Vector3 localZ = origin + new Vector3(0,0,axisDistance);

            worldAngleY = convertTo360(worldAngleY);
            worldAngleX = convertTo360(worldAngleX);
            localAngleY = convertTo360(localAngleY);

            axisAlignment(
                worldAngleY,worldAngleX,localAngleY,
                worldX,worldY,ref localX,ref localY,ref localZ
            );

            x = localX; y = localY; z = localZ;

            this.worldAngleY = worldAngleY;
            this.worldAngleX = worldAngleX;
            this.localAngleY = localAngleY;
            spinPast.resetOrigin();
            spinFuture.resetOrigin();
            movePast.resetOrigin();
            moveFuture.resetOrigin();
            if (renderAxis.created){
                renderAxis.updateAxis();
                spinPast.updateAxis();
                spinFuture.updateAxis();
                movePast.updateAxis();
                moveFuture.updateAxis();
            }
        }

        public void getWorldRotationInRadians(out float worldAngleY,out float worldAngleX,out float localAngleY){
            getWorldRotation();
            worldAngleY = this.worldAngleY;
            worldAngleX = this.worldAngleX;
            localAngleY = this.localAngleY;
        }
        public void setWorldRotationInRadians(float worldAngleY, float worldAngleX, float localAngleY){
            setWorldRotation(worldAngleY, worldAngleX, localAngleY);
        }

        public void getWorldRotationInDegrees(out float worldAngleY,out float worldAngleX,out float localAngleY){
            float radianToDegree = 180/Mathf.PI;
            getWorldRotation();
            worldAngleY = this.worldAngleY *radianToDegree;
            worldAngleX = this.worldAngleX *radianToDegree;
            localAngleY = this.localAngleY *radianToDegree;
        }
        public void setWorldRotationInDegrees(float worldAngleY,float worldAngleX,float localAngleY){
            float degreeToRadian = Mathf.PI/180;
            worldAngleY *= degreeToRadian;
            worldAngleX *= degreeToRadian;
            localAngleY *= degreeToRadian;
            setWorldRotation(worldAngleY, worldAngleX, localAngleY);
        }

        public void rotate(Vector4 quat,Vector3 rotationOrigin){
            origin = quatRotate(origin,rotationOrigin,quat);
            x = quatRotate(x,rotationOrigin,quat);
            y = quatRotate(y,rotationOrigin,quat);
            z = quatRotate(z,rotationOrigin,quat);
            spinFuture.sphere.origin = quatRotate(spinFuture.sphere.origin,rotationOrigin,quat);
            spinPast.sphere.origin = quatRotate(spinPast.sphere.origin,rotationOrigin,quat);
            movePast.sphere.origin = quatRotate(movePast.sphere.origin,rotationOrigin,quat);
            moveFuture.sphere.origin = quatRotate(moveFuture.sphere.origin,rotationOrigin,quat);
            if (renderAxis.created){
                renderAxis.updateAxis();
                spinPast.updateAxis();
                spinFuture.updateAxis();
                movePast.updateAxis();
                moveFuture.updateAxis();
            }
        }

        public Vector4 quatMul(Vector4 q1, Vector4 q2) {
            float w = q1.w * q2.w - q1.x * q2.x - q1.y * q2.y - q1.z * q2.z;
            float x = q1.w * q2.x + q1.x * q2.w + q1.y * q2.z - q1.z * q2.y;
            float y = q1.w * q2.y - q1.x * q2.z + q1.y * q2.w + q1.z * q2.x;
            float z = q1.w * q2.z + q1.x * q2.y - q1.y * q2.x + q1.z * q2.w;
            return new Vector4(x, y, z, w);
        }
        public Vector4 angledAxis(float angle,Vector3 point){
            Vector3 normilized = normalize(point - origin); 
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
        public int availableKeys;

        public KeyGenerator(){}
        public KeyGenerator(int amountOfKeys){
            generateKeys(amountOfKeys);
        }

        public void generateKeys(int increaseKeysBy){
            availableKeys += increaseKeysBy;
        }
        public void getKey(){
            availableKeys -= 1;
        }
        public void returnKey(){
            availableKeys +=1;
        }
        public void resetGenerator(){
            availableKeys = 0;
        }
    }
    public class BakedMesh{
        SkinnedMeshRenderer skinnedMeshRenderer;
        Mesh mesh;
        public Vector3[] vertices;

        public BakedMesh(SkinnedMeshRenderer skinnedMeshRenderer){
            this.skinnedMeshRenderer=skinnedMeshRenderer;
            mesh = new Mesh();
            bakeMesh();
        }
        public void bakeMesh(){
            skinnedMeshRenderer.BakeMesh(mesh);
            vertices = mesh.vertices;
        }
        public Vector3 worldPosition(int index){
            return skinnedMeshRenderer.transform.TransformPoint(vertices[index]);
        }
        public GameObject getGameObject(int index){
            Transform[] bones = skinnedMeshRenderer.bones;
            BoneWeight[] boneWeights = mesh.boneWeights; 
            BoneWeight boneWeight = boneWeights[index];
            return bones[boneWeight.boneIndex0].gameObject;
        }
    }
    public class Body {
        public World world;
        public int worldKey;
        public Axis globalAxis;
        public Joint[] bodyStructure;
        public KeyGenerator keyGenerator;
        public Editor editor;
        public List<BakedMesh> bakedMeshes;
        public string amountOfDigits; 
        public int countStart, count;

        public Body(){}
        public Body(int worldKey){
            this.worldKey = worldKey;
            globalAxis = new Axis(this,new Vector3(0,0,0),5);
            bodyStructure = new Joint[0];
            keyGenerator = new KeyGenerator(0);
            editor = new Editor(this);
            editor.initilizeBody();
            amountOfDigits = "0.000000";
            count = 0;
            countStart = 20;
        }

        public void newCountStart(int countStart){
            this.countStart = countStart;    
        }
        public void newAccuracy(int amount){
            string newString;
            if (amount>0){
                newString = "0.0";
                for (int i = 1; i < amount; i++){
                    newString += "0";
                }
                amountOfDigits = newString;
            }
        }

        public string accuracy(float num){
            return num.ToString(amountOfDigits);
        }
        public string saveBodyPosition(bool radianOrAngle){
            Vector3 globalOrigin = globalAxis.origin;
            float worldAngleY,worldAngleX,localAngleY;
            if (radianOrAngle)
                globalAxis.getWorldRotationInDegrees(out worldAngleY,out worldAngleX,out localAngleY);
                else 
                globalAxis.getWorldRotationInRadians(out worldAngleY,out worldAngleX,out localAngleY);
            string stringPath = $"Body_{worldKey}_";
            string globalOriginLocation = $"{stringPath}GlobalOriginLocation: {accuracy(globalOrigin.x)} {accuracy(globalOrigin.y)} {accuracy(globalOrigin.z)}\n";
            string globalAxisRotationXYZ = $"{stringPath}GlobalAxisRotationXYZ: {accuracy(worldAngleY)} {accuracy(worldAngleX)} {accuracy(localAngleY)}\n";
            string radianOrDegree = $"{stringPath}RadianOrAngle: {radianOrAngle}\n";
            return globalOriginLocation + globalAxisRotationXYZ + radianOrDegree;
        }
        public string saveBody(){
            List<int> jointIndexes = new List<int>();
            string strJointIndexes = "";
            for (int i = 0; i<bodyStructure.Length;i++){
                Joint joint = bodyStructure[i];
                if (joint != null){
                    strJointIndexes += $" {i}";
                    jointIndexes.Add(i);
                }
            }
            string stringPath = $"Body_{worldKey}_";
            string updateReadWrite = $"{stringPath}UpdateReadWrite: {countStart}\n";
            string accuracyAmount = $"{stringPath}Accuracy: {amountOfDigits.Length-2}\n";
            string showAxis = $"{stringPath}ShowAxis: {globalAxis.renderAxis.created}\n";
            string globalAxisScale = $"{stringPath}GlobalAxisScale: {accuracy(globalAxis.axisDistance)}\n";
            string bodyStructureSize = $"{stringPath}BodyStructureSize: {bodyStructure.Length}\n";
            string allJointsInBody = $"{stringPath}AllJointsInBody:{strJointIndexes}\n";

            return accuracyAmount + updateReadWrite + showAxis + globalAxisScale + bodyStructureSize + allJointsInBody;
        }

        public void rotateBody(float worldAngleY, float worldAngleX, float localAngleY){
            resetRotateBody();
            float diffWorldAngleY = globalAxis.convertTo360(worldAngleY) - globalAxis.convertTo360(globalAxis.worldAngleY);
            float diffWorldAngleX = globalAxis.convertTo360(worldAngleX) - globalAxis.convertTo360(globalAxis.worldAngleX);
            float diffLocalAngleY =  globalAxis.convertTo360(localAngleY) - globalAxis.convertTo360(globalAxis.localAngleY);

            Vector3 worldX = globalAxis.origin + new Vector3(globalAxis.axisDistance,0,0);
            Vector3 worldY = globalAxis.origin + new Vector3(0,globalAxis.axisDistance,0);
            
            Vector4 quatY = globalAxis.rotateAxis(ref globalAxis.x,ref globalAxis.y,ref globalAxis.z,worldX,globalAxis.convertTo360(diffWorldAngleY));
            Vector4 quatX = globalAxis.rotateAxis(ref globalAxis.x,ref globalAxis.y,ref globalAxis.z,worldY,globalAxis.convertTo360(diffWorldAngleX));
            Vector4 quatLY = globalAxis.rotateAxis(ref globalAxis.x,ref globalAxis.y,ref globalAxis.z,globalAxis.y,globalAxis.convertTo360(diffLocalAngleY));
            globalAxis.spinPast.resetOrigin();           
            globalAxis.spinFuture.resetOrigin();
            globalAxis.movePast.resetOrigin();
            globalAxis.moveFuture.resetOrigin();
            if (globalAxis.renderAxis.created){
                globalAxis.renderAxis.updateAxis();
                globalAxis.spinPast.updateAxis();
                globalAxis.spinFuture.updateAxis();
                globalAxis.movePast.updateAxis();
                globalAxis.moveFuture.updateAxis();
            }
            Joint[] joints = bodyStructure;
            for (int i = 0; i < joints.Length; i++){
                Joint joint = bodyStructure[i];
                if (joint != null){ 
                    Axis axis = joint.localAxis;
                    axis.rotate(quatY,globalAxis.origin);
                    axis.rotate(quatX,globalAxis.origin);
                    axis.rotate(quatLY,globalAxis.origin);
                    axis.worldAngleY = axis.convertTo360(axis.worldAngleY+diffWorldAngleY)%(2*Mathf.PI);
                    axis.worldAngleX = axis.convertTo360(axis.worldAngleX+diffWorldAngleX)%(2*Mathf.PI);
                    axis.localAngleY = axis.convertTo360(axis.localAngleY+diffLocalAngleY)%(2*Mathf.PI);
                    joint.pointCloud.resetAllSphereOrigins();
                }
            }
            globalAxis.worldAngleY = globalAxis.convertTo360(globalAxis.worldAngleY +diffWorldAngleY)%(2*Mathf.PI);
            globalAxis.worldAngleX = globalAxis.convertTo360(globalAxis.worldAngleX +diffWorldAngleX)%(2*Mathf.PI);
            globalAxis.localAngleY = globalAxis.convertTo360(globalAxis.localAngleY +diffLocalAngleY)%(2*Mathf.PI);
            
        }
        void resetRotateBody(){
            float diffWorldAngleY = 0 - globalAxis.convertTo360(globalAxis.worldAngleY);
            float diffWorldAngleX = 0 - globalAxis.convertTo360(globalAxis.worldAngleX);
            float diffLocalAngleY =  0 - globalAxis.convertTo360(globalAxis.localAngleY);

            Vector3 worldX = globalAxis.origin + new Vector3(globalAxis.axisDistance,0,0);
            Vector3 worldY = globalAxis.origin + new Vector3(0,globalAxis.axisDistance,0);

            Vector4 quatLY = globalAxis.rotateAxis(ref globalAxis.x,ref globalAxis.y,ref globalAxis.z,globalAxis.y,globalAxis.convertTo360(diffLocalAngleY));
            Vector4 quatX = globalAxis.rotateAxis(ref globalAxis.x,ref globalAxis.y,ref globalAxis.z,worldY,globalAxis.convertTo360(diffWorldAngleX));
            Vector4 quatY = globalAxis.rotateAxis(ref globalAxis.x,ref globalAxis.y,ref globalAxis.z,worldX,globalAxis.convertTo360(diffWorldAngleY));
            
            globalAxis.spinPast.resetOrigin();           
            globalAxis.spinFuture.resetOrigin();
            globalAxis.movePast.resetOrigin();
            globalAxis.moveFuture.resetOrigin();
            if (globalAxis.renderAxis.created){
                globalAxis.renderAxis.updateAxis();
                globalAxis.spinPast.updateAxis();
                globalAxis.spinFuture.updateAxis();
                globalAxis.movePast.updateAxis();
                globalAxis.moveFuture.updateAxis();
            }

            Joint[] joints = bodyStructure;
            for (int i = 0; i < joints.Length; i++){
                Joint joint = bodyStructure[i];
                if (joint != null){ 
                    Axis axis = joint.localAxis;
                    axis.rotate(quatLY,globalAxis.origin);
                    axis.rotate(quatX,globalAxis.origin);
                    axis.rotate(quatY,globalAxis.origin);
                    axis.localAngleY = axis.convertTo360(axis.localAngleY+diffLocalAngleY)%(2*Mathf.PI);
                    axis.worldAngleX = axis.convertTo360(axis.worldAngleX+diffWorldAngleX)%(2*Mathf.PI);
                    axis.worldAngleY = axis.convertTo360(axis.worldAngleY+diffWorldAngleY)%(2*Mathf.PI);
                    joint.pointCloud.resetAllSphereOrigins();
                }
            }
            globalAxis.localAngleY = globalAxis.convertTo360(globalAxis.localAngleY +diffLocalAngleY)%(2*Mathf.PI);
            globalAxis.worldAngleX = globalAxis.convertTo360(globalAxis.worldAngleX +diffWorldAngleX)%(2*Mathf.PI);
            globalAxis.worldAngleY = globalAxis.convertTo360(globalAxis.worldAngleY +diffWorldAngleY)%(2*Mathf.PI);
        }

        public void updatePhysics(){
            int sphereCount = bodyStructure.Length;
            for (int i = 0; i<sphereCount; i++){
                bodyStructure[i]?.updatePhysics();
            }            
        }
        public Dictionary<int,int> arraySizeManager(int amount){
            Dictionary<int,int> newKeys = new Dictionary<int,int>();
            if (amount > bodyStructure.Length){
                resizeArray(amount);
            } else if (amount < bodyStructure.Length){
                newKeys = optimizeBody();
            }
            return newKeys;
        }
        public void resizeArray(int maxKey){
            int limitCheck = bodyStructure.Length - maxKey;
            if(limitCheck <= 0) {
                int newMax = bodyStructure.Length + Mathf.Abs(limitCheck)+1;
                keyGenerator.generateKeys(Mathf.Abs(limitCheck)+1);
                Joint[] newJointArray = new Joint[newMax];
                for (int i = 0; i<bodyStructure.Length; i++){
                    Joint joint = bodyStructure[i];
                    if (joint != null){
                        newJointArray[i] = joint;
                    }
                }
                bodyStructure = newJointArray;
            }
        }
        public Dictionary<int,int> optimizeBody(){
            int max = bodyStructure.Length;
            int newSize = max - keyGenerator.availableKeys;
            Joint[] orginizedJoints = new Joint[newSize];
            int newIndex = 0;
            Dictionary<int,int> newKeys = new Dictionary<int,int>();
            for (int i = 0; i < max; i++){
                Joint joint = bodyStructure[i];
                if (joint != null){
                    newKeys.Add(joint.connection.indexInBody,newIndex);
                    joint.connection.indexInBody = newIndex;
                    orginizedJoints[newIndex] = joint; 
                    newIndex++;
                }
            }
            bodyStructure = orginizedJoints;
            keyGenerator.resetGenerator();
            return newKeys;
        }
    }

    public struct LockedConnection{
        public Joint joint;
        public float y,x,length;
        public LockedConnection(Joint joint, float y,float x, float length){
            this.joint = joint;
            this.y = y;
            this.x = x;
            this.length = length;
        }
    }
    public class Connection {
        public bool active = true, used = false;
        public int indexInBody;
        public Joint current;
        public List<LockedConnection> past; 
        public List<LockedConnection> future;

        public Connection(){}
        public Connection(Joint joint, int indexInBody){
            current = joint;
            this.indexInBody = indexInBody;
            past = new List<LockedConnection>();
            future = new List<LockedConnection>();
        }

        public void resetFutureLockPositions(){
            int futureSize = future.Count;
            for (int i =0;i<futureSize;i++){
                LockedConnection locked = future[i];
                Vector3 newPosition = current.localAxis.setPointAroundOrigin(locked.y,locked.x,locked.length);
                locked.joint.moveJoint(newPosition - locked.joint.localAxis.origin);
            }
        }
        public void resetPastLockPositions(){
            int futureSize = past.Count;
            for (int i =0;i<futureSize;i++){
                LockedConnection locked = past[i];
                Vector3 newPosition = current.localAxis.setPointAroundOrigin(locked.y,locked.x,locked.length);
                locked.joint.moveJoint(newPosition - locked.joint.localAxis.origin);
            }
        }
        internal void disconnectFuture(int index){
            future[index].joint.connection.past.RemoveAll(e => e.joint == current);
        } 
        public void disconnectAllFuture(){
            int size = future.Count;
            for (int i =0; i<size;i++){
                disconnectFuture(indexInBody);
            }
            future = new List<LockedConnection>();
        }
        internal void disconnectPast(int index){
            past[index].joint.connection.future.RemoveAll(e => e.joint == current);
        }
        public void disconnectAllPast(){
            int size = past.Count;
            for (int i =0; i<size;i++){
                disconnectPast(indexInBody);
            }
            past = new List<LockedConnection>();
        }
        void lockConnection(Joint joint,out LockedConnection lockThis, out LockedConnection lockOther){
            float jointToCurrentX, jointToCurrentY, currentToJointX, currentToJointY;
            float length = current.localAxis.length(current.localAxis.origin - joint.localAxis.origin);
            if (length>0){
                current.localAxis.getPointAroundOrigin(joint.localAxis.origin, out currentToJointY, out currentToJointX);
                joint.localAxis.getPointAroundOrigin(current.localAxis.origin, out jointToCurrentY, out jointToCurrentX);
            } else {
                jointToCurrentY = 0;
                jointToCurrentX = 0;
                currentToJointY = 0;
                currentToJointX = 0;
            }
            lockThis = new LockedConnection(joint,currentToJointY,currentToJointX,length);
            lockOther = new LockedConnection(current,jointToCurrentY,jointToCurrentX,length);
        }
        public void connectThisPastToFuture(Joint joint, out LockedConnection lockThis, out LockedConnection lockOther){
            List<LockedConnection> connectTo = joint.connection.future;
            lockConnection(joint, out lockThis, out lockOther);
            past.Add(lockThis);
            connectTo.Add(lockOther);
        }
        public void connectThisFutureToPast(Joint joint, out LockedConnection lockThis, out LockedConnection lockOther){
            List<LockedConnection> connectTo = joint.connection.past;
            lockConnection(joint, out lockThis, out lockOther);
            future.Add(lockThis);
            connectTo.Add(lockOther);
        }

        public List<Joint> nextConnections(bool pastOrFuture){
            List<Joint> connectedJoints = new List<Joint>();
            List<LockedConnection> joints = pastOrFuture? future:past;
            int listSize = joints.Count;
            for (int j = 0;j<listSize;j++){
                Joint joint = joints[j].joint;
                bool used = joint.connection.used;
                if (joint.connection.active && !used){
                    connectedJoints.Add(joint);
                    joint.connection.used = true;
                }
            }
            used = true;
            return connectedJoints;
        }
        public List<Joint> getPast(){
            return nextConnections(false);
        }
        public List<Joint> getFuture(){
            return nextConnections(true);
        }

        public List<Joint> getAll(){
            List<Joint> pastAndFuture = new List<Joint>();
            pastAndFuture.AddRange(getPast());
            used = false;
            pastAndFuture.AddRange(getFuture());
            return pastAndFuture;
        }
        public string pastToString(){
            string pastIndexes = "";
            int count = past.Count;
            for (int i = 0; i<count;i++){
                pastIndexes += $"{past[i].joint.connection.indexInBody} ";
            }
            return pastIndexes;
        }
        public string futureToString(){
            string futureIndexes = "";
            int count = future.Count;
            for (int i = 0; i<count;i++){
                futureIndexes += $"{future[i].joint.connection.indexInBody} ";
            }
            return futureIndexes;
        }
    }

    public class Joint {
        public Body body;
        public Axis localAxis;
        public Connection connection;
        public PointCloud pointCloud;
        public float fromGlobalAxisY,fromGlobalAxisX,distanceFromGlobalAxis;
        public bool movementOption;
        public bool keepBodyTogether;

        public Joint(){}
        public Joint(Body body, int indexInBody){
            this.body = body;
            localAxis = new Axis(body,new Vector3(0,0,0),5);
            connection = new Connection(this,indexInBody);
            pointCloud = new PointCloud(this,0);
            body.keyGenerator.getKey();
            fromGlobalAxisY = 0;
            fromGlobalAxisX = 0;
            movementOption = false;
            keepBodyTogether = true;
        }

        public void setBody(Body body){
            this.body=body;
        }
        public string saveJointPosition(bool radianOrAngle){
            float convert = radianOrAngle? 180f/Mathf.PI:1;
            float worldAngleY,worldAngleX,localAngleY;
            Vector3 jointOrigin = localAxis.origin;
            Vector3 globalOrigin = body.globalAxis.origin;
            float distanceFromOrigin = body.globalAxis.length(jointOrigin-globalOrigin);
            distanceFromGlobalAxis = distanceFromOrigin;
            body.globalAxis.getPointAroundOrigin(jointOrigin,out float globalY,out float globalX);
            if (!float.IsNaN(globalY)&& !float.IsNaN(globalX)){
                bool over180 = (fromGlobalAxisY>Mathf.PI)? true:false;
                fromGlobalAxisY = globalY;
                if (!(globalY == 0f || globalY == Mathf.PI)) fromGlobalAxisX = globalX;
                if (over180) {
                    fromGlobalAxisY = 2*Mathf.PI-fromGlobalAxisY;
                    fromGlobalAxisX = (Mathf.PI+fromGlobalAxisX)%(2*Mathf.PI);
                    };
            }
            if (radianOrAngle) 
                localAxis.getWorldRotationInDegrees(out worldAngleY,out worldAngleX,out localAngleY);
                else 
                localAxis.getWorldRotationInRadians(out worldAngleY,out worldAngleX,out localAngleY);
            string stringPath = $"Body_{body.worldKey}_Joint_{connection.indexInBody}_";
            string movementOption = $"{stringPath}MovementOption: {this.movementOption}\n";
            string distanceFromGlobalOrigin = $"{stringPath}DistanceFromGlobalOrigin: {body.accuracy(distanceFromOrigin)}\n";
            string YXFromGlobalAxis = $"{stringPath}YXFromGlobalAxis: {body.accuracy(fromGlobalAxisY*convert)} {body.accuracy(fromGlobalAxisX*convert)}\n";
            string localAxisRotation = $"{stringPath}LocalAxisRotation: {body.accuracy(worldAngleY)} {body.accuracy(worldAngleX)} {body.accuracy(localAngleY)}\n";
            string localOriginLocation = $"{stringPath}LocalOriginLocation: {body.accuracy(localAxis.origin.x)} {body.accuracy(localAxis.origin.y)} {body.accuracy(localAxis.origin.z)}";
            return movementOption + distanceFromGlobalOrigin + YXFromGlobalAxis + localAxisRotation + localOriginLocation;
        }
        public string saveJoint(bool radianOrAngle){
            float convert = radianOrAngle? 180f/Mathf.PI:1;
            string stringPath = $"Body_{body.worldKey}_Joint_{connection.indexInBody}_";
            string active = $"{stringPath}Active: {connection.active}\n";
            string showAxis = $"{stringPath}ShowAxis: {localAxis.renderAxis.created}\n";
            string keepBodyTogether = $"{stringPath}KeepBodyTogether: {this.keepBodyTogether}\n";
            string localAxisScale = $"{stringPath}LocalAxisScale: {body.accuracy(localAxis.axisDistance)}\n";
            string spinPastX = $"{stringPath}SpinPastX: {body.accuracy(localAxis.spinPast.angleX*convert)} {body.accuracy(localAxis.spinPast.sensitivitySpeedX*convert)} {body.accuracy(localAxis.spinPast.sensitivityAccelerationX)}\n";
            string spinPastY = $"{stringPath}SpinPastY: {body.accuracy(localAxis.spinPast.angleY*convert)} {body.accuracy(localAxis.spinPast.sensitivitySpeedY*convert)} {body.accuracy(localAxis.spinPast.sensitivityAccelerationY)}\n";
            string spinPastSpeedAndAcceleration = $"{stringPath}SpinPastSpeedAndAcceleration: {body.accuracy(localAxis.spinPast.speed*convert)} {body.accuracy(localAxis.spinPast.acceleration)}\n";
            string spinFutureX = $"{stringPath}SpinFutureX: {body.accuracy(localAxis.spinFuture.angleX*convert)} {body.accuracy(localAxis.spinFuture.sensitivitySpeedX*convert)} {body.accuracy(localAxis.spinFuture.sensitivityAccelerationX)}\n";
            string spinFutureY = $"{stringPath}SpinFutureY: {body.accuracy(localAxis.spinFuture.angleY*convert)} {body.accuracy(localAxis.spinFuture.sensitivitySpeedY*convert)} {body.accuracy(localAxis.spinFuture.sensitivityAccelerationY)}\n";
            string spinFutureSpeedAndAcceleration = $"{stringPath}SpinFutureSpeedAndAcceleration: {body.accuracy(localAxis.spinFuture.speed*convert)} {body.accuracy(localAxis.spinFuture.acceleration)}\n";
            string movePastX = $"{stringPath}MovePastX: {body.accuracy(localAxis.movePast.angleX*convert)} {body.accuracy(localAxis.movePast.sensitivitySpeedX*convert)} {body.accuracy(localAxis.movePast.sensitivityAccelerationX)}\n";
            string movePastY = $"{stringPath}MovePastY: {body.accuracy(localAxis.movePast.angleY*convert)} {body.accuracy(localAxis.movePast.sensitivitySpeedY*convert)} {body.accuracy(localAxis.movePast.sensitivityAccelerationY)}\n";
            string movePastSpeedAndAcceleration = $"{stringPath}MovePastSpeedAndAcceleration: {body.accuracy(localAxis.movePast.speed)} {body.accuracy(localAxis.movePast.acceleration)}\n";
            string moveFutureX = $"{stringPath}MoveFutureX: {body.accuracy(localAxis.moveFuture.angleX*convert)} {body.accuracy(localAxis.moveFuture.sensitivitySpeedX*convert)} {body.accuracy(localAxis.moveFuture.sensitivityAccelerationX)}\n";
            string moveFutureY = $"{stringPath}MoveFutureY: {body.accuracy(localAxis.moveFuture.angleY*convert)} {body.accuracy(localAxis.moveFuture.sensitivitySpeedY*convert)} {body.accuracy(localAxis.moveFuture.sensitivityAccelerationY)}\n";
            string moveFutureSpeedAndAcceleration = $"{stringPath}MoveFutureSpeedAndAcceleration: {body.accuracy(localAxis.moveFuture.speed)} {body.accuracy(localAxis.moveFuture.acceleration)}\n";
            string pastConnectionsInBody = $"{stringPath}PastConnectionsInBody: {connection.pastToString()}\n";
            string futureConnectionsInBody = $"{stringPath}FutureConnectionsInBody: {connection.futureToString()}\n";
            string resetPastJoints = $"{stringPath}ResetPastJoints: False\n";
            string resetFutureJoints = $"{stringPath}ResetFutureJoints: False\n";
            return active + showAxis + keepBodyTogether + localAxisScale +
                spinPastX + spinPastY + spinPastSpeedAndAcceleration +
                spinFutureX + spinFutureY + spinFutureSpeedAndAcceleration +
                movePastX + movePastY + movePastSpeedAndAcceleration +
                moveFutureX + moveFutureY + moveFutureSpeedAndAcceleration +
                pastConnectionsInBody + futureConnectionsInBody + 
                resetPastJoints + resetFutureJoints;
        }

        public void deleteJoint(){
            body.keyGenerator.returnKey();
            connection.disconnectAllFuture();
            connection.disconnectAllPast();
            pointCloud.deleteAllSpheres();
            localAxis.renderAxis.deleteAxis();
            body.bodyStructure[connection.indexInBody] = null;
        }
        public void distanceFromGlobalOrigin(float newDistance){
            Vector3 globalOrigin = body.globalAxis.origin;
            Vector3 localOrigin = localAxis.origin;
            float length = localAxis.length(localOrigin-globalOrigin);
            Vector3 direction = (length>0)? localAxis.direction(localOrigin,globalOrigin)*(newDistance-length): localAxis.direction(localAxis.y,globalOrigin)*(newDistance-length);
            moveJoint(direction);
        }

        public void moveJoint(Vector3 add){
            localAxis.moveAxis(add);
            pointCloud.moveSpheres(add);
        }
        public void rotateJoint(Vector4 quat, Vector3 rotationOrigin){
            localAxis.rotate(quat,rotationOrigin);
            localAxis.getWorldRotation();
            pointCloud.rotateAllSpheres(quat,rotationOrigin);
        }
        public void worldRotateJoint(float worldAngleY,float worldAngleX,float localAngleY){
            localAxis.setWorldRotationInRadians(worldAngleY,worldAngleX,localAngleY);
            pointCloud.resetAllSphereOrigins();
        }

        public void updatePhysics(){
            localAxis.spinFuture.updatePhysics(false);
            localAxis.spinPast.updatePhysics(false);
            localAxis.movePast.updatePhysics(true);
            localAxis.moveFuture.updatePhysics(true);
            pointCloud.updatePhysics();     
        }

        public void rotatePastHierarchy(){
            Vector4 quat = localAxis.spinFuture.quat(localAxis.spinPast.speed);
            rotateHierarchy(quat, false);
        }
        public void rotateFutureHierarchy(){
            Vector4 quat = localAxis.spinFuture.quat(localAxis.spinFuture.speed);
            rotateHierarchy(quat, true);
        }

        void rotateHierarchy(Vector4 quat, bool pastOrFuture){
            initTree(pastOrFuture, out List<Joint> tree, out int size); 
            Vector3 rotationOrigin = localAxis.origin;
            if (pastOrFuture) tree[0].rotateJoint(quat,rotationOrigin);
            for (int i = 1; i<size;i++){
                Joint joint = tree[i];
                List<Joint> joints = joint.connection.getAll();
                tree.AddRange(joints);
                size += joints.Count;
                joint.rotateJoint(quat,rotationOrigin);
            } 
            resetUsed(tree,size);
        }

        public void movePastHierarchy(){
            Vector3 move = localAxis.movePast.sphere.origin - localAxis.origin;
            moveHierarchy(move, false);
            if (keepBodyTogether) moveHierarchy(move, true);
        }
        public void moveFutureHierarchy(){
            Vector3 move = localAxis.moveFuture.sphere.origin - localAxis.origin;
            moveHierarchy(move, true);
            if (keepBodyTogether) moveHierarchy(move, false);
        }
        void moveHierarchy(Vector3 newVec, bool pastOrFuture){
            initTree(pastOrFuture, out List<Joint> tree, out int size);  
            if (pastOrFuture) tree[0].moveJoint(newVec);
            for (int i = 1; i<size;i++){
                Joint joint = tree[i];
                List<Joint> joints = joint.connection.getAll();
                tree.AddRange(joints);
                size += joints.Count;
                joint.moveJoint(newVec);
            }
            resetUsed(tree,size);
        }
        public void resetPastJoints(){
            initTree(false, out List<Joint> tree, out int size);  
            for (int i = 0; i<size;i++){
                Joint joint = tree[i];
                List<Joint> joints = joint.connection.getAll();
                tree.AddRange(joints);
                size += joints.Count;
                joint.connection.resetPastLockPositions();
            }
            resetUsed(tree,size);
        }
        public void resetFutureJoints(){
            initTree(true, out List<Joint> tree, out int size);  
            for (int i = 0; i<size;i++){
                Joint joint = tree[i];
                List<Joint> joints = joint.connection.getAll();
                tree.AddRange(joints);
                size += joints.Count;
                joint.connection.resetFutureLockPositions();
            }
            resetUsed(tree,size);
        }
        void initTree(bool pastOrFuture, out List<Joint> tree,out int size){
            tree = new List<Joint>{this};
            if (pastOrFuture) 
                    tree.AddRange(connection.getFuture()); 
                else 
                    tree.AddRange(connection.getPast()); 
            size = tree.Count;
        }
        internal void resetUsed(List<Joint> joints, int size){
            for (int i = 0; i<size;i++){
                joints[i].connection.used = false;
            }
        }
    }
    public class PointCloud {
        public Joint joint;
        public KeyGenerator keyGenerator;
        public CollisionSphere[] collisionSpheres;
        
        public PointCloud(){}
        public PointCloud(Joint joint, int amountOfSpheres){
            collisionSpheres = new CollisionSphere[amountOfSpheres];
            keyGenerator = new KeyGenerator(amountOfSpheres);
            this.joint = joint;
        }

        public string savePointCloud(out List<int> indexes, out int listSize){
            listSize = 0;
            int size = collisionSpheres.Length;
            indexes = new List<int>();
            string stringPath = $"Body_{joint.body.worldKey}_Joint_{joint.connection.indexInBody}_";
            string pointCloudSize = $"{stringPath}PointCloudSize: {size}\n";
            string allSpheresInJoint = $"{stringPath}AllSpheresInJoint: ";
            for (int i = 0; i<collisionSpheres.Length; i++){
                CollisionSphere collisionSphere = collisionSpheres[i];
                if (collisionSphere != null) {
                    allSpheresInJoint += $"{i} ";
                    indexes.Add(i);
                    listSize++;
                }
            }
            return pointCloudSize+allSpheresInJoint+"\n";
        }
        public void deleteSphere(int key){
            CollisionSphere remove = collisionSpheres[key];
            if(remove != null){
                keyGenerator.returnKey();
                collisionSpheres[key].aroundAxis.sphere.destroySphere();
                collisionSpheres[key] = null;
            }
        }
        public void deleteAllSpheres(){
            int size = collisionSpheres.Length;
            for (int i = 0; i<size;i++){
                deleteSphere(i);
            }
        }
        public void resetAllSphereOrigins(){
            int sphereCount = collisionSpheres.Length;
            for (int i = 0; i<sphereCount; i++){
                CollisionSphere collisionSphere = collisionSpheres[i];
                collisionSphere?.aroundAxis.resetOrigin();
            }
        }
        public void rotateAllSpheres(Vector4 quat, Vector3 rotationOrigin){
            int sphereCount = collisionSpheres.Length;
            for (int i = 0; i<sphereCount; i++){
                CollisionSphere collisionSphere = collisionSpheres[i];
                if (collisionSphere != null){
                    Vector3 vec = collisionSphere.aroundAxis.sphere.origin;
                    vec = joint.localAxis.quatRotate(vec,rotationOrigin,quat);
                    collisionSphere.setOrigin(vec);
                }
                collisionSphere?.aroundAxis.resetOrigin();
            }
        }
        public void moveSpheres(Vector3 move){
            int sphereCount = collisionSpheres.Length;
            for (int i = 0; i<sphereCount; i++){
                CollisionSphere collisionSphere = collisionSpheres[i];
                collisionSphere?.moveOrigin(move);
            }
        }
        public void updateAllSphereColors(Color color){
            int sphereCount = collisionSpheres.Length;
            for (int i = 0; i<sphereCount; i++){
                collisionSpheres[i]?.aroundAxis.sphere.updateColor(color);
            }
        }
        public void resetAllSphereColors(){
            int sphereCount = collisionSpheres.Length;
            for (int i = 0; i<sphereCount; i++){
                collisionSpheres[i]?.aroundAxis.sphere.resetColor();
            }
        }
        public void updatePhysics(){
            int sphereCount = collisionSpheres.Length;
            for (int i = 0; i<sphereCount; i++){
                collisionSpheres[i]?.aroundAxis.updatePhysics(false);
            }            
        }
        public List<CollisionSphere> arrayToList(){
            List<CollisionSphere> list = new List<CollisionSphere>();
            int sphereCount = collisionSpheres.Length;
            for (int i = 0; i<sphereCount; i++){
                CollisionSphere collisionSphere = collisionSpheres[i];
                if (collisionSphere != null){
                    list.Add(collisionSphere);
                }
            }
            return list;
        }
        public Dictionary<int,int> arraySizeManager(int amount){
            Dictionary<int,int> newKeys = new Dictionary<int,int>();
            if (amount > collisionSpheres.Length){
                resizeArray(amount);
            } else if (amount < collisionSpheres.Length){
                newKeys = optimizeCollisionSpheres();
            }
            return newKeys;
        }
        public void resizeArray(int maxKey){
            int limitCheck = collisionSpheres.Length - maxKey;
            if(limitCheck <= 0) {
                int newMax = collisionSpheres.Length + Mathf.Abs(limitCheck)+1;
                keyGenerator.generateKeys(Mathf.Abs(limitCheck)+1);
                CollisionSphere[] newCollisionSpheresArray = new CollisionSphere[newMax];
                for (int i = 0; i<collisionSpheres.Length; i++){
                    CollisionSphere joint = collisionSpheres[i];
                    if (joint != null){
                        newCollisionSpheresArray[i] = joint;
                    }
                }
                collisionSpheres = newCollisionSpheresArray;
            }
        }
        public Dictionary<int,int> optimizeCollisionSpheres(){
            int maxKeys = collisionSpheres.Length;
            int used = maxKeys - keyGenerator.availableKeys;
            CollisionSphere[] newCollision = new CollisionSphere[used];
            Dictionary<int,int> newKeys = new Dictionary<int,int>();
            int count = 0;
            for (int j = 0; j<maxKeys; j++){
                CollisionSphere collision = collisionSpheres[j];
                if (collision != null){
                    newKeys.Add(collision.path.collisionSphereKey,count);
                    collision.path.setCollisionSphereKey(count);
                    newCollision[count] = collision;
                    count++;
                }
            }
            collisionSpheres = newCollision;
            keyGenerator.resetGenerator();
            return newKeys;
        }
    }

    public class Path {
        public Body body;
        public Joint joint;
        public int collisionSphereKey;

        public Path(){}
        public Path(Body body, Joint joint, int collisionSphereKey){
            this.body=body;
            this.joint=joint;
            this.collisionSphereKey=collisionSphereKey;
        }

        public void setBody(Body body){
            this.body=body;
        }
        public void setJoint(Joint joint){
            this.joint = joint;
        }
        public void setCollisionSphereKey(int collisionSphereKey){
            this.collisionSphereKey = collisionSphereKey;
        }
    }
    public class BakedMeshIndex{
        public int indexInBakedMesh;
        public int indexInVertex;
        public BakedMeshIndex(){}
        public BakedMeshIndex(int indexInBakedMesh,int indexInVertex){
            this.indexInBakedMesh = indexInBakedMesh;
            this.indexInVertex = indexInVertex;
        }
    }
    public class CollisionSphere {
        public Path path;
        public AroundAxis aroundAxis;
        public BakedMeshIndex bakedMeshIndex;

        public CollisionSphere(){}
        public CollisionSphere(Joint joint, int sphereIndex){
            path = new Path(joint.body,joint,sphereIndex);
            aroundAxis = new AroundAxis(joint.localAxis,new Sphere());
            joint.pointCloud.keyGenerator.getKey();
        }
   
        public string saveCollisionSphere(bool radianOrAngle){
            Body body = path.body;
            Sphere sphere = aroundAxis.sphere;
            float convert = radianOrAngle? 180f/Mathf.PI:1;
            string stringPath = $"Body_{path.body.worldKey}_Joint_{path.joint.connection.indexInBody}_Sphere_{path.collisionSphereKey}_";
            string distanceFromLocalOrigin = $"{stringPath}DistanceFromLocalOrigin: {body.accuracy(aroundAxis.distance)} {body.accuracy(aroundAxis.speed)} {body.accuracy(aroundAxis.acceleration)}\n";
            string YFromLocalAxis = $"{stringPath}YFromLocalAxis: {body.accuracy(aroundAxis.angleY*convert)} {body.accuracy(aroundAxis.sensitivitySpeedY*convert)} {body.accuracy(aroundAxis.sensitivityAccelerationY)}\n";
            string XFromLocalAxis = $"{stringPath}XFromLocalAxis: {body.accuracy(aroundAxis.angleX*convert)} {body.accuracy(aroundAxis.sensitivitySpeedX*convert)} {body.accuracy(aroundAxis.sensitivityAccelerationX)}\n";
            string radius = $"{stringPath}Radius: {body.accuracy(sphere.radius)}\n";
            string color = $"{stringPath}ColorRGBA: {body.accuracy(sphere.color.r)} {body.accuracy(sphere.color.g)} {body.accuracy(sphere.color.b)} {body.accuracy(sphere.color.a)}\n";
            return distanceFromLocalOrigin + YFromLocalAxis + XFromLocalAxis + radius + color;
        }
        public void setOrigin(Vector3 newOrigin){
            aroundAxis.sphere.setOrigin(newOrigin);
        }
        public void moveOrigin(Vector3 newOrigin){
            aroundAxis.sphere.moveOrigin(newOrigin);
        }
        public void setRadius(float newRadius){
            aroundAxis.sphere.setRadius(newRadius);
        }
        public void updatePhysics(){
            aroundAxis.updatePhysics(false);
        }
    }
    public class Sphere{
        public float radius;
        public Vector3 origin;
        public Color color;
        public GameObject sphere;
        
        public Sphere(){
            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.GetComponent<Collider>().enabled = false;
            setOrigin(new Vector3(0,0,0));
            setRadius(1);
            setColor(new Color(1,1,1,1));
            updateColor(color);
        }
        public Sphere(Vector3 origin, float radius, Color color){
            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.GetComponent<Collider>().enabled = false;
            setOrigin(origin);
            setRadius(radius);
            setColor(color);
            updateColor(color);
        }
        public void setOrigin(Vector3 newOrigin){
            origin = newOrigin;
            sphere.transform.position = newOrigin;
        }
        public void moveOrigin(Vector3 newOrigin){
            origin += newOrigin;
            sphere.transform.position += newOrigin;
        }
        public void setRadius(float newRadius){
            radius = newRadius;
            sphere.transform.localScale = new Vector3(radius, radius, radius);
        }
        public void setColor(Color newColor){
            color = newColor;
        }
        public void updateColor(Color newColor){
            sphere.GetComponent<Renderer>().material.color = newColor;
        }
        public void resetColor(){
            sphere.GetComponent<Renderer>().material.color = color;
        }  
        public void restoreSphere(){
            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            setOrigin(origin);
            setRadius(radius);
            setColor(color);
            updateColor(color);
        }     
        public void destroySphere(){
            Destroy(sphere);
        }
    }

    public class Editor {
        bool radianOrDegree = false;
        bool initilize;
        Body body;
        Dictionary<int,int> newJointKeys = new Dictionary<int,int>();
        Dictionary<int,int> newSphereKeys = new Dictionary<int,int>();
        Dictionary<int,List<int>> deleted = new Dictionary<int, List<int>>();
        public Editor(){}
        public Editor(Body body){
            this.body = body;
        }
        
        internal void writer(){
            using(StreamWriter writetext = new StreamWriter($"Assets/v4/{body.worldKey}.txt")) {
                writetext.WriteLine(body.saveBody());
                int size = body.bodyStructure.Length;
                for (int i = 0; i<size; i++){
                    Joint joint = body.bodyStructure[i];
                    if (joint != null){
                        writetext.WriteLine(joint.saveJointPosition(radianOrDegree));
                        writetext.WriteLine(joint.pointCloud.savePointCloud(out List<int> collisionSphereIndexes, out int listSize));
                        CollisionSphere[] collisionSpheres = joint.pointCloud.collisionSpheres;
                        for (int j = 0; j<listSize;j++){
                            writetext.WriteLine(collisionSpheres[collisionSphereIndexes[j]].saveCollisionSphere(radianOrDegree));
                        }
                    }
                }
                for (int i = 0; i<size; i++){
                    Joint joint = body.bodyStructure[i];
                    if (joint != null){
                        writetext.WriteLine(joint.saveJoint(radianOrDegree));
                    }
                }
                writetext.WriteLine(body.saveBodyPosition(radianOrDegree));
                writetext.WriteLine("");
            }
        }
        internal void reader(){
            using(StreamReader readtext = new StreamReader($"Assets/v4/{body.worldKey}.txt")){
                string readText;
                while ((readText = readtext.ReadLine()) != null){
                    string[] splitStr = readText.Split(":");
                    if (splitStr.Length == 2){
                        removeEmpty(splitStr[0].Split("_"), out List<string> instruction);
                        removeEmpty(splitStr[1].Split(" "), out List<string> values);
                        switch (instruction.Count){
                            case 3:
                                bodyInstructions(instruction[2],values);
                            break;
                            case 5:
                                jointInstructions(instruction[3],instruction[4],values);
                            break;
                            case 7:
                                sphereInstructions(instruction[3],instruction[5],instruction[6],values);
                            break;
                        }
                    }
                } 
            }
            newJointKeys = new Dictionary<int,int>();
            newSphereKeys = new Dictionary<int,int>();
            deleted = new Dictionary<int, List<int>>();
        }
        public void initilizeBody(){
            initilize = true;
            reader();
            initilize = false;
        }
        public void readWrite(){
            if (body.count == 0){
                reader();
                body.updatePhysics();
                writer();
                body.count = body.countStart;
            } else {
                body.count -=1;
            }
        }
        void removeEmpty(string[] strArray, out List<string> list){
            list = new List<string>();
            int arraySize = strArray.Length;
            for (int i = 0; i < arraySize; i++){
                string str = strArray[i];
                if (str != "") list.Add(str);
            }
        }
        Joint getJoint(string jointKey){
            bool checkKey = int.TryParse(jointKey, out int key);
            if (newJointKeys.TryGetValue(key, out int newKey)){
                key = newKey;
            }
            return checkKey? body.bodyStructure[key]:null;
        }
        CollisionSphere getCollisionSphere(Joint joint,string collisionSphereKey){
            bool checkKey = int.TryParse(collisionSphereKey, out int key);
            if (newSphereKeys.TryGetValue(key, out int newKey)){
                key = newKey;
            }
            return checkKey?joint.pointCloud.collisionSpheres[key]:null;
        }

        const string bodyStructureSize = "BodyStructureSize";
        const string updateReadWrite = "UpdateReadWrite";
        const string allJointsInBody = "AllJointsInBody";
        const string globalAxisScale = "GlobalAxisScale";
        const string globalOriginLocation = "GlobalOriginLocation";
        const string globalAxisRotationXYZ = "GlobalAxisRotationXYZ";
        const string accuracy = "Accuracy";
        const string radianOrAngle = "RadianOrAngle";
        public void bodyInstructions(string instruction, List<string> value){
            switch (instruction){
                case bodyStructureSize:
                    bodyStructureSizeInstruction(value);
                break;
                case updateReadWrite:
                    updateReadWriteInstruction(value);
                break;
                case showAxis:
                    showAxisInstruction(value);
                break;
                case globalAxisScale:
                    globalAxisScaleInstruction(value);
                break;
                case allJointsInBody:
                    allJointsInBodyInstruction(value);
                break;
                case globalOriginLocation:
                    globalOriginLocationInstruction(value);
                break;
                case globalAxisRotationXYZ:
                    globalAxisRotationXYZInstruction(value);
                break;
                case accuracy:
                    accuracyInstruction(value);
                break;
                case radianOrAngle:
                    radianOrAngleInstruction(value);
                break;
            }
        }
        void bodyStructureSizeInstruction(List<string> value){
            if (value.Count>0){
                bool check = int.TryParse(value[0], out int amount);
                if (check) newJointKeys = body.arraySizeManager(amount); 
            }
        }
        void updateReadWriteInstruction(List<string> value){
            if (value.Count>0){
                bool check = int.TryParse(value[0], out int amount);
                if (check) body.countStart = amount; 
            }        
        }

        void gatherBodyData(List<string> value,out bool error, out int maxKey, out int nullCount, out HashSet<int> set,out List<int> nullKeys){
            set = new HashSet<int>();
            nullKeys = new List<int>();
            nullCount = 0;
            int size = value.Count;
            maxKey = 0;
            error = false;
            bool check;
            for (int i = 0; i < size; i++){
                check = int.TryParse(value[i], out int key);
                if (check){
                    if (newJointKeys.TryGetValue(key, out int newKey)){
                        key = newKey;
                    }
                    if (!set.Contains(key)){
                        set.Add(key);
                        if (key >= body.bodyStructure.Length){
                            nullKeys.Add(key);
                            nullCount++;
                        } else if (body.bodyStructure[i] == null){
                            nullKeys.Add(key);
                            nullCount++;
                        } 
                        if (key > maxKey) maxKey = key;
                    }
                } else if (!error && !check) error = true;
            }
        }
        HashSet<int> resizeBody(List<string> value, out bool error){
            gatherBodyData(value,out error, out int maxKey, out int nullCount, out HashSet<int> set,out List<int> nullKeys);
            if (maxKey>=body.bodyStructure.Length) body.resizeArray(maxKey);
            for (int i = 0; i < nullCount; i++){
                if (body.bodyStructure[nullKeys[i]] == null)
                    body.bodyStructure[nullKeys[i]] = new Joint(body,nullKeys[i]);
            }
            return set;
        }
        void showAxisInstruction(List<string> value){
            if (value.Count>0){
                if (value[0] == "True") 
                    body.globalAxis.renderAxis.createAxis();
                else if (value[0] == "False") 
                    body.globalAxis.renderAxis.deleteAxis();
            }
        }
        void globalAxisScaleInstruction(List<string> value){
            if (value.Count>0){
                bool checkScale = float.TryParse(value[0], out float newScale);
                if (checkScale && Mathf.Abs(body.globalAxis.axisDistance-newScale)>0){
                    body.globalAxis.scaleAxis(newScale);    
                    body.globalAxis.spinFuture.scale(newScale);
                    body.globalAxis.spinPast.scale(newScale);
                }

            }
        }
        void allJointsInBodyInstruction(List<string> value){
            HashSet<int> set = resizeBody(value, out bool error);
            if (!error) {
                for (int i = 0; i< body.bodyStructure.Length; i++){
                    if (!set.Contains(i)) body.bodyStructure[i]?.deleteJoint();        
                }
            }
        }
        void globalOriginLocationInstruction(List<string> value){
            int size = value.Count;
            if (size >= 3) {
                bool checkX = float.TryParse(value[0], out float x);
                bool checkY = float.TryParse(value[1], out float y);
                bool checkZ = float.TryParse(value[2], out float z);
                float vecX = checkX? x: body.globalAxis.origin.x;
                float vecY = checkY? y: body.globalAxis.origin.y;
                float vecZ = checkZ? z: body.globalAxis.origin.z;
                body.globalAxis.placeAxis(new Vector3(vecX,vecY,vecZ));
            }
        }
        void globalAxisRotationXYZInstruction(List<string> value){
            int size = value.Count;
            if (size >= 3) {
                bool checkY = float.TryParse(value[0], out float y);
                bool checkX = float.TryParse(value[1], out float x);
                bool checkLY = float.TryParse(value[2], out float ly);
                if (checkX || checkY || checkLY){
                    if (!initilize){
                        float worldAngleY = Mathf.Abs(y - body.globalAxis.worldAngleY) % (2*Mathf.PI);
                        float worldAngleX = Mathf.Abs(x - body.globalAxis.worldAngleX) % (2*Mathf.PI);
                        float localAngleY = Mathf.Abs(ly - body.globalAxis.localAngleY) % (2*Mathf.PI);
                        float limit = 0.0001f;
                        if (worldAngleY > limit || worldAngleX> limit || localAngleY > limit) {
                            if (radianOrDegree) {
                                y*= Mathf.PI/180f;
                                x*= Mathf.PI/180f;
                                ly*= Mathf.PI/180f;
                            }
                            body.rotateBody(y,x,ly);
                        }
                    } else {
                        if (y<0) body.globalAxis.worldAngleY = y;
                        body.globalAxis.setWorldRotation(y,x,ly);
                    }
                }
            }            
        }
        void accuracyInstruction(List<string> value){
            if (value.Count>0){
                bool checkAccuracy = int.TryParse(value[0], out int newAccuracy);
                if (checkAccuracy){
                    body.newAccuracy(newAccuracy);
                }
            }
        }
        void radianOrAngleInstruction(List<string> value){
            int size = value.Count;
            if (size>= 1){
                if (value[0] == "True") radianOrDegree = true; 
                else 
                if (value[0] == "False") radianOrDegree = false;
            }
        }

        const string active = "Active";
        const string showAxis = "ShowAxis";
        const string keepBodyTogether = "KeepBodyTogether";
        const string localAxisScale = "LocalAxisScale";
        const string movementOption = "MovementOption";
        const string distanceFromGlobalOrigin = "DistanceFromGlobalOrigin";
        const string YXFromGlobalAxis = "YXFromGlobalAxis";
        const string localAxisRotation = "LocalAxisRotation";
        const string localOriginLocation = "LocalOriginLocation";
        const string spinPastX = "SpinPastX";
        const string spinPastY = "SpinPastY";
        const string spinPastSpeedAndAcceleration = "SpinPastSpeedAndAcceleration";
        const string spinFutureX = "SpinFutureX";
        const string spinFutureY = "SpinFutureY";
        const string spinFutureSpeedAndAcceleration = "SpinFutureSpeedAndAcceleration";
        const string movePastX = "MovePastX";
        const string movePastY = "MovePastY";
        const string movePastSpeedAndAcceleration = "MovePastSpeedAndAcceleration";
        const string moveFutureX = "MoveFutureX";
        const string moveFutureY = "MoveFutureY";
        const string moveFutureSpeedAndAcceleration = "MoveFutureSpeedAndAcceleration";
        const string pastConnectionsInBody = "PastConnectionsInBody";
        const string futureConnectionsInBody = "FutureConnectionsInBody";
        const string resetPastJoints = "ResetPastJoints";
        const string resetFutureJoints = "ResetFutureJoints";
        const string pointCloudSize = "PointCloudSize";
        const string allSpheresInJoint = "AllSpheresInJoint";
        public void jointInstructions(string jointKey, string instruction, List<string> value){
            Joint joint = getJoint(jointKey);
            if (joint != null) {
                switch (instruction){
                    case active:
                        activeInstruction(joint,value);
                    break;
                    case showAxis:
                        showAxisInstruction(joint,value);
                    break;
                    case keepBodyTogether:
                        keepBodyTogetherInstruction(joint,value);
                    break;
                    case localAxisScale:
                        localAxisScaleInstruction(joint,value);
                    break;
                    case movementOption:
                        movementOptionInstruction(joint,value);
                    break;
                    case distanceFromGlobalOrigin:
                        distanceFromGlobalOriginInstruction(joint,value);
                    break;
                    case YXFromGlobalAxis:
                        YXFromGlobalAxisInstruction(joint,value);
                    break;
                    case localAxisRotation:
                        localAxisRotationInstruction(joint,value);
                    break;
                    case localOriginLocation:
                        localOriginLocationInstruction(joint,value);
                    break;
                    case spinPastX:
                        spinPastXInstruction(joint,value);
                    break;
                    case spinPastY:
                        spinPastYInstruction(joint,value);
                    break;
                    case spinPastSpeedAndAcceleration:
                        spinPastSpeedAndAccelerationInstruction(joint,value);
                    break;
                    case spinFutureX:
                        spinFutureXInstruction(joint,value);
                    break;
                    case spinFutureY:
                        spinFutureYInstruction(joint,value);
                    break;
                    case spinFutureSpeedAndAcceleration:
                        spinFutureSpeedAndAccelerationInstruction(joint,value);
                    break;
                    case movePastX:
                        movePastXInstruction(joint,value);
                    break;
                    case movePastY:
                        movePastYInstruction(joint,value);
                    break;
                    case movePastSpeedAndAcceleration:
                        movePastSpeedAndAccelerationInstruction(joint,value);
                    break;
                    case moveFutureX:
                        moveFutureXInstruction(joint,value);
                    break;
                    case moveFutureY:
                        moveFutureYInstruction(joint,value);
                    break;
                    case moveFutureSpeedAndAcceleration:
                        moveFutureSpeedAndAccelerationInstruction(joint,value);
                    break;                  
                    case pastConnectionsInBody:
                        pastConnectionsInBodyInstruction(joint,value);
                    break;
                    case futureConnectionsInBody:
                        futureConnectionsInBodyInstruction(joint,value);
                    break;
                    case resetPastJoints:
                        resetPastJointsInstruction(joint,value);
                    break;
                    case resetFutureJoints:
                        resetFutureJointsInstruction(joint,value);
                    break;
                    case pointCloudSize:
                        pointCloudSizeInstruction(joint,value);
                    break;
                    case allSpheresInJoint:
                        allSpheresInJointInstruction(joint,value);
                    break;
                }
            }
        }

        void activeInstruction(Joint joint,List<string> value){
            if (value.Count>0){
                joint.connection.active = value[0] == "True";
            }
        }
        void showAxisInstruction(Joint joint,List<string> value){
            if (value.Count>0){
                if (value[0] == "True") 
                    joint.localAxis.renderAxis.createAxis();
                else if (value[0] == "False") 
                    joint.localAxis.renderAxis.deleteAxis();
            }
        }
        void keepBodyTogetherInstruction(Joint joint,List<string> value){
            if (value.Count>0){
                if (value[0] == "True") 
                    joint.keepBodyTogether = true;
                else if (value[0] == "False") 
                    joint.keepBodyTogether = false;
            } 
        }
        void localAxisScaleInstruction(Joint joint,List<string> value){
            if (value.Count>0){
                bool checkScale = float.TryParse(value[0], out float newScale);
                if (checkScale && Mathf.Abs(joint.localAxis.axisDistance-newScale)>0){
                    joint.localAxis.scaleAxis(newScale);
                    joint.localAxis.spinFuture.scale(newScale);
                    joint.localAxis.spinPast.scale(newScale);
                }
            }
        }
        void movementOptionInstruction(Joint joint,List<string> value){
            if (value.Count>0){
                if (value[0] == "True") 
                    joint.movementOption = true;
                else if (value[0] == "False") 
                    joint.movementOption = false;
            }
        }
        void distanceFromGlobalOriginInstruction(Joint joint,List<string> value){
            if (value.Count>0){
                bool checkStr = float.TryParse(value[0], out float distance);
                if (checkStr && !joint.movementOption){
                    joint.distanceFromGlobalOrigin(distance);
                }
            }
        }
        void YXFromGlobalAxisInstruction(Joint joint,List<string> value){
            if (value.Count>=2){
                bool checkY = float.TryParse(value[0], out float y);
                bool checkX = float.TryParse(value[1], out float x);
                Axis globalAxis = joint.body.globalAxis;
                Axis localAxis = joint.localAxis;
                float length = localAxis.length(localAxis.origin-globalAxis.origin);
                if (checkY && checkX && !joint.movementOption) {
                        joint.fromGlobalAxisY = y;
                        joint.fromGlobalAxisX = x;
                        if (radianOrDegree) {
                            y*= Mathf.PI/180f;
                            x*= Mathf.PI/180f;
                            joint.fromGlobalAxisY*= Mathf.PI/180f;
                            joint.fromGlobalAxisX*= Mathf.PI/180f;
                        }
                        joint.moveJoint(globalAxis.setPointAroundOrigin(y,x,length) - localAxis.origin);
                    } 
            }
        }
        void localAxisRotationInstruction(Joint joint,List<string> value){
            if (value.Count>=3){
                bool checkY = float.TryParse(value[0], out float y);
                bool checkX = float.TryParse(value[1], out float x);
                bool checkLY = float.TryParse(value[2], out float ly);
                if (checkY && checkX && checkLY) {
                    if (radianOrDegree) {
                        y*= Mathf.PI/180f;
                        x*= Mathf.PI/180f;
                        ly*= Mathf.PI/180f;
                    }
                    joint.worldRotateJoint(y,x,ly);
                }
            }
        }
        void localOriginLocationInstruction(Joint joint,List<string> value){
            int size = value.Count;
            if (size >= 3) {
                bool checkX = float.TryParse(value[0], out float x);
                bool checkY = float.TryParse(value[1], out float y);
                bool checkZ = float.TryParse(value[2], out float z);
                if (checkX && checkY && checkZ && joint.movementOption) {
                    Vector3 add = initilize? new Vector3(0,0,0):joint.localAxis.origin;
                    joint.moveJoint(new Vector3(x,y,z)-add);
                }
            }
        }
        void xAroundAxis(AroundAxis aroundAxis, List<string> value){
            if (value.Count>=3){
                bool checkX = float.TryParse(value[0], out float angleX);
                bool checkSpeedX = float.TryParse(value[1], out float speedX);
                bool checkAccelerationX = float.TryParse(value[2], out float accelerationX);
                if (checkX && checkSpeedX && checkAccelerationX){
                    if (radianOrDegree) {
                        angleX*= Mathf.PI/180f;
                        speedX*= Mathf.PI/180f;
                    }
                    aroundAxis.sensitivitySpeedX = speedX;
                    aroundAxis.sensitivityAccelerationX = accelerationX;
                    if (angleX != aroundAxis.angleX) aroundAxis.setInRadians(aroundAxis.angleY,angleX);  
                    float direction = aroundAxis.sensitivitySpeedX*aroundAxis.sensitivityAccelerationX;  
                    if (Mathf.Abs(direction) > 0) aroundAxis.rotationX();
                }
            }
        }
        void yAroundAxis(AroundAxis aroundAxis, List<string> value){
            if (value.Count>=3){
                bool checkY = float.TryParse(value[0], out float angleY);
                bool checkSpeedY = float.TryParse(value[1], out float speedY);
                bool checkAccelerationY = float.TryParse(value[2], out float accelerationY);    
                if (checkY && checkSpeedY && checkAccelerationY){ 
                    if (radianOrDegree) {
                        angleY*= Mathf.PI/180f;
                        speedY*= Mathf.PI/180f;
                    }
                    aroundAxis.sensitivitySpeedY = speedY;
                    aroundAxis.sensitivityAccelerationY = accelerationY;
                    if (angleY != aroundAxis.angleY) aroundAxis.setInRadians(angleY,aroundAxis.angleX); 
                    float direction = aroundAxis.sensitivitySpeedY*aroundAxis.sensitivityAccelerationY;              
                    if (Mathf.Abs(direction) > 0) aroundAxis.rotationY();
                }
            }
        }

        void spinPastXInstruction(Joint joint,List<string> value){
            xAroundAxis(joint.localAxis.spinPast,value);
        }
        void spinPastYInstruction(Joint joint,List<string> value){
            yAroundAxis(joint.localAxis.spinPast,value);
        }
        void spinPastSpeedAndAccelerationInstruction(Joint joint,List<string> value){
            if (value.Count>=2){
                AroundAxis aroundAxis = joint.localAxis.spinPast;
                bool checkSpeed = float.TryParse(value[0], out float speed);
                bool checkAcceleration = float.TryParse(value[1], out float acceleration);  
                if (checkSpeed && checkAcceleration) {
                    if (radianOrDegree) {
                        speed*= Mathf.PI/180f;
                    }  
                    aroundAxis.speed = speed;
                    aroundAxis.acceleration = acceleration;
                    joint.rotatePastHierarchy();
                }
            }
        }

        void spinFutureXInstruction(Joint joint,List<string> value){
            xAroundAxis(joint.localAxis.spinFuture,value);
        }
        void spinFutureYInstruction(Joint joint,List<string> value){
            yAroundAxis(joint.localAxis.spinFuture,value);
        }
        void spinFutureSpeedAndAccelerationInstruction(Joint joint,List<string> value){
            if (value.Count>=2){
                AroundAxis aroundAxis = joint.localAxis.spinFuture;
                bool checkSpeed = float.TryParse(value[0], out float speed);
                bool checkAcceleration = float.TryParse(value[1], out float acceleration);  
                if (checkSpeed && checkAcceleration) {
                    if (radianOrDegree) {
                        speed*= Mathf.PI/180f;
                    }
                    aroundAxis.speed = speed;
                    aroundAxis.acceleration = acceleration;
                    joint.rotateFutureHierarchy();
                }
            }
        }
        void movePastXInstruction(Joint joint,List<string> value){
            xAroundAxis(joint.localAxis.movePast,value);
        }
        void movePastYInstruction(Joint joint,List<string> value){
            yAroundAxis(joint.localAxis.movePast,value);
        }
        void movePastSpeedAndAccelerationInstruction(Joint joint,List<string> value){
            if (value.Count>=2){
                AroundAxis aroundAxis = joint.localAxis.movePast;
                bool checkSpeed = float.TryParse(value[0], out float speed);
                bool checkAcceleration = float.TryParse(value[1], out float acceleration);  
                if (checkSpeed && checkAcceleration){
                    if (speed*acceleration <0) {
                        aroundAxis.angleY = (Mathf.PI+aroundAxis.angleY)%(2*Mathf.PI);
                    };    
                    aroundAxis.speed = Mathf.Abs(speed);
                    aroundAxis.acceleration = Mathf.Abs(acceleration);
                    if (Mathf.Abs(speed*acceleration)>0) joint.movePastHierarchy();
                }
            }
        }
        void moveFutureXInstruction(Joint joint,List<string> value){
            xAroundAxis(joint.localAxis.moveFuture,value);
        }
        void moveFutureYInstruction(Joint joint,List<string> value){
            yAroundAxis(joint.localAxis.moveFuture,value);
        }
        void moveFutureSpeedAndAccelerationInstruction(Joint joint,List<string> value){
            if (value.Count>=2){
                AroundAxis aroundAxis = joint.localAxis.moveFuture;
                bool checkSpeed = float.TryParse(value[0], out float speed);
                bool checkAcceleration = float.TryParse(value[1], out float acceleration);  
                if (checkSpeed && checkAcceleration){    
                    if (speed*acceleration <0) {
                        aroundAxis.angleY = (Mathf.PI+aroundAxis.angleY)%(2*Mathf.PI);
                    };  
                    aroundAxis.speed = Mathf.Abs(speed);
                    aroundAxis.acceleration = Mathf.Abs(acceleration);
                    if (Mathf.Abs(speed*acceleration)>0) joint.moveFutureHierarchy();
                }
            }
        }
        void pastConnectionsInBodyInstruction(Joint joint,List<string> value){
            HashSet<int> set = resizeBody(value, out _);
            List<LockedConnection> past = joint.connection.past;
            int size = past.Count;
            List<LockedConnection> newPast = new List<LockedConnection>();
            bool checkDelete = deleted.TryGetValue(joint.connection.indexInBody,out List<int> delete);
            if (checkDelete){
                int deletSize = delete.Count;
                for (int i = 0; i < deletSize; i++){
                    set.Remove(delete[i]);
                }
            }
            bool checkChange = false;
            for (int i = 0; i< size; i++){
                int index = past[i].joint.connection.indexInBody;
                if (!set.Contains(index) ) {
                    joint.connection.disconnectPast(i);
                    checkDelete = deleted.TryGetValue(index,out delete);
                    if (checkDelete){
                        delete.Add(joint.connection.indexInBody);
                    } else {
                        deleted[index] = new List<int>(){joint.connection.indexInBody};
                    }
                    checkChange = true;
                } else {
                    newPast.Add(past[i]);
                    set.Remove(index);
                    }
            }
            if (checkChange || size != set.Count){
                foreach (int i in set){
                    if (i != joint.connection.indexInBody) {
                        Joint newjoint = joint.body.bodyStructure[i];
                        if (newjoint == null) newjoint = new Joint(body,i);  
                        newjoint.connection.connectThisFutureToPast(joint, out _, out LockedConnection lockOther);
                        newPast.Add(lockOther);
                    }
                }
                joint.connection.past = newPast;
            }
        }
        void futureConnectionsInBodyInstruction(Joint joint,List<string> value){
            HashSet<int> set = resizeBody(value, out _);
            List<LockedConnection> future = joint.connection.future;
            int size = future.Count;
            List<LockedConnection> newFuture = new List<LockedConnection>();
            bool checkDelete = deleted.TryGetValue(joint.connection.indexInBody,out List<int> delete);
            if (checkDelete){
                int deletSize = delete.Count;
                for (int i = 0; i < deletSize; i++){
                    set.Remove(delete[i]);
                }
            }
            bool checkChange = false;
            for (int i = 0; i< size; i++){
                int index = future[i].joint.connection.indexInBody;
                if (!set.Contains(index)) {
                    joint.connection.disconnectFuture(i);
                    checkDelete = deleted.TryGetValue(index,out delete);
                    if (checkDelete){
                        delete.Add(joint.connection.indexInBody);
                    } else {
                        deleted[index] = new List<int>(){joint.connection.indexInBody};
                    }
                    checkChange = true;
                } else {
                    newFuture.Add(future[i]);
                    set.Remove(index);
                    }
            }
            if (checkChange || size != set.Count){
                foreach (int i in set){
                    if (i != joint.connection.indexInBody) {
                        Joint newjoint = joint.body.bodyStructure[i];
                        if (newjoint == null) newjoint = new Joint(body,i);
                        newjoint.connection.connectThisPastToFuture(joint, out _, out LockedConnection lockOther);
                        newFuture.Add(lockOther);
                    }
                }
                joint.connection.future = newFuture;
            }
        }
        void resetPastJointsInstruction(Joint joint,List<string> value){
            if (value.Count >0){
                if (value[0]== "True") joint.resetPastJoints();
            }
        }
        void resetFutureJointsInstruction(Joint joint,List<string> value){
            if (value.Count >0){
                if (value[0]== "True") joint.resetFutureJoints();
            }
        }
        void pointCloudSizeInstruction(Joint joint,List<string> value){
            if (value.Count> 0){
                bool check = int.TryParse(value[0], out int amount);
                if (check) newSphereKeys = joint.pointCloud.arraySizeManager(amount);
            }
        }

        void gatherJointData(Joint joint,List<string> value,out bool error, out int maxKey, out int nullCount, out HashSet<int> set,out List<int> nullKeys){
            set = new HashSet<int>();
            nullKeys = new List<int>();
            nullCount = 0;
            int size = value.Count;
            maxKey = 0;
            error = false;
            bool check;
            PointCloud pointCloud = joint.pointCloud;
            for (int i = 0; i < size; i++){
                check = int.TryParse(value[i], out int key);
                if (check){
                    if (newSphereKeys.TryGetValue(key, out int newKey)){
                        key = newKey;
                    }
                    if (!set.Contains(key)){
                        set.Add(key);
                        if (key >= pointCloud.collisionSpheres.Length){
                            nullKeys.Add(key);
                            nullCount++;
                        } else if (pointCloud.collisionSpheres[i] == null){
                            nullKeys.Add(key);
                            nullCount++;
                        } 
                        if (key > maxKey) maxKey = key;
                    }
                } else if (!error && !check) error = true;
            }
        }
        HashSet<int> resizePointCloud(Joint joint,List<string> value, out bool error){
            gatherJointData(joint, value,out error, out int maxKey, out int nullCount, out HashSet<int> set,out List<int> nullKeys);
            if (maxKey>=joint.pointCloud.collisionSpheres.Length) joint.pointCloud.resizeArray(maxKey);
            for (int i = 0; i < nullCount; i++){
                if (joint.pointCloud.collisionSpheres[nullKeys[i]] == null)
                    joint.pointCloud.collisionSpheres[nullKeys[i]] = new CollisionSphere(joint,nullKeys[i]);
            }
            return set;
        }
        void allSpheresInJointInstruction(Joint joint,List<string> value){
            HashSet<int> set = resizePointCloud(joint,value, out bool error);
            if (!error) {
                for (int i = 0; i< joint.pointCloud.collisionSpheres.Length; i++){
                    if (!set.Contains(i)) joint.pointCloud?.deleteSphere(i);        
                }
            }
        }

        const string distanceFromLocalOrigin = "DistanceFromLocalOrigin";
        const string XFromLocalAxis = "XFromLocalAxis";
        const string YFromLocalAxis = "YFromLocalAxis";
        const string radius = "Radius";
        const string colorRGBA = "ColorRGBA";
        public void sphereInstructions(string jointKey,string collisionSphereKey, string instruction, List<string> value){
            Joint joint = getJoint(jointKey);
            if (joint != null) { 
                CollisionSphere collisionSphere = getCollisionSphere(joint,collisionSphereKey);
                if (collisionSphere != null){
                    switch (instruction){
                        case distanceFromLocalOrigin:  
                            distanceFromLocalAxisInstruction(collisionSphere, value);
                        break;
                        case YFromLocalAxis:
                            YFromLocalAxisInstruction(collisionSphere,value);
                        break;
                        case XFromLocalAxis:
                            XFromLocalAxisInstruction(collisionSphere,value);
                        break;
                        case radius:
                            radiusInstruction(collisionSphere,value);
                        break;
                        case colorRGBA:
                            colorRGBAInstruction(collisionSphere,value);
                        break;
                    }
                }
            }
        }
        void distanceFromLocalAxisInstruction(CollisionSphere collisionSphere, List<string> value){
            if (value.Count>= 1){
                bool checkDistance = float.TryParse(value[0], out float distance);
                bool checkSpeed = float.TryParse(value[1], out float speed);
                bool checkAcceleration = float.TryParse(value[2], out float acceleration);
                if (checkDistance && checkSpeed && checkAcceleration){
                    collisionSphere.aroundAxis.acceleration = acceleration;  
                    collisionSphere.aroundAxis.speed = speed;
                    collisionSphere.aroundAxis.distance = distance+speed;
                    collisionSphere.aroundAxis.scale(distance+speed);
                }
            }
        }
        void XFromLocalAxisInstruction(CollisionSphere collisionSphere, List<string> value){
            xAroundAxis(collisionSphere.aroundAxis,value);
        }
        void YFromLocalAxisInstruction(CollisionSphere collisionSphere, List<string> value){
            yAroundAxis(collisionSphere.aroundAxis,value);
        }
        void radiusInstruction(CollisionSphere collisionSphere, List<string> value){
            if (value.Count>0){
                bool checkRadius = float.TryParse(value[0], out float radius);
                if (checkRadius) collisionSphere.aroundAxis.sphere.setRadius(radius);
            }
        }
        void colorRGBAInstruction(CollisionSphere collisionSphere, List<string> value){
            if (value.Count>=4){
                Sphere sphere = collisionSphere.aroundAxis.sphere;
                bool checkR = float.TryParse(value[0], out float r);
                bool checkG = float.TryParse(value[1], out float g);
                bool checkB= float.TryParse(value[2], out float b);
                bool checkA = float.TryParse(value[3], out float a);
                if (checkR) sphere.color.r = r;
                if (checkG) sphere.color.g = g;
                if (checkB) sphere.color.b = b;
                if (checkA) sphere.color.a = a;
                sphere.resetColor();
            }
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
