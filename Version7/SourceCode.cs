using System;
using System.Collections.Generic;
using System.IO;
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
        public Sphere origin,x,y,z,spin,move;
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
                spin = new Sphere(y.origin,1,new Color(0,0,0,0));
                move = new Sphere(y.origin,1,new Color(1.0f, 0.64f, 0.0f,0.0f));
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
                spin.destroySphere();
                move.destroySphere();
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
                     distance, distanceSpeed, distanceAcceleration,
                     speed, acceleration;
        
        public AroundAxis(){}
        public AroundAxis(Axis axis, Sphere sphere){
            this.sphere = sphere;
            this.axis = axis;
            angleY = 0;angleX = 0;
            sensitivitySpeedY = 0; sensitivityAccelerationY = 1;
            sensitivitySpeedX = 0; sensitivityAccelerationX = 1;
            speed = 0; acceleration = 1;
            distance = axis.axisDistance;
            distanceSpeed = 0; distanceAcceleration = 1;
            sphere.setOrigin(axis.origin + new Vector3(0,distance,0));
        }
        public void scale(float newDistance){
            bool check = Mathf.Sign(distance) >0;
            float dis = distance+newDistance;
            bool check2 = check? dis>0:dis<0;
            if (check2){
                    distance += newDistance;
                    sphere.origin = axis.origin + axis.distanceFromOrigin(sphere.origin,axis.origin,Mathf.Abs(distance));
                } else {
                    if (dis != 0) distance += newDistance;
                    distance *=-1;
                    sphere.origin = axis.origin + axis.distanceFromOrigin(axis.origin,sphere.origin,Mathf.Abs(distance));
                    get();
                }
            if (axis.renderAxis.created){
                updateAxis();
            }
        }

        public void get(){
            float sign = Mathf.Sign(angleY);
            float tempAngleX = angleX;
            axis.getPointAroundOrigin(sphere.origin,out angleY, out angleX);
            if (angleY == 0f || angleY == Mathf.PI) angleX = tempAngleX;
            if (sign<0 && Mathf.Sign(angleY) != sign) angleY *= sign;
            if (angleY == 2*Mathf.PI) angleY = 0;
            if (angleX == 2*Mathf.PI) angleX = 0;
        }

        void set(float angleY,float angleX){
            float sign = Mathf.Sign(this.angleY);
            sphere.origin = axis.setPointAroundOrigin(angleY,angleX,distance);
            this.angleY = (sign<0 && Mathf.Sign(angleY) != sign)? sign * angleY: angleY;
            this.angleX = angleX;
            if (axis.renderAxis.created){
                updateAxis();
            }
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

        public void rightDirection(){
            angleX += sensitivitySpeedX;
            if (angleX>2*Mathf.PI) angleX -= 2*Mathf.PI;
        }
        public void leftDirection(){
            angleX -= sensitivitySpeedX;
            if (angleX<0) angleX += 2*Mathf.PI;
        } 
        public void upDirection(){
            float sign = Mathf.Sign(angleY);
            angleY = sign*(Mathf.Abs(angleY) + sign*sensitivitySpeedY);
            if (Mathf.Abs(angleY)>Mathf.PI) {
                angleY = (sign>0)? angleY - 2*Mathf.PI: 2*Mathf.PI + angleY;
                angleX = Mathf.PI + angleX;
            }
            if (Mathf.Abs(angleY) +sign*sensitivitySpeedY<0){
                angleX = Mathf.PI + angleX;
                angleY *= -1;
            }
            if (angleX == 2*Mathf.PI) angleX = 0;
            if (angleX > 2*Mathf.PI) angleX -= 2*MathF.PI;
        }    
        public void downDirection(){
            float sign = Mathf.Sign(angleY);
            angleY = sign*(Mathf.Abs(angleY) - sign*sensitivitySpeedY);
            if (Mathf.Abs(angleY)>Mathf.PI) {
                angleY = (sign>0)? angleY - 2*Mathf.PI: 2*Mathf.PI + angleY;
                angleX = Mathf.PI + angleX;
            }
            if (Mathf.Abs(angleY) -sign*sensitivitySpeedY<0){
                angleX = Mathf.PI + angleX;
                angleY *= -1;
            }
            if (angleX == 2*Mathf.PI) angleX = 0;
            if (angleX > 2*Mathf.PI) angleX -= 2*MathF.PI;
        }  
        public void right(){
            rightDirection();
            set(angleY,angleX);
        }
        public void left(){
            leftDirection();
            set(angleY,angleX);  
        }
        public void up(){
            upDirection();
            set(angleY,angleX);
        }
        public void down(){
            downDirection();
            set(angleY,angleX);
        }
        
        public void scaleUp(){
            scale(distanceSpeed*distanceAcceleration);
        }
        public void scaleDown(){
            scale(-distanceSpeed*distanceAcceleration);
        }
        public void updatePhysics(){
            distanceSpeed *= distanceAcceleration;
            sensitivitySpeedY *= sensitivityAccelerationY;
            sensitivitySpeedX *= sensitivityAccelerationX;
            speed *= acceleration;
        }
        public void updateAxis(){
            sphere.setOrigin(sphere.origin);
        }
    }
    public class Axis {
        public RenderAxis renderAxis;
        public Vector3 origin,x,y,z;
        public float axisDistance;

        public float worldAngleY,worldSpeedY,
                     worldAngleX,worldSpeedX,
                     localAngleY,localSpeedY;
                     
        public AroundAxis spin,move;
        public Axis(){}
        public Axis(Vector3 origin, float distance){
            this.origin = origin;
            axisDistance = (distance >0.1f)? distance:1f;
            x = origin + new Vector3(distance,0,0);
            y = origin + new Vector3(0,distance,0);
            z = origin + new Vector3(0,0,distance);
            worldAngleY = 0; worldAngleX = 0; localAngleY = 0; 
            worldSpeedY = Mathf.PI/400f;
            worldSpeedX = Mathf.PI/400f;
            localSpeedY = Mathf.PI/400f;
            renderAxis = new RenderAxis(this);
            spin = new AroundAxis(this, renderAxis.spin);
            move = new AroundAxis(this, renderAxis.move);
        }
        
        public void moveAxis(Vector3 add){
            origin += add;
            x += add;
            y += add;
            z += add;
            spin.sphere.origin += add;
            move.sphere.origin += add;
            if (renderAxis.created){
                renderAxis.updateAxis();
                spin.updateAxis();
                move.updateAxis();
            }
        }
        public Vector3 placeAxis(Vector3 newOrigin){
            Vector3 newPosition = newOrigin-origin;
            moveAxis(newPosition);
            if (renderAxis.created){
                renderAxis.updateAxis();
                spin.updateAxis();
                move.updateAxis();
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
            Vector3 point = origin + distanceFromOrigin(y,origin,Mathf.Abs(distance));
            Vector4 rotY = angledAxis(Mathf.Abs(angleY),x);
            Vector4 rotX = angledAxis(Mathf.Abs(angleX),y);
            point = quatRotate(point,origin,rotY);
            point = quatRotate(point,origin,rotX);
            return point;
        }
        
        public void getPointAroundOrigin(Vector3 point, out float angleY,out float angleX){
            getAngle(point,origin,x,y,z,out angleY,out angleX);
            if (angleY == 0f || angleX == Mathf.PI) angleX = 0;
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
            float sign = Mathf.Sign(worldAngleY);
            getAngle(y,origin,worldX,worldY,worldZ,out worldAngleY,out float tempWorldAngleX);
            if (!(worldAngleY == 0f || worldAngleY == Mathf.PI)) worldAngleX = tempWorldAngleX;

            Vector3 localX = origin + new Vector3(axisDistance,0,0);
            Vector3 localY = origin + new Vector3(0,axisDistance,0);
            Vector3 localZ = origin + new Vector3(0,0,axisDistance);
            axisAlignment(
                Mathf.Abs(worldAngleY),Mathf.Abs(worldAngleX),0,
                worldX,worldY,ref localX,ref localY,ref localZ
                );   
            Vector3 dirLocalX = direction(localX,origin);
            Vector3 dirZ = direction(z,origin);
            Vector3 dirLocalZ = direction(localZ,origin);
            float angleSide = angleBetweenLines(dirZ,dirLocalX);
            localAngleY = (angleSide>Mathf.PI/2)? 
                2*Mathf.PI-angleBetweenLines(dirZ,dirLocalZ):
                angleBetweenLines(dirZ,dirLocalZ);
            
            if (sign<0 && Mathf.Sign(worldAngleY) != sign) worldAngleY*= sign;
            if (worldAngleY == 2*Mathf.PI) worldAngleX = 0;
            if (worldAngleX == 2*Mathf.PI) worldAngleX = 0;
            if (localAngleY == 2*Mathf.PI) localAngleY = 0;
        }
        public void setWorldRotation(float worldAngleY,float worldAngleX,float localAngleY){
            Vector3 worldX = origin + new Vector3(axisDistance,0,0);
            Vector3 worldY = origin + new Vector3(0,axisDistance,0);
            
            Vector3 localX = origin + new Vector3(axisDistance,0,0);
            Vector3 localY = origin + new Vector3(0,axisDistance,0);
            Vector3 localZ = origin + new Vector3(0,0,axisDistance);
            float sign = Mathf.Sign(this.worldAngleY);
            axisAlignment(
                Mathf.Abs(worldAngleY),Mathf.Abs(worldAngleX),Mathf.Abs(localAngleY),
                worldX,worldY,ref localX,ref localY,ref localZ
                );

            x = localX; y = localY; z = localZ;
            spin.setInRadians(spin.angleY,spin.angleX);
            move.setInRadians(move.angleY,move.angleX);
            if (renderAxis.created){
                renderAxis.updateAxis();
                spin.updateAxis();
                move.updateAxis();
            }
            this.worldAngleY = (sign<0 && Mathf.Sign(worldAngleY) != sign)? worldAngleY*sign: worldAngleY;
            this.worldAngleX = worldAngleX;
            this.localAngleY = localAngleY;
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
            getWorldRotation();
            spin.sphere.origin = quatRotate(spin.sphere.origin,rotationOrigin,quat);
            move.sphere.origin = quatRotate(move.sphere.origin,rotationOrigin,quat);
            if (renderAxis.created){
                renderAxis.updateAxis();
                spin.updateAxis();
                move.updateAxis();
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

        public void rightDirection(){
            worldAngleX += worldSpeedX;
            if (worldAngleX>2*Mathf.PI) worldAngleX -= 2*Mathf.PI;
        }
        public void leftDirection(){
            worldAngleX -= worldSpeedX;
            if (worldAngleX<0) worldAngleX += 2*Mathf.PI;
        } 
        public void upDirection(){
            float sign = Mathf.Sign(worldAngleY);
            worldAngleY = sign*(Mathf.Abs(worldAngleY) + sign*worldSpeedY);
            if (Mathf.Abs(worldAngleY)>Mathf.PI) {
                worldAngleY = (sign>0)? worldAngleY - 2*Mathf.PI: 2*Mathf.PI + worldAngleY;
                worldAngleX = Mathf.PI + worldAngleX;
                localAngleY = Mathf.PI + localAngleY;
            }
            if (Mathf.Abs(worldAngleY) +sign*worldSpeedY<0){
                worldAngleX = Mathf.PI + worldAngleX;
                localAngleY = Mathf.PI + localAngleY;
                worldAngleY *= -1;
            }
            if (worldAngleX == 2*Mathf.PI) worldAngleX = 0;
            if (worldAngleX > 2*Mathf.PI) worldAngleX -= 2*MathF.PI;
            if (localAngleY == 2*Mathf.PI) localAngleY = 0;
            if (localAngleY > 2*Mathf.PI) localAngleY -= 2*MathF.PI;
        }    
        public void downDirection(){
            float sign = Mathf.Sign(worldAngleY);
            worldAngleY = sign*(Mathf.Abs(worldAngleY) - sign*worldSpeedY);
            if (Mathf.Abs(worldAngleY)>Mathf.PI) {
                worldAngleY = (sign>0)? worldAngleY - 2*Mathf.PI: 2*Mathf.PI + worldAngleY;
                worldAngleX = Mathf.PI + worldAngleX;
                localAngleY = Mathf.PI + localAngleY;
            }
            if (Mathf.Abs(worldAngleY) -sign*worldSpeedY<0){
                worldAngleX = Mathf.PI + worldAngleX;
                localAngleY = Mathf.PI + localAngleY;
                worldAngleY *= -1;
            }
            if (worldAngleX == 2*Mathf.PI) worldAngleX = 0;
            if (worldAngleX > 2*Mathf.PI) worldAngleX -= 2*MathF.PI;
            if (localAngleY == 2*Mathf.PI) localAngleY = 0;
            if (localAngleY > 2*Mathf.PI) localAngleY -= 2*MathF.PI;
        }   

        public void up(){
            upDirection();
            setWorldRotation(worldAngleY,worldAngleX,localAngleY);
        }
        public void down(){
            downDirection();
            setWorldRotation(worldAngleY,worldAngleX,localAngleY);  
        }
        public void right(){
            rightDirection();
            setWorldRotation(worldAngleY,worldAngleX,localAngleY);
        }
        public void left(){
            leftDirection();
            setWorldRotation(worldAngleY,worldAngleX,localAngleY);
        }
        public void clockwise(){
            localAngleY += localSpeedY; 
            if (localAngleY>2*Mathf.PI) localAngleY -= 2*MathF.PI;
            setWorldRotation(worldAngleY,worldAngleX,localAngleY);
        }
        public void antiClockwise(){
            localAngleY -= localSpeedY;
            if (localAngleY<0) localAngleY += 2*MathF.PI;
            setWorldRotation(worldAngleY,worldAngleX,localAngleY);
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


    public class Editor {
        public Body body;
        public SaveBody saveBody;
        public Editor(){}
        public Editor(Body body){
            this.body = body;
            saveBody = new SaveBody(body);
            
        } 

        int oldOption = 0;
        int currentOption = 0;
        
        public void options(){
            for (int i = 0; i<10;i++){
                if (Input.GetKeyDown($"{i}")){
                    oldOption = currentOption;
                    currentOption = i;
                }
            }
            switch (currentOption){
                case 0:
                    worldAxisControls();
                    rotationAxisControls();
                    if (Input.GetKeyDown("b")) {
                        saveBody.reader();
                        saveBody.writer();
                    }
                    if (Input.GetKeyDown("v")) {
                        saveBody.reader();
                    }
                break;

                case 1:

                break;
            }
        }
        public void worldAxisControls(){
            if (Input.GetKey("i")) {
                body.globalAxis.up();
            }
            if (Input.GetKey("k")) {
                body.globalAxis.down();
            }
            if (Input.GetKey("l")) {       
                body.globalAxis.right();     
            }
            if (Input.GetKey("j")) {    
                body.globalAxis.left();        
            }
            if (Input.GetKey("o")) { 
                body.globalAxis.clockwise();           
            }
            if (Input.GetKey("u")) {
                body.globalAxis.antiClockwise(); 
            }
        }
        public void rotationAxisControls(){
            if (Input.GetKey("w")) {
                body.globalAxis.spin.up();
            }
            if (Input.GetKey("s")) {
                body.globalAxis.spin.down();
            }
            if (Input.GetKey("d")) {  
                body.globalAxis.spin.right();          
            }
            if (Input.GetKey("a")) { 
                body.globalAxis.spin.left();           
            }
            if (Input.GetKeyDown("e")) { 
                body.globalAxis.spin.scaleUp();           
            }
            if (Input.GetKeyDown("q")) {
                body.globalAxis.spin.scaleDown();
            }
            // if (Input.GetKeyDown("return")) {
            //     Vector3 newPoisition = jointSelector.selected.localAxis.move.placeAxis();
            //     jointSelector.collisionSphereSelector.moveCollisionSpheres(newPoisition);
            // }
        }
    }
    public class Body {
        public World world;
        public int worldKey;
        public Axis globalAxis;
        public Joint[] bodyStructure;
        public KeyGenerator keyGenerator;
        public Editor editor;

        public Body(){}
        public Body(int worldKey,Axis globalAxis, int amountOfJoints){
            this.worldKey = worldKey;
            this.globalAxis = globalAxis;
            bodyStructure = new Joint[amountOfJoints];
            keyGenerator = new KeyGenerator(amountOfJoints);
            editor = new Editor(this);
        }
        public Body(int worldKey){
            this.worldKey = worldKey;
            globalAxis = new Axis(new Vector3(0,0,0),5);
            bodyStructure = new Joint[0];
            keyGenerator = new KeyGenerator(0);
            editor = new Editor(this);
            editor.saveBody.reader();
        }

        public string saveBody(){
            Vector3 globalOrigin = globalAxis.origin;
            globalAxis.getWorldRotationInRadians(out float worldAngleY,out float worldAngleX,out float localAngleY);
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
            string bodyStructureSize = $"{stringPath}BodyStructureSize: {bodyStructure.Length}\n";
            string allJointsInBody = $"{stringPath}AllJointsInBody:{strJointIndexes}\n";
            string globalOriginLocation = $"{stringPath}globalOriginLocation: {globalOrigin.x} {globalOrigin.y} {globalOrigin.z}\n";
            string globalAxisRotationXYZ = $"{stringPath}globalAxisRotationXYZ: {worldAngleY} {worldAngleX} {localAngleY}\n";

            return bodyStructureSize + allJointsInBody + globalOriginLocation + globalAxisRotationXYZ;

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
        public List<Joint> getPastEnds(){
            List<Joint> joints = new List<Joint>();
            for (int i =0; i<bodyStructure.Length; i++){
                Joint joint = bodyStructure[i];
                if (joint != null){
                    if (joint.connection.past.Count == 0){
                        joints.Add(joint);
                    }
                }
            }
            return joints;
        }
        public Dictionary<int,int> optimizeBody(){
            int max = bodyStructure.Length;
            int newSize = max - keyGenerator.availableKeys;
            print($"{max} {keyGenerator.availableKeys}");
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

        internal void disconnectFuture(int index){
            future[index].connection.past.Remove(current);
        }
        public void disconnectAllFuture(){
            int size = future.Count;
            for (int i =0; i<size;i++){
                disconnectFuture(indexInBody);
            }
            future = new List<Joint>();
        }
        internal void disconnectPast(int index){
            past[index].connection.future.Remove(current);
        }
        public void disconnectAllPast(){
            int size = past.Count;
            for (int i =0; i<size;i++){
                disconnectPast(indexInBody);
            }
            past = new List<Joint>();
        }

        public void connectThisPastToFuture(Joint joint){
            List<Joint> connectTo = joint.connection.future;
            past.Add(joint);
            connectTo.Add(current);
        }
        public void connectThisFutureToPast(Joint joint){
            List<Joint> connectTo = joint.connection.past;
            future.Add(joint);
            connectTo.Add(current);
        }

        public List<Joint> nextConnections(bool pastOrFuture){
            List<Joint> connectedJoints = new List<Joint>();
            List<Joint> joints = pastOrFuture? future:past;
            int listSize = joints.Count;
            for (int j = 0;j<listSize;j++){
                Joint joint = joints[j];
                bool used = joint.connection.used;
                if (joint.connection.active && !used){
                    connectedJoints.Add(joint);
                    joint.connection.used = true;
                }
            }
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
                pastIndexes += $" {past[i].connection.indexInBody}";
            }
            return pastIndexes;
        }
        public string futureToString(){
            string futureIndexes = "";
            int count = future.Count;
            for (int i = 0; i<count;i++){
                futureIndexes += $" {future[i].connection.indexInBody}";
            }
            return futureIndexes;
        }
    }


    public class SaveBody{
        Body body;
        Dictionary<int,int> newJointKeys = new Dictionary<int,int>();
        Dictionary<int,int> newSphereKeys = new Dictionary<int,int>();
        public SaveBody(){}
        public SaveBody(Body body){
            this.body = body;
        }
        
        public void writer(){
            using(StreamWriter writetext = new StreamWriter($"Assets/v4/{body.worldKey}.txt")) {
                writetext.WriteLine(body.saveBody());
                int size = body.bodyStructure.Length;
                for (int i = 0; i<size; i++){
                    Joint joint = body.bodyStructure[i];
                        if (joint != null){
                        writetext.WriteLine(joint.saveJoint());
                        writetext.WriteLine(joint.pointCloud.savePointCloud(out List<int> collisionSphereIndexes, out int listSize));
                        CollisionSphere[] collisionSpheres = joint.pointCloud.collisionSpheres;
                        for (int j = 0; j<listSize;j++){
                            writetext.WriteLine(collisionSpheres[collisionSphereIndexes[j]].saveCollisionSphere());
                        }
                    }
                }
            }
        }
        public void reader(){
            using(StreamReader readtext = new StreamReader($"Assets/v4/{body.worldKey}.txt")){
                string readText;
                while ((readText = readtext.ReadLine()) != null){
                    string[] splitStr = readText.Split(": ");
                    if (splitStr.Length == 2){
                        removeEmpty(splitStr[0].Split("_"), out List<string> instruction, out int instructionSize);
                        removeEmpty(splitStr[1].Split(" "), out List<string> values, out int valueSize);
                        if (instructionSize == 3){
                            bodyInstructions(instruction[2],values);
                        } else if (instructionSize == 5){
                            jointInstructions(instruction[3],instruction[4],values);

                        } else if (instructionSize == 7){
                            sphereInstructions(instruction[3],instruction[5],instruction[6],values);
                        }
                    }
                }
            }
            body.updatePhysics();
        }
        void removeEmpty(string[] strArray, out List<string> list, out int size){
            list = new List<string>();
            size = 0;
            int arraySize = strArray.Length;
            for (int i = 0; i < arraySize; i++){
                string str = strArray[i];
                if (str != ""){
                    list.Add(str);
                    size++;
                }
            }
        }

        const string bodyStructureSize = "BodyStructureSize";
        const string allJointsInBody = "AllJointsInBody";
        const string globalOriginLocation = "GlobalOriginLocation";
        const string globalAxisRotationXYZ = "GlobalAxisRotationXYZ";

        public void bodyInstructions(string instruction, List<string> value){
            switch (instruction){
                case bodyStructureSize:
                    bodyStructureSizeInstruction(value);
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
            }
        }
        void bodyStructureSizeInstruction(List<string> value){
            if (value.Count>0){
                bool check = int.TryParse(value[0], out int amount);
                if (check) newJointKeys = body.arraySizeManager(amount); 
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
                float vecX = checkX? y: body.globalAxis.origin.x;
                float vecY = checkY? x: body.globalAxis.origin.y;
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
                float worldAngleY = checkX? y: body.globalAxis.worldAngleY;
                float worldAngleX = checkY? x: body.globalAxis.worldAngleX;
                float localAngleY = checkLY? ly: body.globalAxis.localAngleY;
                body.globalAxis.setWorldRotation(worldAngleY,worldAngleX,localAngleY);
            }            
        }

        const string active = "Active";
        const string distanceFromGlobalOrigin = "DistanceFromGlobalOrigin";
        const string YXFromGlobalAxis = "YXFromGlobalAxis";
        const string localAxisRotation = "LocalAxisRotation";
        const string spinX = "SpinX";
        const string spinY = "SpinY";
        const string spinSpeedAndAcceleration = "SpinSpeedAndAcceleration";
        const string moveX = "MoveX";
        const string moveY = "MoveY";
        const string moveSpeedAndAcceleration = "MoveSpeedAndAcceleration";
        const string pastConnectionsInBody = "PastConnectionsInBody";
        const string futureConnectionsInBody = "FutureConnectionsInBody";
        const string pointCloudSize = "PointCloudSize";
        const string allSpheresInJoint = "AllSpheresInJoint";

        public void jointInstructions(string jointKey, string instruction, List<string> value){
            bool checkKey = int.TryParse(jointKey, out int key);
            if (newJointKeys.TryGetValue(key, out int newKey)){
                key = newKey;
            }
            Joint joint = checkKey? body.bodyStructure[key]:null;
            if (joint != null) {
                switch (instruction){
                    case active:
                        activeInstruction(joint,value);
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
                    case spinX:
                        spinXInstruction(joint,value);
                    break;
                    case spinY:
                        spinYInstruction(joint,value);
                    break;
                    case spinSpeedAndAcceleration:
                        spinSpeedAndAccelerationInstruction(joint,value);
                    break;
                    case moveX:
                        moveXInstruction(joint,value);
                    break;
                    case moveY:
                        moveYInstruction(joint,value);
                    break;
                    case moveSpeedAndAcceleration:
                        moveSpeedAndAccelerationInstruction(joint,value);
                    break;                
                    case pastConnectionsInBody:
                        pastConnectionsInBodyInstruction(joint,value);
                    break;
                    case futureConnectionsInBody:
                        futureConnectionsInBodyInstruction(joint,value);
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
        void distanceFromGlobalOriginInstruction(Joint joint,List<string> value){
            if (value.Count>0){
                bool checkStr = float.TryParse(value[0], out float key);
                if (checkStr){
                    joint.distanceFromGlobalOrigin(key);
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
                if (checkY && checkX) {
                        if (length>0){
                            if (float.IsNaN(y)) y = 0;
                            if (float.IsNaN(x)) x = 0;
                            joint.moveJoint(globalAxis.setPointAroundOrigin(y,x,length) - localAxis.origin);
                        } else {
                            joint.moveJoint(localAxis.origin-globalAxis.origin);
                        }
                    } 
            }
        }
        void localAxisRotationInstruction(Joint joint,List<string> value){
            if (value.Count>=3){
                bool checkY = float.TryParse(value[0], out float y);
                bool checkX = float.TryParse(value[1], out float x);
                bool checkLY = float.TryParse(value[2], out float ly);
                if (checkY && checkX && checkLY) {
                    joint.worldRotateJoint(y,x,ly);
                } else{
                    if (!checkY) y = joint.localAxis.worldAngleY;
                    if (!checkX) x = joint.localAxis.worldAngleX;
                    if (!checkLY) ly = joint.localAxis.localAngleY;
                    joint.worldRotateJoint(y,x,ly);
                }
            }
        }
        void spinXInstruction(Joint joint,List<string> value){
            xAroundAxis(joint.localAxis.spin,value);
        }
        void spinYInstruction(Joint joint,List<string> value){
            yAroundAxis(joint.localAxis.spin,value);
        }
        void spinSpeedAndAccelerationInstruction(Joint joint,List<string> value){
            if (value.Count>=2){
                AroundAxis aroundAxis = joint.localAxis.spin;
                bool checkSpeed = float.TryParse(value[0], out float speed);
                bool checkAcceleration = float.TryParse(value[1], out float acceleration);     
                if (checkSpeed) aroundAxis.speed = speed;
                if (checkAcceleration) aroundAxis.acceleration = acceleration;
                joint.rotateFutureHierarchy();
            }
        }
        void xAroundAxis(AroundAxis aroundAxis, List<string> value){
            if (value.Count>=3){
                bool checkX = float.TryParse(value[0], out float angleX);
                bool checkSpeedX = float.TryParse(value[1], out float speedX);
                bool checkAccelerationX = float.TryParse(value[2], out float accelerationX);
                if (checkSpeedX) aroundAxis.sensitivitySpeedX = speedX;
                if (checkAccelerationX) aroundAxis.sensitivityAccelerationX = accelerationX;
                if (checkX && angleX != aroundAxis.angleY) aroundAxis.setInRadians(aroundAxis.angleY,angleX);  
                float direction = aroundAxis.sensitivitySpeedX*aroundAxis.sensitivityAccelerationX;  
                if (direction > 0) {
                    aroundAxis.right();
                } else if (direction < 0) {
                    aroundAxis.left();
                }
            }
        }
        void yAroundAxis(AroundAxis aroundAxis, List<string> value){
            if (value.Count>=3){
                bool checkY = float.TryParse(value[0], out float angleY);
                bool checkSpeedY = float.TryParse(value[1], out float speedY);
                bool checkAccelerationY = float.TryParse(value[2], out float accelerationY);     
                if (checkSpeedY) aroundAxis.sensitivitySpeedY = speedY;
                if (checkAccelerationY) aroundAxis.sensitivityAccelerationY = accelerationY;
                if (checkY && angleY != aroundAxis.angleY) aroundAxis.setInRadians(angleY,aroundAxis.angleX);  
                float direction = aroundAxis.sensitivitySpeedY*aroundAxis.sensitivityAccelerationY;              
                if (direction > 0) {
                    aroundAxis.up();
                } else if (direction < 0) {
                    aroundAxis.down();
                }  
            }
        }
        void moveXInstruction(Joint joint,List<string> value){
            xAroundAxis(joint.localAxis.move,value);
        }
        void moveYInstruction(Joint joint,List<string> value){
            yAroundAxis(joint.localAxis.move,value);
        }
        void moveSpeedAndAccelerationInstruction(Joint joint,List<string> value){
            if (value.Count>=2){
                AroundAxis aroundAxis = joint.localAxis.move;
                bool checkSpeed = float.TryParse(value[0], out float speed);
                bool checkAcceleration = float.TryParse(value[1], out float acceleration);     
                if (checkSpeed) aroundAxis.speed = speed;
                if (checkAcceleration) aroundAxis.acceleration = acceleration;
                if (Mathf.Abs(speed*acceleration)>0) joint.moveAllHierarchy();
                print(speed);
            }
        }
        void pastConnectionsInBodyInstruction(Joint joint,List<string> value){
            HashSet<int> set = resizeBody(value, out _);
            List<Joint> past = joint.connection.past;
            int size = past.Count;
            List<Joint> newPast = new List<Joint>();
            bool checkChange = false;
            
            for (int i = 0; i< size; i++){
                int index = past[i].connection.indexInBody;
                if (!set.Contains(index)) {
                    joint.connection.disconnectPast(i);
                    checkChange = true;
                } else {
                    newPast.Add(body.bodyStructure[index]);
                    set.Remove(index);
                    }
            }
            if (checkChange || size != set.Count){
                foreach (int i in set){
                    Joint newjoint = new Joint(body,i);
                    newjoint.connection.connectThisFutureToPast(joint);
                    newPast.Add(newjoint);
                }
                joint.connection.past = newPast;
            }
        }
        void futureConnectionsInBodyInstruction(Joint joint,List<string> value){
            HashSet<int> set = resizeBody(value, out _);
            List<Joint> future = joint.connection.future;
            int size = future.Count;
            List<Joint> newPast = new List<Joint>();
            bool checkChange = false;
            
            for (int i = 0; i< size; i++){
                int index = future[i].connection.indexInBody;
                if (!set.Contains(index)) {
                    joint.connection.disconnectFuture(i);
                    checkChange = true;
                } else {
                    newPast.Add(body.bodyStructure[index]);
                    set.Remove(index);
                    }
            }
            if (checkChange || size != set.Count){
                foreach (int i in set){
                    Joint newjoint = new Joint(body,i);
                    newjoint.connection.connectThisPastToFuture(joint);
                    newPast.Add(newjoint);
                }
                joint.connection.past = newPast;
            }
        }
        void pointCloudSizeInstruction(Joint joint,List<string> value){
            if (value.Count>=0){
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
                if (check && !set.Contains(key)){
                    if (newSphereKeys.TryGetValue(key, out int newKey)){
                        key = newKey;
                    }
                    set.Add(key);
                    if (key >= pointCloud.collisionSpheres.Length){
                        nullKeys.Add(key);
                        nullCount++;
                    } else if (pointCloud.collisionSpheres[i] == null){
                        nullKeys.Add(key);
                        nullCount++;
                    } 
                    if (key > maxKey) maxKey = key;
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

        const string distanceFromLocalAxis = "DistanceFromLocalAxis";
        const string XFromLocalAxis = "XFromLocalAxis";
        const string YFromLocalAxis = "YFromLocalAxis";
        const string radius = "Radius";
        const string colorRGBA = "ColorRGBA";
 
        public void sphereInstructions(string jointKey,string collisionSphereKey, string instruction, List<string> value){
            bool checkKey = int.TryParse(jointKey, out int key);
            if (newSphereKeys.TryGetValue(key, out int newKey)){
                key = newKey;
            }
            Joint joint = checkKey? body.bodyStructure[key]:null;
            if (joint != null) { 
                bool checkKey2 = int.TryParse(collisionSphereKey, out int key2);
                CollisionSphere collisionSphere = checkKey2?joint.pointCloud.collisionSpheres[key2]:null;
                if (collisionSphere != null){
                    switch (instruction){
                        case distanceFromLocalAxis:  
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
                 if (checkDistance){
                    collisionSphere.aroundAxis.scale(distance);
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


    public class Joint {
        public Body body;
        public Axis localAxis;
        public Connection connection;
        public PointCloud pointCloud;

        public Joint(){}
        public Joint(Body body, int indexInBody){
            this.body = body;
            localAxis = new Axis(new Vector3(0,0,0),5);
            connection = new Connection(indexInBody);
            pointCloud = new PointCloud(this,0);
            body.keyGenerator.getKey();
        }

        public void setBody(Body body){
            this.body=body;
        }
        public string saveJoint(){
            Vector3 jointOrigin = localAxis.origin;
            Vector3 globalOrigin = body.globalAxis.origin;
            body.globalAxis.getPointAroundOrigin(jointOrigin,out float globalY,out float globalX);
            localAxis.getWorldRotationInRadians(out float worldAngleY,out float worldAngleX,out float localAngleY);
            float distanceFromOrigin = body.globalAxis.length(jointOrigin-globalOrigin);
            string stringPath = $"Body_{body.worldKey}_Joint_{connection.indexInBody}_";
            string active = $"{stringPath}Active: {connection.active}\n";
            string distanceFromGlobalOrigin = $"{stringPath}DistanceFromGlobalOrigin: {distanceFromOrigin}\n";
            string YXFromGlobalAxis = $"{stringPath}YXFromGlobalAxis: {globalY} {globalX}\n";
            string localAxisRotation = $"{stringPath}LocalAxisRotation: {worldAngleY} {worldAngleX} {localAngleY}\n";
            string spinX = $"{stringPath}SpinX: {localAxis.spin.angleX} {localAxis.spin.sensitivitySpeedX} {localAxis.spin.sensitivityAccelerationX}\n";
            string spinY = $"{stringPath}SpinY: {localAxis.spin.angleY} {localAxis.spin.sensitivitySpeedY} {localAxis.spin.sensitivityAccelerationY}\n";
            string spinSpeedAndAcceleration = $"{stringPath}SpinSpeedAndAcceleration: {localAxis.spin.speed} {localAxis.spin.acceleration}\n";
            string moveX = $"{stringPath}MoveX: {localAxis.move.angleX} {localAxis.move.sensitivitySpeedX} {localAxis.move.sensitivityAccelerationX}\n";
            string moveY = $"{stringPath}MoveY: {localAxis.move.angleY} {localAxis.move.sensitivitySpeedY} {localAxis.move.sensitivityAccelerationY}\n";
            string moveSpeedAndAcceleration = $"{stringPath}MoveSpeedAndAcceleration: {localAxis.move.speed} {localAxis.move.acceleration}\n";
            string pastConnectionsInBody = $"{stringPath}PastConnectionsInBody: {connection.pastToString()}\n";
            string futureConnectionsInBody = $"{stringPath}FutureConnectionsInBody: {connection.futureToString()}\n";
            return active + distanceFromGlobalOrigin + 
                YXFromGlobalAxis + localAxisRotation + 
                spinX + spinY + spinSpeedAndAcceleration +
                moveX + moveY + moveSpeedAndAcceleration +
                pastConnectionsInBody + futureConnectionsInBody;
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
            pointCloud.rotateAllSpheres(quat,rotationOrigin);
        }
        public void worldRotateJoint(float worldAngleY,float worldAngleX,float localAngleY){
            localAxis.setWorldRotation(worldAngleY,worldAngleX,localAngleY);
            pointCloud.resetAllSphereOrigins();
        }

        public void updatePhysics(){
            localAxis.spin.updatePhysics();
            localAxis.move.updatePhysics();
            pointCloud.updatePhysics();     
        }

        public void rotatePastHierarchy(){
            Vector4 quat = localAxis.spin.quat(localAxis.spin.speed);
            rotateHierarchy(quat, false,false);
        }
        public void rotateFutureHierarchy(){
            Vector4 quat = localAxis.spin.quat(localAxis.spin.speed);
            rotateHierarchy(quat, true,false);
        }
        public void rotateAllHierarchy(){
            Vector4 quat = localAxis.spin.quat(localAxis.spin.speed);
            rotateHierarchy(quat, false,true);
        }
        void rotateHierarchy(Vector4 quat, bool pastOrFuture, bool rotateAll){
            initTree(pastOrFuture, rotateAll, out List<Joint> tree, out int size);  
            Vector3 rotationOrigin = localAxis.origin;
            for (int i = 0; i<size;i++){
                Joint joint = tree[i];
                List<Joint> joints = joint.connection.getAll();
                tree.AddRange(joints);
                size += joints.Count;
                joint.rotateJoint(quat,rotationOrigin);
            }
            resetUsed(tree,size);
        }

        public void movePastHierarchy(){
            Vector3 move = localAxis.move.sphere.origin - localAxis.origin;
            moveHierarchy(move, false,false);
        }
        public void moveFutureHierarchy(){
            Vector3 move = localAxis.move.sphere.origin - localAxis.origin;
            moveHierarchy(move, true,false);
        }
        public void moveAllHierarchy(){
            Vector3 move = localAxis.move.sphere.origin - localAxis.origin;
            moveHierarchy(move, false,true);
        }
        void moveHierarchy(Vector3 newVec, bool pastOrFuture, bool rotateAll){
            initTree(pastOrFuture, rotateAll, out List<Joint> tree, out int size);  
            for (int i = 0; i<size;i++){
                Joint joint = tree[i];
                List<Joint> joints = joint.connection.getAll();
                tree.AddRange(joints);
                size += joints.Count;
                joint.moveJoint(newVec);
            }
            resetUsed(tree,size);
        }

        void initTree(bool pastOrFuture,bool rotateAll, out List<Joint> tree,out int size){
            tree = new List<Joint>{this};
            if (rotateAll) 
                tree.AddRange(connection.getAll()); 
                else if (pastOrFuture) 
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
                collisionSpheres[i]?.aroundAxis.updatePhysics();
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
            int used = keyGenerator.availableKeys;
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

    public class CollisionSphere {
        public Path path;
        public AroundAxis aroundAxis;

        public CollisionSphere(){}
        public CollisionSphere(Joint joint, int sphereIndex){
            path = new Path(joint.body,joint,sphereIndex);
            aroundAxis = new AroundAxis(joint.localAxis,new Sphere());
            joint.pointCloud.keyGenerator.getKey();
        }
   
        public string saveCollisionSphere(){
            Sphere sphere = aroundAxis.sphere;
            string stringPath = $"Body_{path.body.worldKey}_Joint_{path.joint.connection.indexInBody}_Sphere_{path.collisionSphereKey}_";
            string distanceFromLocalOrigin = $"{stringPath}DistanceFromLocalOrigin: {aroundAxis.distance} {aroundAxis.distanceSpeed} {aroundAxis.distanceAcceleration}\n";
            string YFromLocalAxis = $"{stringPath}YFromLocalAxis: {aroundAxis.angleY} {aroundAxis.sensitivitySpeedY} {aroundAxis.sensitivityAccelerationY}\n";
            string XFromLocalAxis = $"{stringPath}XFromLocalAxis: {aroundAxis.angleX} {aroundAxis.sensitivitySpeedX} {aroundAxis.sensitivityAccelerationX}\n";
            string radius = $"{stringPath}Radius: {sphere.radius}\n";
            string color = $"{stringPath}ColorRGBA: {sphere.color.r} {sphere.color.g} {sphere.color.b} {sphere.color.a}\n";
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
            aroundAxis.updatePhysics();
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
        public Sphere(Vector3 origin, float radius){
            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.GetComponent<Collider>().enabled = false;
            setOrigin(origin);
            setRadius(radius);
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
