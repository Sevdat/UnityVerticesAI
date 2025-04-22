using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        public float radius = 0.1f;

        public RenderAxis(Axis axis){
            this.axis = axis;
            origin = new Sphere(axis.origin,radius,new Color(1,1,1,0));
            x = new Sphere(axis.x,radius,new Color(1,0,0,0));
            y = new Sphere(axis.y,radius,new Color(0,1,0,0));
            z = new Sphere(axis.z,radius,new Color(0,0,1,0));
            spinPast = new Sphere(y.origin,radius,new Color(1,0,1,1));
            spinFuture = new Sphere(y.origin,radius,new Color(0,0,0,0));
            movePast = new Sphere(axis.origin,radius,new Color(1.0f, 0.64f, 0.0f,0.0f));
            moveFuture = new Sphere(axis.origin,radius,new Color(1.0f, 0.64f, 0.0f,0.0f));
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
            getPointAroundAxis(sphere.origin,out angleY, out angleX);
        }
        
        public void getPointAroundAxis(Vector3 point,out float angleY, out float angleX){
            float tempY = this.angleY;
            float tempX = this.angleX;
            bool over180 = (this.angleY > Mathf.PI)? true:false;
            axis.getPointAroundOrigin(point,out angleY, out angleX);
            if (!float.IsNaN(angleY) && !float.IsNaN(angleX)){
                if (angleY == 0f || angleY == Mathf.PI) angleX = this.angleX;
                if (over180) {
                    angleY = 2* Mathf.PI - angleY;
                    angleX = (Mathf.PI + angleX)%(2* Mathf.PI);
                    };
            } else {
                angleY = tempY;
                angleX = tempX;
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
        public float worldAngleY, worldAngleX, localAngleY;
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
            renderAxis = new RenderAxis(this);
            spinPast = new AroundAxis(this, renderAxis.spinPast);
            spinFuture = new AroundAxis(this, renderAxis.spinFuture);
            movePast = new AroundAxis(this, renderAxis.movePast);
            moveFuture = new AroundAxis(this, renderAxis.moveFuture);
            renderAxis.deleteAxis();
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
        public Vector3 normalize(Vector3 vec){    
            float radius = length(vec);
            if (radius > 0){
                vec.x /= radius;
                vec.y /= radius;
                vec.z /= radius;
            }
            return vec;
        }
        public float length(Vector4 vectorDirections){
            float radius = Mathf.Sqrt(
                Mathf.Pow(vectorDirections.x,2.0f)+
                Mathf.Pow(vectorDirections.y,2.0f)+
                Mathf.Pow(vectorDirections.z,2.0f)+
                Mathf.Pow(vectorDirections.w,2.0f)
            );
            return radius;
        }
        public Vector4 normalize(Vector4 vec){    
            float radius = length(vec);
            if (radius > 0){
                vec.x /= radius;
                vec.y /= radius;
                vec.z /= radius;
                vec.w /= radius;
            }
            return vec;
        }
        internal Vector3 direction(Vector3 point,Vector3 origin){ 
            Vector3 v = point-origin;
            return v/length(v);
        }
        internal Vector3 distanceFromOrigin(Vector3 point,Vector3 origin, float distance){
            return direction(point,origin)*distance;
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
        public void convertAngleAxis(Vector4 q, out float angle, out Vector3 axis){
            q = normalize(q);
            angle = 2.0f * Mathf.Acos(q.w);

            if (Mathf.Approximately(angle, 0.0f)){
                axis = Vector3.zero;
                return;
            }

            float sinHalfAngle = Mathf.Sin(angle / 2.0f);
            axis = new Vector3(
                q.x / sinHalfAngle,
                q.y / sinHalfAngle,
                q.z / sinHalfAngle
            );
            axis = axis.normalized;
        }
        public Vector4 inverseQuat(Vector4 q) {
            return new Vector4(-q.x,-q.y,-q.z,q.w);
        }
        public Vector3 quatRotate(Vector3 point, Vector3 origin, Vector4 angledAxis){
            Vector3 pointDirection = point - origin;     
            Vector4 rotatingVector = new Vector4(pointDirection.x, pointDirection.y, pointDirection.z,0);
            Vector4 rotatedQuaternion = quatMul(quatMul(angledAxis,rotatingVector), inverseQuat(angledAxis));
            return origin + new Vector3(rotatedQuaternion.x,rotatedQuaternion.y,rotatedQuaternion.z);
        }

        public void alignRotationTo(Axis engineAxis, out float angle, out Vector3 axis, out Vector4 quat){
            Vector4 from = getQuat(this);
            Vector4 to = getQuat(engineAxis);
            quat = quatMul(to, inverseQuat(from));
            convertAngleAxis(quat,out angle, out axis);
        }
        public void alignRotationTo(Vector4 to, out float angle, out Vector3 axis, out Vector4 quat){
            Vector4 from = getQuat(this);
            quat = quatMul(to, inverseQuat(from));
            convertAngleAxis(quat,out angle, out axis);
        }
        public Vector4 getQuat(){
            return matrixToQuat(rotationMatrix(this));
        }
        public Vector4 getQuat(Axis engineAxis){
            return matrixToQuat(rotationMatrix(engineAxis));
        }

        Matrix4x4 rotationMatrix(Axis axis){
            Vector3 right = direction(axis.x,axis.origin);
            Vector3 up = direction(axis.y,axis.origin);
            Vector3 forward = direction(axis.z,axis.origin);
            return new Matrix4x4(
                new Vector4(right.x, right.y, right.z, 0),
                new Vector4(up.x, up.y, up.z, 0),
                new Vector4(forward.x, forward.y, forward.z, 0),
                new Vector4(0, 0, 0, 1)
            );
        }
        
        public Vector4 matrixToQuat(Matrix4x4 m){
            float trace = m.m00 + m.m11 + m.m22;
            float w, x, y, z;

            if (trace > 0) {
                float s = Mathf.Sqrt(1 + trace) * 2;
                w = 0.25f * s;
                x = (m.m21 - m.m12) / s;
                y = (m.m02 - m.m20) / s;
                z = (m.m10 - m.m01) / s;
            }
            else if ((m.m00 > m.m11) && (m.m00 > m.m22)) {
                float s = Mathf.Sqrt(1 + m.m00 - m.m11 - m.m22) * 2;
                w = (m.m21 - m.m12) / s;
                x = 0.25f * s;
                y = (m.m01 + m.m10) / s;
                z = (m.m02 + m.m20) / s;
            }
            else if (m.m11 > m.m22) {
                float s = Mathf.Sqrt(1 + m.m11 - m.m00 - m.m22) * 2;
                w = (m.m02 - m.m20) / s;
                x = (m.m01 + m.m10) / s;
                y = 0.25f * s;
                z = (m.m12 + m.m21) / s;
            }
            else {
                float s = Mathf.Sqrt(1 + m.m22 - m.m00 - m.m11) * 2;
                w = (m.m10 - m.m01) / s;
                x = (m.m02 + m.m20) / s;
                y = (m.m12 + m.m21) / s;
                z = 0.25f * s;
            }
            return new Vector4(x, y, z, w);
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
 
    public class SendToGPU{
        public Body body;
        public Vector3[] vertices;
        public Color[] colors;
        public int[] triangles;

        public SendToGPU(Body body){
            vertices = new Vector3[0];
            colors = new Color[0];
            triangles = new int[0];
            this.body = body;
        }
        public void init(){
            updatePointCloudStart();
            int countTriangleIndex = 0;
            foreach(Joint joint in body.bodyStructure){
                if (joint != null){
                    PointCloud pointCloud = joint.pointCloud;
                    int startIndex = pointCloud.startIndexInArray;
                    CollisionSphere[] collisionSpheres = pointCloud.collisionSpheres;
                    for (int i = 0; i < collisionSpheres.Length;i++){
                        vertices[i+startIndex] = collisionSpheres[i].aroundAxis.sphere.origin;
                    }
                    for (int i = 0; i < pointCloud.triangles.Length;i++){
                        triangles[i+countTriangleIndex] = pointCloud.triangles[i]+startIndex;
                    }
                    countTriangleIndex += pointCloud.triangles.Length;
                }
            }
        }
        public void updatePointCloudStart(){
            int maxVerticesSize = 0;
            int maxTriangleSize = 0;
            for (int i = 0; i<body.bodyStructure.Length;i++){
                Joint joint = body.bodyStructure[i];
                if (joint !=null){
                    int pointCloudSize = joint.pointCloud.collisionSpheres.Length;
                    if (pointCloudSize>0){
                        joint.pointCloud.startIndexInArray = maxVerticesSize;
                        maxVerticesSize += pointCloudSize;
                        maxTriangleSize += joint.pointCloud.triangles.Length;
                    }
                }
            }
            colors = new Color[maxVerticesSize];
            vertices = new Vector3[maxVerticesSize];
            triangles = new int[maxTriangleSize];
        }
        public void updateArray(){
            for (int i = 0; i<body.bodyStructure.Length;i++){
                body.bodyStructure[i]?.pointCloud.updateGPUArray();
            }
        }
    }
    public class MeshData {
        public Vector3[] vertices;
        public int[] triangles;
        public MeshData(Vector3[] vertices,int[] triangles){
            this.vertices = vertices;
            this.triangles = triangles;
        }
    }
    public class Body {
        public World world;
        public int worldKey;
        public Axis globalAxis;
        public Joint[] bodyStructure;
        public KeyGenerator keyGenerator;
        public Editor editor;
        public List<MeshData> bakedMeshes;
        public string amountOfDigits; 
        public int timerStart, time;
        public string globalOriginLocationString,globalAxisRotationXYZString,radianOrDegreeString,
            updateReadWriteString,accuracyAmountString,showAxisString,globalAxisScaleString,
            bodyStructureSizeString,allJointsInBodyString;
        public SendToGPU sendToGPU;
        public UnityAxis unityAxis;

        public Body(){}
        public Body(int worldKey){
            init();
            this.worldKey = worldKey;
        }
        public Body(int worldKey, UnityAxis unityAxis){
            init();
            this.worldKey = worldKey;
            this.unityAxis = unityAxis;
        }
        void init(){
            globalAxis = new Axis(this,new Vector3(0,0,0),1);
            bodyStructure = null;
            keyGenerator = null;
            editor = new Editor(this);
            editor.initilizeBody();
            amountOfDigits = "0.000000";
            time = 0;
            timerStart = 20;
            sendToGPU = new SendToGPU(this);

            globalOriginLocationString = ""; 
            globalAxisRotationXYZString = ""; 
            radianOrDegreeString = "";
            updateReadWriteString = "";
            accuracyAmountString = "";
            showAxisString = "";
            globalAxisScaleString = "";
            bodyStructureSizeString = "";
            allJointsInBodyString = "";
        }

        public void newCountStart(int timerStart){
            this.timerStart = timerStart;    
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

        public string accuracyAmount(float num){
            return num.ToString(amountOfDigits);
        }
        public string saveBodyPosition(bool radianOrDegree){
            Vector3 globalOrigin = globalAxis.origin;
            float worldAngleY,worldAngleX,localAngleY;
            if (radianOrDegree)
                globalAxis.getWorldRotationInDegrees(out worldAngleY,out worldAngleX,out localAngleY);
                else 
                globalAxis.getWorldRotationInRadians(out worldAngleY,out worldAngleX,out localAngleY);
            string stringPath = $"Body_{worldKey}_";
            globalOriginLocationString = $" {accuracyAmount(globalOrigin.x)} {accuracyAmount(globalOrigin.y)} {accuracyAmount(globalOrigin.z)}";
            globalAxisRotationXYZString = $" {accuracyAmount(worldAngleY)} {accuracyAmount(worldAngleX)} {accuracyAmount(localAngleY)}";
            radianOrDegreeString = $" {radianOrDegree}";
            return $"{stringPath}{globalOriginLocation}:{globalOriginLocationString}\n" + 
                $"{stringPath}{globalAxisRotationXYZ}:{globalAxisRotationXYZString}\n" + 
                $"{stringPath}{radianOrAngle}:{radianOrDegreeString}\n";
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
            updateReadWriteString = $" {timerStart}";
            accuracyAmountString = $" {amountOfDigits.Length-2}";
            showAxisString = $" {globalAxis.renderAxis.created}";
            globalAxisScaleString = $" {accuracyAmount(globalAxis.axisDistance)}";
            bodyStructureSizeString = $" {bodyStructure.Length}";
            allJointsInBodyString = $"{strJointIndexes}";

            return $"{stringPath}{updateReadWrite}:{updateReadWriteString}\n" + 
                $"{stringPath}{accuracy}:{accuracyAmountString}\n" + 
                $"{stringPath}{showAxis}:{showAxisString}\n" + 
                $"{stringPath}{globalAxisScale}:{globalAxisScaleString}\n" + 
                $"{stringPath}{bodyStructureSize}:{bodyStructureSizeString}\n" + 
                $"{stringPath}{allJointsInBody}:{allJointsInBodyString}\n";
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
            if (unityAxis != null){          
                globalAxis.placeAxis(unityAxis.origin);
            }
            for (int i = 0; i<bodyStructure.Length; i++){
                bodyStructure[i]?.updatePhysics();
            }            
        }
        public Dictionary<int,int> arraySizeManager(int amount){
            Dictionary<int,int> newKeys = new Dictionary<int,int>();
            if (bodyStructure != null){
                if (amount > bodyStructure.Length){
                    resizeArray(amount);
                } else if (amount < bodyStructure.Length){
                    newKeys = optimizeBody();
                }
            } else {
                bodyStructure = new Joint[amount];
                keyGenerator = new KeyGenerator(amount);
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
            int capacity = past.Count+future.Count; // Desired capacity
            List<Joint> pastAndFuture = new List<Joint>(capacity);
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
    public class UnityAxis{
        public Joint joint;
        public Vector3 origin;
        public Vector4 quat;

        public UnityAxis(){}
        public UnityAxis(Vector3 origin, Vector4 quat){
            this.origin = origin;
            this.quat = quat;
        }

        public void updateJoint(){
            Vector3 move = joint.unityAxis.origin - joint.localAxis.origin;
            joint.moveHierarchy(move, true);
            joint.localAxis.alignRotationTo(joint.unityAxis.quat, out float angle, out Vector3 axis, out Vector4 quat);
            joint.localAxis.spinFuture.sphere.setOrigin(joint.unityAxis.origin+axis*joint.localAxis.axisDistance);
            joint.localAxis.spinFuture.get();
            joint.localAxis.spinFuture.speed = angle;
            joint.rotateHierarchy(quat, true);
        }
    }
    public class Joint {
        public Body body;
        public Axis localAxis;
        public Connection connection;
        public PointCloud pointCloud;
        public float fromGlobalAxisY,fromGlobalAxisX,distanceFromGlobalAxis;
        public bool movementOptionBool,keepBodyTogetherBool;
        public UnityAxis unityAxis;
        public string movementOptionString,distanceFromGlobalOriginString,YXFromGlobalAxisString,localAxisRotationString,localOriginLocationString,
            activeString,showAxisString,keepBodyTogetherString,localAxisScaleString,
            spinPastXString,spinPastYString,spinPastSpeedAndAccelerationString,
            spinFutureXString,spinFutureYString,spinFutureSpeedAndAccelerationString,
            movePastXString,movePastYString,movePastSpeedAndAccelerationString,
            moveFutureXString,moveFutureYString,moveFutureSpeedAndAccelerationString,
            pastConnectionsInBodyString,futureConnectionsInBodyString,
            resetPastJointsString,resetFutureJointsString;

        public Joint(){}
        public Joint(Body body, int indexInBody){
            init(body, indexInBody);
        }
        public Joint(Body body, int indexInBody, UnityAxis unityAxis){
            init(body, indexInBody);
            unityAxis.joint = this;
            this.unityAxis = unityAxis;
        }
        void init(Body body, int indexInBody){
            this.body = body;
            localAxis = new Axis(body,new Vector3(0,0,0),1);
            connection = new Connection(this,indexInBody);
            pointCloud = new PointCloud(this);
            body.keyGenerator.getKey();
            fromGlobalAxisY = 0;
            fromGlobalAxisX = 0;
            movementOptionBool = false;
            keepBodyTogetherBool = true;

            movementOptionString = "";
            distanceFromGlobalOriginString = "";
            YXFromGlobalAxisString = "";
            localAxisRotationString = "";
            localOriginLocationString = "";
            activeString = "";
            showAxisString = "";
            keepBodyTogetherString = "";
            localAxisScaleString = "";
            spinPastXString = "";
            spinPastYString = "";
            spinPastSpeedAndAccelerationString = "";
            spinFutureXString = "";
            spinFutureYString = "";
            spinFutureSpeedAndAccelerationString = "";
            movePastXString = "";
            movePastYString = "";
            movePastSpeedAndAccelerationString = "";
            moveFutureXString = "";
            moveFutureYString = "";
            moveFutureSpeedAndAccelerationString = "";
            pastConnectionsInBodyString = "";
            futureConnectionsInBodyString = "";
            resetPastJointsString = "";
            resetFutureJointsString = "";
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
            movementOptionString = $"{stringPath}{movementOption}: {movementOptionBool}\n";
            distanceFromGlobalOriginString = $"{stringPath}{distanceFromGlobalOrigin}: {body.accuracyAmount(distanceFromOrigin)}\n";
            YXFromGlobalAxisString = $"{stringPath}{YXFromGlobalAxis}: {body.accuracyAmount(fromGlobalAxisY*convert)} {body.accuracyAmount(fromGlobalAxisX*convert)}\n";
            localAxisRotationString = $"{stringPath}{localAxisRotation}: {body.accuracyAmount(worldAngleY)} {body.accuracyAmount(worldAngleX)} {body.accuracyAmount(localAngleY)}\n";
            localOriginLocationString = $"{stringPath}{localOriginLocation}: {body.accuracyAmount(localAxis.origin.x)} {body.accuracyAmount(localAxis.origin.y)} {body.accuracyAmount(localAxis.origin.z)}";
            return movementOptionString + distanceFromGlobalOriginString + YXFromGlobalAxisString + localAxisRotationString + localOriginLocationString;
        }


        public string saveJoint(bool radianOrAngle){
            float convert = radianOrAngle? 180f/Mathf.PI:1;
            string stringPath = $"Body_{body.worldKey}_Joint_{connection.indexInBody}_";
            activeString = $" {connection.active}";
            showAxisString = $" {localAxis.renderAxis.created}";
            keepBodyTogetherString = $" {keepBodyTogetherBool}";
            localAxisScaleString = $" {body.accuracyAmount(localAxis.axisDistance)}";
            spinPastXString = $" {body.accuracyAmount(localAxis.spinPast.angleX*convert)} {body.accuracyAmount(localAxis.spinPast.sensitivitySpeedX*convert)} {body.accuracyAmount(localAxis.spinPast.sensitivityAccelerationX)}";
            spinPastYString = $" {body.accuracyAmount(localAxis.spinPast.angleY*convert)} {body.accuracyAmount(localAxis.spinPast.sensitivitySpeedY*convert)} {body.accuracyAmount(localAxis.spinPast.sensitivityAccelerationY)}";
            spinPastSpeedAndAccelerationString = $" {body.accuracyAmount(localAxis.spinPast.speed*convert)} {body.accuracyAmount(localAxis.spinPast.acceleration)}";
            spinFutureXString = $" {body.accuracyAmount(localAxis.spinFuture.angleX*convert)} {body.accuracyAmount(localAxis.spinFuture.sensitivitySpeedX*convert)} {body.accuracyAmount(localAxis.spinFuture.sensitivityAccelerationX)}";
            spinFutureYString = $" {body.accuracyAmount(localAxis.spinFuture.angleY*convert)} {body.accuracyAmount(localAxis.spinFuture.sensitivitySpeedY*convert)} {body.accuracyAmount(localAxis.spinFuture.sensitivityAccelerationY)}";
            spinFutureSpeedAndAccelerationString = $" {body.accuracyAmount(localAxis.spinFuture.speed*convert)} {body.accuracyAmount(localAxis.spinFuture.acceleration)}";
            movePastXString = $" {body.accuracyAmount(localAxis.movePast.angleX*convert)} {body.accuracyAmount(localAxis.movePast.sensitivitySpeedX*convert)} {body.accuracyAmount(localAxis.movePast.sensitivityAccelerationX)}";
            movePastYString = $" {body.accuracyAmount(localAxis.movePast.angleY*convert)} {body.accuracyAmount(localAxis.movePast.sensitivitySpeedY*convert)} {body.accuracyAmount(localAxis.movePast.sensitivityAccelerationY)}";
            movePastSpeedAndAccelerationString = $" {body.accuracyAmount(localAxis.movePast.speed)} {body.accuracyAmount(localAxis.movePast.acceleration)}";
            moveFutureXString = $" {body.accuracyAmount(localAxis.moveFuture.angleX*convert)} {body.accuracyAmount(localAxis.moveFuture.sensitivitySpeedX*convert)} {body.accuracyAmount(localAxis.moveFuture.sensitivityAccelerationX)}";
            moveFutureYString = $" {body.accuracyAmount(localAxis.moveFuture.angleY*convert)} {body.accuracyAmount(localAxis.moveFuture.sensitivitySpeedY*convert)} {body.accuracyAmount(localAxis.moveFuture.sensitivityAccelerationY)}";
            moveFutureSpeedAndAccelerationString = $" {body.accuracyAmount(localAxis.moveFuture.speed)} {body.accuracyAmount(localAxis.moveFuture.acceleration)}";
            pastConnectionsInBodyString = $" {connection.pastToString()}";
            futureConnectionsInBodyString = $" {connection.futureToString()}";
            resetPastJointsString = $" False";
            resetFutureJointsString = $" False";
            return $"{stringPath}{active}:{activeString}\n" + 
                $"{stringPath}{showAxis}:{showAxisString}\n" + 
                $"{stringPath}{keepBodyTogether}:{keepBodyTogetherString}\n" + 
                $"{stringPath}{localAxisScale}:{localAxisScaleString}\n" +
                $"{stringPath}{pastConnectionsInBody}:{pastConnectionsInBodyString}\n" + 
                $"{stringPath}{futureConnectionsInBody}:{futureConnectionsInBodyString}\n" + 
                $"{stringPath}{resetPastJoints}:{resetPastJointsString}\n" + 
                $"{stringPath}{resetFutureJoints}:{resetPastJointsString}\n" +
                $"{stringPath}{spinPastX}:{spinPastXString}\n" + 
                $"{stringPath}{spinPastY}:{spinPastYString}\n" + 
                $"{stringPath}{spinPastSpeedAndAcceleration}:{spinPastSpeedAndAccelerationString}\n" +
                $"{stringPath}{spinFutureX}:{spinFutureXString}\n" + 
                $"{stringPath}{spinFutureY}:{spinFutureYString}\n" + 
                $"{stringPath}{spinFutureSpeedAndAcceleration}:{spinFutureSpeedAndAccelerationString}\n" +
                $"{stringPath}{movePastX}:{movePastXString}\n" + 
                $"{stringPath}{movePastY}:{movePastYString}\n" + 
                $"{stringPath}{movePastSpeedAndAcceleration}:{movePastSpeedAndAccelerationString}\n" +
                $"{stringPath}{moveFutureX}:{moveFutureXString}\n" + 
                $"{stringPath}{moveFutureY}:{moveFutureYString}\n" + 
                $"{stringPath}{moveFutureSpeedAndAcceleration}:{moveFutureSpeedAndAccelerationString}\n";
        }

        public void deleteJoint(){
            body.keyGenerator.returnKey();
            connection.disconnectAllFuture();
            connection.disconnectAllPast();
            pointCloud.deleteAllSpheres();
            localAxis.renderAxis.deleteAxis();
            body.bodyStructure[connection.indexInBody] = null;
        }
        public void getDistanceFromGlobalOrigin(float newDistance){
            Vector3 globalOrigin = body.globalAxis.origin;
            Vector3 localOrigin = localAxis.origin;
            float length = localAxis.length(localOrigin-globalOrigin);
            Vector3 direction = (length>0)? localAxis.direction(localOrigin,globalOrigin)*(newDistance-length): localAxis.direction(localAxis.y,globalOrigin)*(newDistance-length);
            moveJoint(direction);
        }

        public void moveJoint(Vector3 add){
            localAxis.moveAxis(add);
            if (pointCloud.collisionSpheres != null) pointCloud.moveSpheres(add);
        }
        public void rotateJoint(Vector4 quat, Vector3 rotationOrigin){
            localAxis.rotate(quat,rotationOrigin);
            localAxis.getWorldRotation();
            if (pointCloud.collisionSpheres != null) pointCloud.rotateAllSpheres(quat,rotationOrigin);
        }
        public void worldRotateJoint(float worldAngleY,float worldAngleX,float localAngleY){
            localAxis.setWorldRotationInRadians(worldAngleY,worldAngleX,localAngleY);
            if (pointCloud.collisionSpheres != null) pointCloud.resetAllSphereOrigins();
        }

        public void updatePhysics(){
            localAxis.spinFuture.updatePhysics(false);
            localAxis.spinPast.updatePhysics(false);
            localAxis.movePast.updatePhysics(true);
            localAxis.moveFuture.updatePhysics(true);   
            unityAxis?.updateJoint();
            localAxis.getQuat();
            if (pointCloud.collisionSpheres != null) pointCloud.updatePhysics();  
        }

        public void rotatePastHierarchy(){
            Vector4 quat = localAxis.spinFuture.quat(localAxis.spinPast.speed);
            rotateHierarchy(quat, false);
        }
        public void rotateFutureHierarchy(){
            Vector4 quat = localAxis.spinFuture.quat(localAxis.spinFuture.speed);
            rotateHierarchy(quat, true);
        }

        internal void rotateHierarchy(Vector4 quat, bool pastOrFuture){
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
            if (keepBodyTogetherBool) moveHierarchy(move, true);
        }
        public void moveFutureHierarchy(){
            Vector3 move = localAxis.moveFuture.sphere.origin - localAxis.origin;
            moveHierarchy(move, true);
            if (keepBodyTogetherBool) moveHierarchy(move, false);
        }
        internal void moveHierarchy(Vector3 newVec, bool pastOrFuture){
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
        public void resetPastJointHierarchies(){
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
        public void resetFutureHierarchies(){
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
        internal void resetUsed(Joint[] joints, int size){
            for (int i = 0; i<size;i++){
                joints[i].connection.used = false;
            }
        }
    }
    public class PointCloud {
        public Joint joint;
        public KeyGenerator keyGenerator;
        public CollisionSphere[] collisionSpheres;
        public string pointCloudSizeString,allSpheresInJointString,triangleString;
        public int startIndexInArray;
        public int[] triangles;

        public PointCloud(){}
        public PointCloud(Joint joint){
            collisionSpheres = null;
            this.joint = joint;
            triangles = new int[0];
            pointCloudSizeString = "";
            allSpheresInJointString = "";
            triangleString = "";
        }

        public string savePointCloud(out List<int> indexes, out int listSize){
            listSize = 0;
            indexes = new List<int>();
            string stringPath = $"Body_{joint.body.worldKey}_Joint_{joint.connection.indexInBody}_";
            if (collisionSpheres != null){
                int size = collisionSpheres.Length;
                pointCloudSizeString = $" {size}";
                allSpheresInJointString = "";
                for (int i = 0; i<collisionSpheres.Length; i++){
                    CollisionSphere collisionSphere = collisionSpheres[i];
                    if (collisionSphere != null) {
                        allSpheresInJointString += $" {i}";
                        indexes.Add(i);
                        listSize++;
                    }
                }
                if (triangles.Length>3){
                    for (int i = 0; i<triangles.Length; i+=3){
                        int index1 = triangles[i];
                        int index2 = triangles[i+1];
                        int index3 = triangles[i+2];
                        bool check = index1<triangles.Length || index2<triangles.Length || index3<triangles.Length;
                        if (check) triangleString += $" {index1} {index2} {index3}";
                    }
                }                
                return $"{stringPath}{pointCloudSize}:{pointCloudSizeString}\n"+ 
                    $"{stringPath}{allSpheresInJoint}:{allSpheresInJointString}\n"+ 
                    $"{stringPath}{trianglesInPointCloud}:{triangleString}\n";
            }
            return $"{stringPath}{pointCloudSize}: {0}\n"+ 
                    $"{stringPath}{allSpheresInJoint}:\n"+ 
                    $"{stringPath}{trianglesInPointCloud}:\n";
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
                collisionSpheres[i]?.aroundAxis.resetOrigin();
            }
        }
        public void updateGPUArray(){
            for (int i = 0; i<collisionSpheres.Length;i++){
                CollisionSphere collisionSphere = collisionSpheres[i];
                if (collisionSphere!= null){
                    joint.body.sendToGPU.vertices[i+startIndexInArray] = collisionSphere.aroundAxis.sphere.origin;
                }
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
            for (int i = 0;i<sphereCount; i++){
                collisionSpheres[i]?.updatePhysics(); 
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
            if (collisionSpheres != null){
                if (amount > collisionSpheres.Length){
                    resizeArray(amount);
                } else if (amount < collisionSpheres.Length){
                    newKeys = optimizeCollisionSpheres();
                }
            } else {
                collisionSpheres = new CollisionSphere[amount];
                keyGenerator = new KeyGenerator(amount);
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
                    CollisionSphere collisionSphere = collisionSpheres[i];
                    if (collisionSphere != null){
                        newCollisionSpheresArray[i] = collisionSphere;
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
        public CollisionSphere collisionSphere;
        public int indexInBakedMesh;
        public int indexInVertex;
        public BakedMeshIndex(){}
        public BakedMeshIndex(int indexInBakedMesh,int indexInVertex){
            this.indexInBakedMesh = indexInBakedMesh;
            this.indexInVertex = indexInVertex;
        }
        public void updatePoint(){
            MeshData bakedMesh = collisionSphere.path.body.bakedMeshes[indexInBakedMesh];
            AroundAxis aroundAxis = collisionSphere.aroundAxis;
            Axis axis = aroundAxis.axis;
            Vector3 point = collisionSphere.path.joint.body.bodyStructure[0].localAxis.origin+bakedMesh.vertices[indexInVertex];
            aroundAxis.speed = aroundAxis.axis.length(point - axis.origin) - aroundAxis.distance;
            aroundAxis.getPointAroundAxis(point, out float angleY,out float angleX);
            aroundAxis.sensitivitySpeedY = angleY - aroundAxis.angleY;
            aroundAxis.sensitivitySpeedX = angleX - aroundAxis.angleX;
        }
        public void setPoint(){
            MeshData bakedMesh = collisionSphere.path.body.bakedMeshes[indexInBakedMesh];
            AroundAxis aroundAxis = collisionSphere.aroundAxis;
            Axis axis = aroundAxis.axis;
            Vector3 point = collisionSphere.path.joint.body.bodyStructure[0].localAxis.origin+bakedMesh.vertices[indexInVertex];
            collisionSphere.setOrigin(point);
            aroundAxis.distance = aroundAxis.axis.length(point - axis.origin);
            aroundAxis.getPointAroundAxis(point, out float angleY,out float angleX);
            aroundAxis.angleY = angleY;
            aroundAxis.angleX = angleX;
        }
    }
    public class CollisionSphere {
        public Path path;
        public AroundAxis aroundAxis;
        public BakedMeshIndex bakedMeshIndex;
        public string distanceFromLocalOriginString,YFromLocalAxisString,XFromLocalAxisString,radiusString,colorString;

        public CollisionSphere(){}
        public CollisionSphere(Joint joint, int sphereIndex){
            init(joint, sphereIndex);
        }
        public CollisionSphere(Joint joint, int sphereIndex,BakedMeshIndex bakedMeshIndex){
            init(joint, sphereIndex);
            this.bakedMeshIndex = bakedMeshIndex;
            bakedMeshIndex.collisionSphere = this;
        }
        void init(Joint joint, int sphereIndex){
            path = new Path(joint.body,joint,sphereIndex);
            aroundAxis = new AroundAxis(joint.localAxis,new Sphere());
            joint.pointCloud.keyGenerator.getKey();
            
            distanceFromLocalOriginString = "";
            YFromLocalAxisString = "";
            XFromLocalAxisString = ""; 
            radiusString = ""; 
            colorString= "";
        }
        public string saveCollisionSphere(bool radianOrAngle){
            Body body = path.body;
            Sphere sphere = aroundAxis.sphere;
            float convert = radianOrAngle? 180f/Mathf.PI:1;
            string stringPath = $"Body_{path.body.worldKey}_Joint_{path.joint.connection.indexInBody}_Sphere_{path.collisionSphereKey}_";
            distanceFromLocalOriginString = $" {body.accuracyAmount(aroundAxis.distance)} {body.accuracyAmount(aroundAxis.speed)} {body.accuracyAmount(aroundAxis.acceleration)}";
            YFromLocalAxisString = $" {body.accuracyAmount(aroundAxis.angleY*convert)} {body.accuracyAmount(aroundAxis.sensitivitySpeedY*convert)} {body.accuracyAmount(aroundAxis.sensitivityAccelerationY)}";
            XFromLocalAxisString = $" {body.accuracyAmount(aroundAxis.angleX*convert)} {body.accuracyAmount(aroundAxis.sensitivitySpeedX*convert)} {body.accuracyAmount(aroundAxis.sensitivityAccelerationX)}";
            radiusString = $" {body.accuracyAmount(sphere.radius)}";
            colorString = $" {body.accuracyAmount(sphere.color.r)} {body.accuracyAmount(sphere.color.g)} {body.accuracyAmount(sphere.color.b)} {body.accuracyAmount(sphere.color.a)}";
            return $"{stringPath}{distanceFromLocalOrigin}:{distanceFromLocalOriginString}\n" + 
                $"{stringPath}{YFromLocalAxis}:{YFromLocalAxisString}\n" + 
                $"{stringPath}{XFromLocalAxis}:{XFromLocalAxisString}\n" + 
                $"{stringPath}{radius}:{radiusString}\n" + 
                $"{stringPath}{colorRGBA}:{colorString}\n";
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
            if (bakedMeshIndex != null && path.body.bakedMeshes != null) bakedMeshIndex.updatePoint();
            aroundAxis.updatePhysics(false);
        }
    }
    public class Sphere{
        public float radius;
        public Vector3 origin;
        public Color color;
        // public GameObject sphere;
        
        public Sphere(){
            // sphere = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // sphere.GetComponent<Collider>().enabled = false;
            setOrigin(new Vector3(0,0,0));
            setRadius(0.01f);
            setColor(new Color(1,1,1,1));
            updateColor(color);
        }
        public Sphere(Vector3 origin, float radius, Color color){
            // sphere = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // sphere.GetComponent<Collider>().enabled = false;
            setOrigin(origin);
            setRadius(radius);
            setColor(color);
            updateColor(color);
        }
        public void setOrigin(Vector3 newOrigin){
            origin = newOrigin;
            // if (sphere != null) sphere.transform.position = newOrigin;
        }
        public void moveOrigin(Vector3 newOrigin){
            origin += newOrigin;
            // if (sphere != null)sphere.transform.position += newOrigin;
        }
        public void setRadius(float newRadius){
            radius = newRadius;
            // if (sphere != null) sphere.transform.localScale = new Vector3(radius, radius, radius);
        }
        public void setColor(Color newColor){
            color = newColor;
        }
        public void updateColor(Color newColor){
            // sphere.GetComponent<Renderer>().material.color = newColor;
        }
        public void resetColor(){
            // sphere.GetComponent<Renderer>().material.color = color;
        }  
        public void restoreSphere(){
            // sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            setOrigin(origin);
            setRadius(radius);
            setColor(color);
            updateColor(color);
        }     
        public void destroySphere(){
            // Destroy(sphere);
        }
    }

    const string bodyStructureSize = "BodyStructureSize";
    const string updateReadWrite = "UpdateReadWrite";
    const string allJointsInBody = "AllJointsInBody";
    const string globalAxisScale = "GlobalAxisScale";
    const string globalOriginLocation = "GlobalOriginLocation";
    const string globalAxisRotationXYZ = "GlobalAxisRotationXYZ";
    const string accuracy = "Accuracy";
    const string radianOrAngle = "RadianOrAngle";

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
    const string trianglesInPointCloud = "TrianglesInPointCloud";

    const string distanceFromLocalOrigin = "DistanceFromLocalOrigin";
    const string XFromLocalAxis = "XFromLocalAxis";
    const string YFromLocalAxis = "YFromLocalAxis";
    const string radius = "Radius";
    const string colorRGBA = "ColorRGBA";
    
    public class Editor {
        public BodyInstructions bodyInstructions;
        public JointInstructions jointInstructions;
        public CollisionSphereInstructions collisionSphereInstructions;
        internal bool radianOrDegree = false;
        internal bool initilize;
        internal Body body;
        internal string pathToFolder;
        internal Dictionary<int,int> newJointKeys = new Dictionary<int,int>();
        internal Dictionary<int,int> newSphereKeys = new Dictionary<int,int>();
        internal Dictionary<int,List<int>> deleted = new Dictionary<int, List<int>>();
        internal int count = 0;

        public Editor(){}
        public Editor(Body body){
            this.body = body;
            pathToFolder = $"Assets/v4/{body.worldKey}";
            if (!Directory.Exists(pathToFolder)) {
                Directory.CreateDirectory(pathToFolder);
                }
            bodyInstructions = new BodyInstructions(this);
            jointInstructions = new JointInstructions(this);
            collisionSphereInstructions = new CollisionSphereInstructions(this);
        }
        
        void writePositions(StreamWriter writetext){
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
        }
        void writeMotions(StreamWriter writetext){
            int size = body.bodyStructure.Length;
            for (int i = 0; i<size; i++){
                Joint joint = body.bodyStructure[i];
                if (joint != null){
                    writetext.WriteLine(joint.saveJoint(radianOrDegree));
                }
            }
        }
        
        void reader(int count){
            using(StreamReader readtext = new StreamReader($"{pathToFolder}/{count}.txt")){
                string readText;
                while ((readText = readtext.ReadLine()) != null){
                    string[] splitStr = readText.Split(":");
                    if (splitStr.Length == 2){
                        removeEmpty(splitStr[0].Split("_"), out List<string> instruction);
                        switch (instruction.Count){
                            case 3:
                                bodyInstructions.bodyMethods(instruction[2],splitStr[1]);
                            break;
                            case 5:
                                jointInstructions.jointMethods(instruction[3],instruction[4],splitStr[1]);
                            break;
                            case 7:
                                collisionSphereInstructions.sphereInstructions(instruction[3],instruction[5],instruction[6],splitStr[1]);
                            break;
                        }
                    }
                } 
            }
            newJointKeys = new Dictionary<int,int>();
            newSphereKeys = new Dictionary<int,int>();
            deleted = new Dictionary<int, List<int>>();
        }
        internal void writer(){
            using(StreamWriter writeText = new StreamWriter($"{pathToFolder}/{count}.txt")) {
                writePositions(writeText);
                writeMotions(writeText);
                writeText.WriteLine(body.saveBodyPosition(radianOrDegree));
                writeText.WriteLine("");
            }
        }
        internal void trackWriter(){
            using (StreamWriter writeText = new StreamWriter($"{pathToFolder}/{count}.txt",  true)){
                writePositions(writeText);
            }
            body.updatePhysics();
            using (StreamWriter writeText = new StreamWriter($"{pathToFolder}/{count}.txt",  true)){
                writeMotions(writeText);
                writeText.WriteLine(body.saveBodyPosition(radianOrDegree));
                writeText.WriteLine("");
            }
            count++;
        }
        
        public void initilizeBody(){
            initilize = true;
            reader(0);
            initilize = false;
        }
        public void trackBody(){
            trackWriter();
        }
        public void readWrite(){
            if (body.time == 0){
                reader(0);
                body.updatePhysics();
                writer();
                body.time = body.timerStart;
            } else {
                body.time -=1;
            }
        }
        internal void removeEmpty(string[] strArray, out List<string> list){
            list = new List<string>();
            int arraySize = strArray.Length;
            for (int i = 0; i < arraySize; i++){
                string str = strArray[i];
                if (str != "") list.Add(str);
            }
        }
        internal Joint getJoint(string jointKey){
            bool checkKey = int.TryParse(jointKey, out int key);
            if (newJointKeys.TryGetValue(key, out int newKey)){
                key = newKey;
            }
            return checkKey? body.bodyStructure[key]:null;
        }
        internal CollisionSphere getCollisionSphere(Joint joint,string collisionSphereKey){
            bool checkKey = int.TryParse(collisionSphereKey, out int key);
            if (newSphereKeys.TryGetValue(key, out int newKey)){
                key = newKey;
            }
            return checkKey?joint.pointCloud.collisionSpheres[key]:null;
        }
    }

    public class BodyInstructions{
        Editor editor;
        public BodyInstructions(Editor editor){
            this.editor = editor;
        }
        public void bodyMethods(string instruction, string str){
            switch (instruction){
                case bodyStructureSize:
                    if (!str.Equals(editor.body.bodyStructureSizeString)){
                        bodyStructureSizeInstruction(str);
                    }
                break;
                case updateReadWrite:
                    if (!str.Equals(editor.body.updateReadWriteString)){
                        updateReadWriteInstruction(str);
                    }
                break;
                case showAxis:
                    if (!str.Equals(editor.body.showAxisString)){
                        showAxisInstruction(str);
                    }
                break;
                case globalAxisScale:
                    if (!str.Equals(editor.body.globalAxisScaleString)){
                        globalAxisScaleInstruction(str);
                    }
                break;
                case allJointsInBody:
                    if (!str.Equals(editor.body.allJointsInBodyString)){
                        allJointsInBodyInstruction(str);
                    }
                break;
                case globalOriginLocation:
                    if (!str.Equals(editor.body.globalOriginLocationString)){
                        globalOriginLocationInstruction(str);
                    }
                break;
                case globalAxisRotationXYZ:
                    if (!str.Equals(editor.body.globalAxisRotationXYZString)){
                        globalAxisRotationXYZInstruction(str);
                    }
                break;
                case accuracy:
                    if (!str.Equals(editor.body.accuracyAmountString)){
                        accuracyInstruction(str);
                    }
                break;
                case radianOrAngle:
                    if (!str.Equals(editor.body.radianOrDegreeString)){
                        radianOrAngleInstruction(str);
                    }
                break;
            }
        }
        void bodyStructureSizeInstruction(string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                if (value.Count>0){
                    bool check = int.TryParse(value[0], out int amount);
                    if (check) {
                        editor.newJointKeys = editor.body.arraySizeManager(amount);
                        editor.body.bodyStructureSizeString = str;
                    };
                }
            }
        }
        void updateReadWriteInstruction(string str){
            if (str.Length>0) {
                editor.removeEmpty(str.Split(" "), out List<string> value);
                if (value.Count>0){
                    bool check = int.TryParse(value[0], out int amount);
                    if (check) {
                        editor.body.timerStart = amount;
                        editor.body.updateReadWriteString = str;
                    } 
                }
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
                    if (editor.newJointKeys.TryGetValue(key, out int newKey)){
                        key = newKey;
                    }
                    if (!set.Contains(key)){
                        set.Add(key);
                        if (key >= editor.body.bodyStructure.Length){
                            nullKeys.Add(key);
                            nullCount++;
                        } else if (editor.body.bodyStructure[key] == null){
                            nullKeys.Add(key);
                            nullCount++;
                        } 
                        if (key > maxKey) maxKey = key;
                    }
                } else if (!error && !check) error = true;
            }
        }
        internal HashSet<int> resizeBody(List<string> value, out bool error){
            gatherBodyData(value,out error, out int maxKey, out int nullCount, out HashSet<int> set,out List<int> nullKeys);
            if (maxKey>=editor.body.bodyStructure.Length) editor.body.resizeArray(maxKey);
            for (int i = 0; i < nullCount; i++){
                if (editor.body.bodyStructure[nullKeys[i]] == null)
                    editor.body.bodyStructure[nullKeys[i]] = new Joint(editor.body,nullKeys[i]);
            }
            return set;
        }
        void showAxisInstruction(string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                if (value.Count>0) {
                    if (value[0] == "True"){ 
                        editor.body.globalAxis.renderAxis.createAxis();
                        editor.body.showAxisString = str;
                    }
                    else if (value[0] == "False") {
                        editor.body.globalAxis.renderAxis.deleteAxis();
                        editor.body.showAxisString = str;
                    }
                }
            }
        }
        void globalAxisScaleInstruction(string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                if (value.Count>0){
                    bool checkScale = float.TryParse(value[0], out float newScale);
                    if (checkScale && Mathf.Abs(editor.body.globalAxis.axisDistance-newScale)>0){
                        editor.body.globalAxis.scaleAxis(newScale);    
                        editor.body.globalAxis.spinFuture.scale(newScale);
                        editor.body.globalAxis.spinPast.scale(newScale);
                        editor.body.globalAxisScaleString = str;
                    }
                }
            }
        }
        void allJointsInBodyInstruction(string str){
            editor.removeEmpty(str.Split(" "), out List<string> value);
            HashSet<int> set = resizeBody(value, out bool error);
            if (!error) {
                for (int i = 0; i< editor.body.bodyStructure.Length; i++){
                    if (!set.Contains(i)) editor.body.bodyStructure[i]?.deleteJoint();        
                }
                editor.body.allJointsInBodyString = str;
            }
        }
        void globalOriginLocationInstruction(string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                int size = value.Count;
                if (size >= 3) {
                    bool checkX = float.TryParse(value[0], out float x);
                    bool checkY = float.TryParse(value[1], out float y);
                    bool checkZ = float.TryParse(value[2], out float z);
                    float vecX = checkX? x: editor.body.globalAxis.origin.x;
                    float vecY = checkY? y: editor.body.globalAxis.origin.y;
                    float vecZ = checkZ? z: editor.body.globalAxis.origin.z;
                    editor.body.globalAxis.placeAxis(new Vector3(vecX,vecY,vecZ));
                    editor.body.globalOriginLocationString = str;
                }
            }
        }
        void globalAxisRotationXYZInstruction(string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                int size = value.Count;
                if (size >= 3) {
                    bool checkY = float.TryParse(value[0], out float y);
                    bool checkX = float.TryParse(value[1], out float x);
                    bool checkLY = float.TryParse(value[2], out float ly);
                    if (checkX || checkY || checkLY){
                        if (!editor.initilize){
                            float worldAngleY = Mathf.Abs(y - editor.body.globalAxis.worldAngleY) % (2*Mathf.PI);
                            float worldAngleX = Mathf.Abs(x - editor.body.globalAxis.worldAngleX) % (2*Mathf.PI);
                            float localAngleY = Mathf.Abs(ly - editor.body.globalAxis.localAngleY) % (2*Mathf.PI);
                            float limit = 0.0001f;
                            if (worldAngleY > limit || worldAngleX> limit || localAngleY > limit) {
                                if (editor.radianOrDegree) {
                                    y*= Mathf.PI/180f;
                                    x*= Mathf.PI/180f;
                                    ly*= Mathf.PI/180f;
                                }
                                editor.body.rotateBody(y,x,ly);
                                editor.body.globalAxisRotationXYZString = str;
                            }
                        } else {
                            if (y<0) editor.body.globalAxis.worldAngleY = y;
                            editor.body.globalAxis.setWorldRotation(y,x,ly);
                        }
                    }
                }     
            }       
        }
        void accuracyInstruction(string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                if (value.Count>0){
                    bool checkAccuracy = int.TryParse(value[0], out int newAccuracy);
                    if (checkAccuracy){
                        editor.body.newAccuracy(newAccuracy);
                        editor.body.accuracyAmountString = str;
                    }
                }
            }
        }
        void radianOrAngleInstruction(string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                int size = value.Count;
                if (size>= 1){
                    if (value[0] == "True") {
                        editor.radianOrDegree = true; 
                        editor.body.radianOrDegreeString = str;
                    }
                    else 
                    if (value[0] == "False") {
                        editor.radianOrDegree = false;
                        editor.body.radianOrDegreeString = str;
                    }
                }
            }
        }

    }

    public class JointInstructions{
        Editor editor;
        public JointInstructions(Editor editor){
            this.editor = editor;
        }
        public void jointMethods(string jointKey, string instruction, string str){
            Joint joint = editor.getJoint(jointKey);
            if (joint != null) {
                switch (instruction){
                    case active:
                        if (!str.Equals(joint.activeString)){
                            activeInstruction(joint,str);
                        }
                    break;
                    case showAxis:
                        if (!str.Equals(joint.showAxisString)){
                            showAxisInstruction(joint,str);
                        }
                    break;
                    case keepBodyTogether:
                        if (!str.Equals(joint.keepBodyTogetherString)){
                            keepBodyTogetherInstruction(joint,str);
                        }
                    break;
                    case localAxisScale:
                        if (!str.Equals(joint.localAxisScaleString)){
                            localAxisScaleInstruction(joint,str);
                        }
                    break;
                    case movementOption:
                        if (!str.Equals(joint.movementOptionString)){
                            movementOptionInstruction(joint,str);
                        }
                    break;
                    case distanceFromGlobalOrigin:
                        if (!str.Equals(joint.distanceFromGlobalOriginString)){
                            distanceFromGlobalOriginInstruction(joint,str);
                        }
                    break;
                    case YXFromGlobalAxis:
                        if (true){
                            YXFromGlobalAxisInstruction(joint,str);
                        }
                    break;
                    case localAxisRotation:
                        if (!str.Equals(joint.localAxisRotationString)){
                            localAxisRotationInstruction(joint,str);
                        }
                    break;
                    case localOriginLocation:
                        if (!str.Equals(joint.localOriginLocationString)){
                            localOriginLocationInstruction(joint,str);
                        }
                    break;
                    case spinPastX:
                        if (!str.Equals(joint.spinPastXString)){
                            spinPastXInstruction(joint,str);
                        }
                    break;
                    case spinPastY:
                        if (!str.Equals(joint.spinPastYString)){
                            spinPastYInstruction(joint,str);
                        }
                    break;
                    case spinPastSpeedAndAcceleration:
                        if (!str.Equals(joint.spinFutureSpeedAndAccelerationString)){
                            spinPastSpeedAndAccelerationInstruction(joint,str);
                        }
                    break;
                    case spinFutureX:
                        if (!str.Equals(joint.spinFutureXString)){
                            spinFutureXInstruction(joint,str);
                        }
                    break;
                    case spinFutureY:
                        if (!str.Equals(joint.spinFutureYString)){
                            spinFutureYInstruction(joint,str);
                        }
                    break;
                    case spinFutureSpeedAndAcceleration:
                        if (!str.Equals(joint.spinFutureSpeedAndAccelerationString)){
                            spinFutureSpeedAndAccelerationInstruction(joint,str);
                        }
                    break;
                    case movePastX:
                        if (!str.Equals(joint.movePastXString)){
                            movePastXInstruction(joint,str);
                        }
                    break;
                    case movePastY:
                        if (!str.Equals(joint.movePastYString)){
                            movePastYInstruction(joint,str);
                        }
                    break;
                    case movePastSpeedAndAcceleration:
                        if (!str.Equals(joint.movePastSpeedAndAccelerationString)){
                            movePastSpeedAndAccelerationInstruction(joint,str);
                        }
                    break;
                    case moveFutureX:
                        if (!str.Equals(joint.moveFutureXString)){
                            moveFutureXInstruction(joint,str);
                        }
                    break;
                    case moveFutureY:
                        if (!str.Equals(joint.moveFutureYString)){
                            moveFutureYInstruction(joint,str);
                        }
                    break;
                    case moveFutureSpeedAndAcceleration:
                        if (!str.Equals(joint.moveFutureSpeedAndAccelerationString)){
                            moveFutureSpeedAndAccelerationInstruction(joint,str);
                        }
                    break;                  
                    case pastConnectionsInBody:
                        if (!str.Equals(joint.pastConnectionsInBodyString)){
                            pastConnectionsInBodyInstruction(joint,str);
                        }
                    break;
                    case futureConnectionsInBody:
                        if (!str.Equals(joint.futureConnectionsInBodyString)){
                            futureConnectionsInBodyInstruction(joint,str);
                        }
                    break;
                    case resetPastJoints:
                        if (true){
                            resetPastJointsInstruction(joint,str);
                        }
                    break;
                    case resetFutureJoints:
                        if (true){
                            resetFutureJointsInstruction(joint,str);
                        }
                    break;
                    case pointCloudSize:
                        if (!str.Equals(joint.pointCloud.pointCloudSizeString)){
                            pointCloudSizeInstruction(joint,str);
                        }
                    break;
                    case allSpheresInJoint:
                        if (!str.Equals(joint.pointCloud.allSpheresInJointString)){
                            allSpheresInJointInstruction(joint,str);   
                        }
                    break;
                    case trianglesInPointCloud:
                        if (!str.Equals(joint.pointCloud.triangleString)){
                            trianglesInPointCloudInstruction(joint,str);
                        }                    
                    break;
                }
            }
        }

        void activeInstruction(Joint joint,string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                if (value.Count>0){
                    joint.connection.active = value[0] == "True";
                    joint.activeString = str;
                }
            }
        }
        void showAxisInstruction(Joint joint,string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                if (value.Count>0){
                    if (value[0] == "True") {
                        joint.localAxis.renderAxis.createAxis();
                        joint.showAxisString = str;
                    }
                    else if (value[0] == "False") {
                        joint.localAxis.renderAxis.deleteAxis();
                        joint.showAxisString = str;
                    }
                }
            }
        }
        void keepBodyTogetherInstruction(Joint joint,string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                if (value.Count>0){
                    if (value[0] == "True") {
                        joint.keepBodyTogetherBool = true;
                        joint.keepBodyTogetherString = str;
                    }
                    else if (value[0] == "False") {
                        joint.keepBodyTogetherBool = false;
                        joint.keepBodyTogetherString = str;
                    }
                } 
            }
        }
        void localAxisScaleInstruction(Joint joint,string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                if (value.Count>0){
                    bool checkScale = float.TryParse(value[0], out float newScale);
                    if (checkScale && Mathf.Abs(joint.localAxis.axisDistance-newScale)>0){
                        joint.localAxis.scaleAxis(newScale);
                        joint.localAxis.spinFuture.scale(newScale);
                        joint.localAxis.spinPast.scale(newScale);
                        joint.localAxisScaleString = str;
                    }
                }
            }
        }
        void movementOptionInstruction(Joint joint,string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                if (value.Count>0){
                    if (value[0] == "True") {
                        joint.movementOptionBool = true;
                        joint.movementOptionString = str;
                    }
                    else if (value[0] == "False") {
                        joint.movementOptionBool = false;
                        joint.movementOptionString = str;
                    }
                } 
            }
        }
        void distanceFromGlobalOriginInstruction(Joint joint,string str){
            editor.removeEmpty(str.Split(" "), out List<string> value);
            if (value.Count>0){
                bool checkStr = float.TryParse(value[0], out float distance);
                if (checkStr && !joint.movementOptionBool){
                    joint.getDistanceFromGlobalOrigin(distance);
                    joint.distanceFromGlobalOriginString = str;
                }
            }
        }
        void YXFromGlobalAxisInstruction(Joint joint,string str){
            editor.removeEmpty(str.Split(" "), out List<string> value);
            if (value.Count>=2){
                bool checkY = float.TryParse(value[0], out float y);
                bool checkX = float.TryParse(value[1], out float x);
                Axis globalAxis = joint.body.globalAxis;
                Axis localAxis = joint.localAxis;
                float length = localAxis.length(localAxis.origin-globalAxis.origin);
                if (checkY && checkX && !joint.movementOptionBool) {
                        joint.fromGlobalAxisY = y;
                        joint.fromGlobalAxisX = x;
                        if (editor.radianOrDegree) {
                            y*= Mathf.PI/180f;
                            x*= Mathf.PI/180f;
                            joint.fromGlobalAxisY*= Mathf.PI/180f;
                            joint.fromGlobalAxisX*= Mathf.PI/180f;
                        }
                        joint.moveJoint(globalAxis.setPointAroundOrigin(y,x,length) - localAxis.origin);
                    } 
            }
        }
        void localAxisRotationInstruction(Joint joint,string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                if (value.Count>=3){
                    bool checkY = float.TryParse(value[0], out float y);
                    bool checkX = float.TryParse(value[1], out float x);
                    bool checkLY = float.TryParse(value[2], out float ly);
                    if (checkY && checkX && checkLY) {
                        if (editor.radianOrDegree) {
                            y*= Mathf.PI/180f;
                            x*= Mathf.PI/180f;
                            ly*= Mathf.PI/180f;
                        }
                        joint.worldRotateJoint(y,x,ly);
                        joint.localAxisRotationString = str;
                    }
                }
            }
        }
        void localOriginLocationInstruction(Joint joint,string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                if (value.Count >= 3) {
                    bool checkX = float.TryParse(value[0], out float x);
                    bool checkY = float.TryParse(value[1], out float y);
                    bool checkZ = float.TryParse(value[2], out float z);
                    if (checkX && checkY && checkZ && joint.movementOptionBool) {
                        Vector3 add = editor.initilize? new Vector3(0,0,0):joint.localAxis.origin;
                        joint.moveJoint(new Vector3(x,y,z)-add);
                        joint.localOriginLocationString = str;
                    }
                }
            }
        }
        internal bool xAroundAxis(AroundAxis aroundAxis, List<string> value){
            if (value.Count>=3){
                bool checkX = float.TryParse(value[0], out float angleX);
                bool checkSpeedX = float.TryParse(value[1], out float speedX);
                bool checkAccelerationX = float.TryParse(value[2], out float accelerationX);
                if (checkX && checkSpeedX && checkAccelerationX){
                    if (editor.radianOrDegree) {
                        angleX*= Mathf.PI/180f;
                        speedX*= Mathf.PI/180f;
                    }
                    aroundAxis.sensitivitySpeedX = speedX;
                    aroundAxis.sensitivityAccelerationX = accelerationX;
                    if (angleX != aroundAxis.angleX) aroundAxis.setInRadians(aroundAxis.angleY,angleX);  
                    float direction = aroundAxis.sensitivitySpeedX*aroundAxis.sensitivityAccelerationX;  
                    if (Mathf.Abs(direction) > 0) aroundAxis.rotationX();
                    return true;
                }
            }
            return false;
        }
        internal bool yAroundAxis(AroundAxis aroundAxis, List<string> value){
            if (value.Count>=3){
                bool checkY = float.TryParse(value[0], out float angleY);
                bool checkSpeedY = float.TryParse(value[1], out float speedY);
                bool checkAccelerationY = float.TryParse(value[2], out float accelerationY);    
                if (checkY && checkSpeedY && checkAccelerationY){ 
                    if (editor.radianOrDegree) {
                        angleY*= Mathf.PI/180f;
                        speedY*= Mathf.PI/180f;
                    }
                    aroundAxis.sensitivitySpeedY = speedY;
                    aroundAxis.sensitivityAccelerationY = accelerationY;
                    if (angleY != aroundAxis.angleY) aroundAxis.setInRadians(angleY,aroundAxis.angleX); 
                    float direction = aroundAxis.sensitivitySpeedY*aroundAxis.sensitivityAccelerationY;              
                    if (Mathf.Abs(direction) > 0) aroundAxis.rotationY();
                    return true;
                }
            }
            return false;
        }

        void spinPastXInstruction(Joint joint,string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                bool check = xAroundAxis(joint.localAxis.spinPast,value);
                if (check) joint.spinPastXString = str;
            }
        }
        void spinPastYInstruction(Joint joint,string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                bool check = yAroundAxis(joint.localAxis.spinPast,value);
                if (check) joint.spinPastXString = str;
            }
        }
        void spinPastSpeedAndAccelerationInstruction(Joint joint,string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                if (value.Count>=2){
                    AroundAxis aroundAxis = joint.localAxis.spinPast;
                    bool checkSpeed = float.TryParse(value[0], out float speed);
                    bool checkAcceleration = float.TryParse(value[1], out float acceleration);  
                    if (checkSpeed && checkAcceleration) {
                        if (editor.radianOrDegree) {
                            speed*= Mathf.PI/180f;
                        }  
                        aroundAxis.speed = speed;
                        aroundAxis.acceleration = acceleration;
                        joint.rotatePastHierarchy();
                        joint.spinPastYString = str;
                    }
                }
            }
        }

        void spinFutureXInstruction(Joint joint,string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                bool check = xAroundAxis(joint.localAxis.spinPast,value);
                if (check) joint.spinFutureXString = str;
            }
        }
        void spinFutureYInstruction(Joint joint,string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                bool check = yAroundAxis(joint.localAxis.spinFuture,value);
                if (check) joint.spinFutureYString = str;
            }
        }
        void spinFutureSpeedAndAccelerationInstruction(Joint joint,string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                if (value.Count>=2){
                    AroundAxis aroundAxis = joint.localAxis.spinFuture;
                    bool checkSpeed = float.TryParse(value[0], out float speed);
                    bool checkAcceleration = float.TryParse(value[1], out float acceleration);  
                    if (checkSpeed && checkAcceleration) {
                        if (editor.radianOrDegree) {
                            speed*= Mathf.PI/180f;
                        }
                        aroundAxis.speed = speed;
                        aroundAxis.acceleration = acceleration;
                        joint.rotateFutureHierarchy();
                        joint.spinFutureSpeedAndAccelerationString = str;
                    }
                }
            }
        }
        void movePastXInstruction(Joint joint,string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                bool check = xAroundAxis(joint.localAxis.movePast,value);
                if (check) joint.movePastXString = str;
            }
        }
        void movePastYInstruction(Joint joint,string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                bool check = yAroundAxis(joint.localAxis.movePast,value);
                if (check) joint.movePastXString = str;
            }
        }
        void movePastSpeedAndAccelerationInstruction(Joint joint,string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
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
                        joint.movePastSpeedAndAccelerationString = str;
                    }
                }
            }
        }
        void moveFutureXInstruction(Joint joint,string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                bool check = xAroundAxis(joint.localAxis.moveFuture,value);
                if (check) joint.moveFutureXString = str;
            }
        }
        void moveFutureYInstruction(Joint joint,string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                bool check = yAroundAxis(joint.localAxis.moveFuture,value);
                if (check) joint.moveFutureYString = str;
            }
        }
        void moveFutureSpeedAndAccelerationInstruction(Joint joint,string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
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
                        joint.moveFutureSpeedAndAccelerationString = str;
                    }
                }
            }
        }
        void pastConnectionsInBodyInstruction(Joint joint,string str){
            editor.removeEmpty(str.Split(" "), out List<string> value);
            HashSet<int> set = editor.bodyInstructions.resizeBody(value, out _);
            List<LockedConnection> past = joint.connection.past;
            int size = past.Count;
            List<LockedConnection> newPast = new List<LockedConnection>();
            bool checkDelete = editor.deleted.TryGetValue(joint.connection.indexInBody,out List<int> delete);
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
                    checkDelete = editor.deleted.TryGetValue(index,out delete);
                    if (checkDelete){
                        delete.Add(joint.connection.indexInBody);
                    } else {
                        editor.deleted[index] = new List<int>(){joint.connection.indexInBody};
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
                        if (newjoint == null) newjoint = new Joint(editor.body,i);  
                        newjoint.connection.connectThisFutureToPast(joint, out _, out LockedConnection lockOther);
                        newPast.Add(lockOther);
                    }
                }
                joint.connection.past = newPast;
                joint.pastConnectionsInBodyString = str;
            }
        }
        void futureConnectionsInBodyInstruction(Joint joint,string str){
            editor.removeEmpty(str.Split(" "), out List<string> value);
            HashSet<int> set = editor.bodyInstructions.resizeBody(value, out _);
            List<LockedConnection> future = joint.connection.future;
            int size = future.Count;
            List<LockedConnection> newFuture = new List<LockedConnection>();
            bool checkDelete = editor.deleted.TryGetValue(joint.connection.indexInBody,out List<int> delete);
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
                    checkDelete = editor.deleted.TryGetValue(index,out delete);
                    if (checkDelete){
                        delete.Add(joint.connection.indexInBody);
                    } else {
                        editor.deleted[index] = new List<int>(){joint.connection.indexInBody};
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
                        if (newjoint == null) newjoint = new Joint(editor.body,i);
                        newjoint.connection.connectThisPastToFuture(joint, out _, out LockedConnection lockOther);
                        newFuture.Add(lockOther);
                    }
                }
                joint.connection.future = newFuture;
                joint.futureConnectionsInBodyString = str;
            }
        }
        void resetPastJointsInstruction(Joint joint,string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                if (value.Count >0){
                    if (value[0]== "True") joint.resetPastJointHierarchies();
                }
            }
        }
        void resetFutureJointsInstruction(Joint joint,string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                if (value.Count >0){
                    if (value[0]== "True") joint.resetFutureHierarchies();
                }
            }
        }
        void pointCloudSizeInstruction(Joint joint,string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                PointCloud pointCloud = joint.pointCloud;
                if (pointCloud != null){
                    if (value.Count> 0){
                        bool check = int.TryParse(value[0], out int amount);
                        if (check) editor.newSphereKeys = joint.pointCloud.arraySizeManager(amount);
                        joint.pointCloud.pointCloudSizeString = str;
                    }
                }
            }
        }
        void gatherJointData(Joint joint, List<string> value,out bool error, out int maxKey, out int nullCount, out HashSet<int> set,out List<int> nullKeys){
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
                    if (editor.newSphereKeys.TryGetValue(key, out int newKey)){
                        key = newKey;
                    }
                    if (!set.Contains(key)){
                        set.Add(key);
                        if (key >= pointCloud.collisionSpheres.Length){
                            nullKeys.Add(key);
                            nullCount++;
                        } else if (pointCloud.collisionSpheres[key] == null){
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
        void allSpheresInJointInstruction(Joint joint,string str){                           
                editor.removeEmpty(str.Split(" "), out List<string> value);
                PointCloud pointCloud = joint.pointCloud;
                if (pointCloud != null){
                    HashSet<int> set = resizePointCloud(joint,value, out bool error);
                    if (!error) {
                        for (int i = 0; i< joint.pointCloud.collisionSpheres.Length; i++){
                            if (!set.Contains(i)) joint.pointCloud?.deleteSphere(i);
                        }
                        joint.pointCloud.allSpheresInJointString = str;
                    }
                }

        }
        void trianglesInPointCloudInstruction(Joint joint,string str){
            if (str.Length>0){                            
                editor.removeEmpty(str.Split(" "), out List<string> value);
                PointCloud pointCloud = joint.pointCloud;
                if (pointCloud != null){
                    List<int> newTriangles = new List<int>(value.Count);
                    if (value.Count>=3){
                        for (int i = 0; i < value.Count; i+=3){
                            bool check1 = int.TryParse(value[i], out int key1);
                            bool check2 = int.TryParse(value[i+1], out int key2);
                            bool check3 = int.TryParse(value[i+2], out int key3);
                            if (check1 && check2 && check3){
                                newTriangles.Add(Mathf.Abs(key1));
                                newTriangles.Add(Mathf.Abs(key2));
                                newTriangles.Add(Mathf.Abs(key3));
                            } else {
                                newTriangles.RemoveAt(i);
                                newTriangles.RemoveAt(i);
                                newTriangles.RemoveAt(i);
                            }
                        }
                        pointCloud.triangles = newTriangles.ToArray();
                        joint.pointCloud.triangleString = $" {string.Join(" ", value)}";
                    } else if (value.Count == 0){
                        pointCloud.triangles = new int[0];
                        joint.pointCloud.triangleString = "";
                    }
                }
            } else {
                joint.pointCloud.triangles = new int[0];
                joint.pointCloud.triangleString = "";
            }
        }
    }
    public class CollisionSphereInstructions {
        Editor editor;
        public CollisionSphereInstructions(Editor editor){
            this.editor = editor;
        }
        public void sphereInstructions(string jointKey,string collisionSphereKey, string instruction, string str){
            Joint joint = editor.getJoint(jointKey);
            if (joint != null) { 
                CollisionSphere collisionSphere = editor.getCollisionSphere(joint,collisionSphereKey);
                if (collisionSphere != null){
                    switch (instruction){
                        case distanceFromLocalOrigin:  
                            if (!str.Equals(collisionSphere.distanceFromLocalOriginString)){
                                distanceFromLocalAxisInstruction(collisionSphere, str);
                            }
                        break;
                        case YFromLocalAxis:
                            if (!str.Equals(collisionSphere.YFromLocalAxisString)){
                                YFromLocalAxisInstruction(collisionSphere,str);
                            }
                        break;
                        case XFromLocalAxis:
                            if (!str.Equals(collisionSphere.XFromLocalAxisString)){
                                XFromLocalAxisInstruction(collisionSphere,str);
                            }
                        break;
                        case radius:
                            if (!str.Equals(collisionSphere.radiusString)){
                                radiusInstruction(collisionSphere,str);
                            }
                        break;
                        case colorRGBA:
                            if (!str.Equals(collisionSphere.colorString)){
                                colorRGBAInstruction(collisionSphere,str);
                            }
                        break;
                    }
                }
            }
        }
        void distanceFromLocalAxisInstruction(CollisionSphere collisionSphere, string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                if (value.Count>= 1){
                    bool checkDistance = float.TryParse(value[0], out float distance);
                    bool checkSpeed = float.TryParse(value[1], out float speed);
                    bool checkAcceleration = float.TryParse(value[2], out float acceleration);
                    if (checkDistance && checkSpeed && checkAcceleration){
                        collisionSphere.aroundAxis.acceleration = acceleration;  
                        collisionSphere.aroundAxis.speed = speed;
                        collisionSphere.aroundAxis.distance = distance+speed;
                        collisionSphere.aroundAxis.scale(distance+speed);
                        collisionSphere.distanceFromLocalOriginString = str;
                    }
                }
            }
        }
        void XFromLocalAxisInstruction(CollisionSphere collisionSphere, string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                bool check = editor.jointInstructions.xAroundAxis(collisionSphere.aroundAxis,value);
                if (check) collisionSphere.XFromLocalAxisString = str;
            }
        }
        void YFromLocalAxisInstruction(CollisionSphere collisionSphere, string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                bool check = editor.jointInstructions.xAroundAxis(collisionSphere.aroundAxis,value);
                if (check) collisionSphere.YFromLocalAxisString = str;
            }
        }
        void radiusInstruction(CollisionSphere collisionSphere, string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
                if (value.Count>0){
                    bool checkRadius = float.TryParse(value[0], out float radius);
                    if (checkRadius) collisionSphere.aroundAxis.sphere.setRadius(radius);
                    collisionSphere.radiusString = str;
                }
            }
        }
        void colorRGBAInstruction(CollisionSphere collisionSphere, string str){
            if (str.Length>0){
                editor.removeEmpty(str.Split(" "), out List<string> value);
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
                    collisionSphere.colorString = str;
                }
            }
        }
    }
}