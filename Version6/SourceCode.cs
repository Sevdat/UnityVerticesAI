using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class SourceCode:MonoBehaviour {
// 
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
        public float axisDistance,rotationAxisDistance;

        public float worldAngleY,worldSpeedY,
                     worldAngleX,worldSpeedX,
                     localAngleY,localSpeedY;
              
        public float angleY,ySpeed,
                     angleX,xSpeed;

        public Axis(){}
        public Axis(Vector3 origin, float distance){
            this.origin = origin;
            axisDistance = (distance >0.1f)? distance:1f;
            rotationAxisDistance = axisDistance;
            x = origin + new Vector3(distance,0,0);
            y = origin + new Vector3(0,distance,0);
            z = origin + new Vector3(0,0,distance);
            rotationAxis = origin + new Vector3(0,distance,0);
            renderAxis = new RenderAxis(this);
            worldAngleY = 0; worldAngleX = 0; localAngleY = 0; 
            angleY = 0;angleX = 0;
            xSpeed = Mathf.PI/400f;
            ySpeed = Mathf.PI/400f;
            worldSpeedY = Mathf.PI/400f;
            worldSpeedX = Mathf.PI/400f;
            localSpeedY = Mathf.PI/400f;
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
        public Vector3 placeAxis(Vector3 newOrigin){
            Vector3 newPosition = newOrigin-origin;
            moveAxis(newPosition);
            if (renderAxis.created){
                renderAxis.updateAxis();
                renderAxis.updateRotationAxis();
            }
            return newPosition;
        }
        public void scaleAxis(float newDistance){
            if (newDistance > 0f){
                axisDistance = newDistance;
                x = origin + distanceFromOrign(x,origin,axisDistance);
                y = origin + distanceFromOrign(y,origin,axisDistance);
                z = origin + distanceFromOrign(z,origin,axisDistance);
                if (renderAxis.created){
                    renderAxis.updateAxis();
                }
            }
        }
        public void scaleRotationAxis(float newDistance){
            if (newDistance > 0f){
                rotationAxisDistance = newDistance;
                rotationAxis = origin + distanceFromOrign(rotationAxis,origin,rotationAxisDistance);
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
        internal Vector3 distanceFromOrign(Vector3 point,Vector3 origin, float distance){
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
        internal void getAngle(Vector3 point,Vector3 origin, Vector3 x, Vector3 y, Vector3 z, out float yAngle,out float xAngle){
            Vector3 dirX = direction(x,origin);
            Vector3 dirY = direction(y,origin);
            Vector3 dirZ = direction(z,origin);
            Vector3 dirH = direction(point,origin);
            yAngle = angleBetweenLines(dirY,dirH);

            if (float.IsNaN(yAngle)) xAngle = float.NaN; else
            if (yAngle == 0f || yAngle == Mathf.PI) 
                xAngle = worldAngleX; 
            else {   
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

            getAngle(y,origin,worldX,worldY,worldZ,out worldAngleY,out worldAngleX);
            Vector3 localX = origin + new Vector3(axisDistance,0,0);
            Vector3 localY = origin + new Vector3(0,axisDistance,0);
            Vector3 localZ = origin + new Vector3(0,0,axisDistance);
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
        }
        public void setWorldRotation(float worldAngleY,float worldAngleX,float localAngleY){
            Vector3 worldX = origin + new Vector3(axisDistance,0,0);
            Vector3 worldY = origin + new Vector3(0,axisDistance,0);
            
            Vector3 localX = origin + new Vector3(axisDistance,0,0);
            Vector3 localY = origin + new Vector3(0,axisDistance,0);
            Vector3 localZ = origin + new Vector3(0,0,axisDistance);
            axisAlignment(
                worldAngleY,worldAngleX,localAngleY,
                worldX,worldY,ref localX,ref localY,ref localZ
                );

            x = localX; y = localY; z = localZ;
            setRotationAxis(angleY,angleX);
            if (renderAxis.created){
                renderAxis.updateAxis();
                renderAxis.updateRotationAxis();
            }
            this.worldAngleY = worldAngleY;
            this.worldAngleX = worldAngleX;
            this.localAngleY = localAngleY;
        }
        public void getRotationAxis(){
            getAngle(rotationAxis,origin,x,y,z,out angleY,out angleX);
        }
        public void setRotationAxis(float angleY,float angleX){
            rotationAxis = origin + distanceFromOrign(y,origin,rotationAxisDistance);
            Vector4 rotY = angledAxis(angleY,y);
            Vector4 rotX = angledAxis(angleX,x);
            rotationAxis = quatRotate(rotationAxis,origin,rotX);
            rotationAxis = quatRotate(rotationAxis,origin,rotY);
            this.angleY = angleY;
            this.angleX = angleX;
            if (renderAxis.created){
                renderAxis.updateRotationAxis();
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
        public void getRotationAxisInRadians(out float angleY,out float angleX){
            getRotationAxis();
            angleY = this.angleY;
            angleX = this.angleX;
        }
        public void setRotationAxisInRadians(float angleY,float angleX){
            setRotationAxis(angleY, angleX);
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
        public void getRotationAxisInDegrees(out float angleY,out float angleX){
            float radianToDegree = 180/Mathf.PI;
            getRotationAxis();
            angleY = this.angleY*radianToDegree;
            angleX = this.angleX*radianToDegree;
        }
        public void setRotationAxisInDegrees(float angleY,float angleX){
            float degreeToRadian = Mathf.PI/180;
            angleY *= degreeToRadian;
            angleX *= degreeToRadian;
            setRotationAxis(angleY, angleX);
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
            getWorldRotation();
            rotationAxis = quatRotate(rotationAxis,rotationOrigin,quat);
            if (renderAxis.created){
                renderAxis.updateAxis();
                renderAxis.updateRotationAxis();
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
        public void worldAxisUp(){
            worldAngleY += worldSpeedY;
            if (worldAngleY>Mathf.PI) worldAngleY -= 2*MathF.PI;
            setWorldRotation(worldAngleY,worldAngleX,localAngleY);
        }
        public void worldAxisDown(){
            worldAngleY -= worldSpeedY;
            if (worldAngleY<0) worldAngleY += 2*MathF.PI;
            setWorldRotation(worldAngleY,worldAngleX,localAngleY);          
        }
        public void worldAxisRight(){
            worldAngleX += worldSpeedX;
            if (worldAngleX>Mathf.PI) worldAngleX -= 2*MathF.PI;
            setWorldRotation(worldAngleY,worldAngleX,localAngleY);
        }
        public void worldAxisLeft(){
            worldAngleX -= worldSpeedX;
            if (worldAngleX<0) worldAngleX += 2*MathF.PI;
            setWorldRotation(worldAngleY,worldAngleX,localAngleY);
        }
        public void worldAxisRotateClockwise(){
            localAngleY += localSpeedY; 
            if (localAngleY>Mathf.PI) localAngleY -= 2*MathF.PI;
            setWorldRotation(worldAngleY,worldAngleX,localAngleY);
        }
        public void worldAxisRotateAntiClockwise(){
            localAngleY -= localSpeedY;
            if (localAngleY<0) localAngleY += 2*MathF.PI;
            setWorldRotation(worldAngleY,worldAngleX,localAngleY);
        }
        
        public void rotationAxisUp(){
            angleX += xSpeed;
            if (angleX>Mathf.PI) angleX -= 2*MathF.PI;
            setRotationAxis(angleY,angleX);
        }
        public void rotationAxisDown(){
            angleX -= xSpeed;
            if (angleX<0) angleX += 2*MathF.PI;
            setRotationAxis(angleY,angleX);          
        }
        public void rotationAxisRight(){
            angleY += ySpeed;
            if (angleY>Mathf.PI) angleY -= 2*MathF.PI;
            setRotationAxis(angleY,angleX);
        }
        public void rotationAxisLeft(){
            angleY -= ySpeed;
            if (angleY<0) angleY += 2*MathF.PI;
            setRotationAxis(angleY,angleX);
        }
        public void rotationAxisScaleUp(){
            scaleRotationAxis(rotationAxisDistance + 1);
        }
        public void rotationAxisScaleDown(){
            scaleRotationAxis(rotationAxisDistance - 1);
        }
        public Vector3 placeAxis(){
            return placeAxis(rotationAxis);
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
    public class JointSelector{
        public Editor editor;
        public CollisionSphereSelector collisionSphereSelector;
        public Joint selected;
        public Joint connectTo;
        public List<Joint> joints;
        public int size;
        public int index;
        public Color red = new Color(1, 0, 0, 1);
        public Color green = new Color(0,1,0,1);
        public Color blue = new Color(0,0,1,1);
        public Color yellow = new Color(1, 1, 0, 1);

        public JointSelector(){}
        public JointSelector(Editor editor,List<Joint> joints){
            this.editor = editor;
            this.joints = joints;
            size = joints.Count;
            index = 0;
            if (size >0) selected = joints[index];
            selected.pointCloud.updateAllSphereColors(green);
            collisionSphereSelector = new CollisionSphereSelector(this);
        }   
             
        public void selectJoint(){
            deselectJoints();
            List<Joint> newJoints = new List<Joint>();
            List<Joint> selectedPast = selected.connection.past;
            List<Joint> selectedFuture = selected.connection.future;
            newJoints.AddRange(selectedPast);
            newJoints.Add(selected);
            newJoints.AddRange(selectedFuture);
            index = selectedPast.Count;
            size = newJoints.Count;
            for (int i = 0; i<size;i++){
                if (i != index) joints[i].pointCloud.updateAllSphereColors(green);
            }
        }
        void deselectJoints(){
            for (int i = 0; i<size;i++){
                if (i != index) joints[i].pointCloud.resetAllSphereColors(); 
            }            
        }
        
        void reColor(){
            selected.pointCloud.updateAllSphereColors(green);
            selected = joints[index];
            collisionSphereSelector.renew();
            selected.pointCloud.updateAllSphereColors(yellow);
        }
        public void nextJoint(){
            if (index +1 <size) { index++; reColor(); }
        }
        public void previousJoint(){
            if (index-1>-1) { index--; reColor(); }         
        }
        public void createJoint(){
            selected.pointCloud.updateAllSphereColors(green);
            selected = selected.createJoint();
            joints.Add(selected);
            index = size;
            size++;
            collisionSphereSelector.renew();
            collisionSphereSelector.size = 0;
            collisionSphereSelector.index = 0;
        }

        public void jointSelectorControls(){
            if (Input.GetKeyDown("right")) {
                previousJoint();
            }
            if (Input.GetKeyDown("left")) {
                nextJoint();
            }
            if (Input.GetKeyDown("enter")) {
                selectJoint();
            } 
        }

    }
    public class CollisionSphereSelector{
        public JointSelector jointSelector;
        public CollisionSphere selected;
        public List<CollisionSphere> collisionSpheres;
        public int size;
        public int index;

        public CollisionSphereSelector(){}
        public CollisionSphereSelector(JointSelector jointSelector){
            this.jointSelector = jointSelector;
            collisionSpheres = jointSelector.selected.pointCloud.arrayToList();
            size = collisionSpheres.Count;
            index = 0;
        }

        public void renew(){
            collisionSpheres = jointSelector.selected.pointCloud.arrayToList();
            int count = collisionSpheres.Count;
            if (count >0){
                selected = collisionSpheres[0];
                selected.sphere.updateColor(jointSelector.blue);
            }
        }
        public void createCollisionSphere(){
            collisionSpheres.Add(jointSelector.selected.pointCloud.createSphere());
            index = size;
            size += 1;
            if (selected != null) recolor(); else {
                selected = collisionSpheres[index];
                selected.sphere.updateColor(jointSelector.blue);
            }
        }
        public void deleteCollisionSphere(){
            if (size-1>-1) {
                collisionSpheres.RemoveAt(index);
                jointSelector.selected.pointCloud.deleteSphere(selected.path.collisionSphereKey);
                index -= 1;
                size -= 1;
                if (index >-1){
                    selected = collisionSpheres[index];
                    selected.sphere.updateColor(jointSelector.blue);
                } else selected = null;
            }
        }

        void recolor(){
            selected.sphere.updateColor(jointSelector.yellow);
            selected = collisionSpheres[index];
            selected.sphere.updateColor(jointSelector.blue);
        }
        public void nextCollisionSphere(){
            if (index+1<size) { index++; recolor(); }
        }
        public void previousCollisionSphere(){
            if (index-1>-1) { index--; recolor(); }         
        }

        public void moveCollisionSpheres(Vector3 add){
            for (int i = 0; i<size;i++){
                collisionSpheres[i].moveOrigin(add);
            }
        }
    }

    public class Editor {
        public Body body;
        public JointSelector jointSelector;

        public Editor(){}
        public Editor(Body body){
            this.body = body;
            jointSelector = new JointSelector(this,body.getPastEnds());
            
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
                    jointSelectorControls();
                    collisionSphereSelectorControls();
                break;

                case 1:

                break;
            }
        }
        public void worldAxisControls(){
            if (Input.GetKey("i")) {
                jointSelector.selected.localAxis.worldAxisUp();
            }
            if (Input.GetKey("k")) {
                jointSelector.selected.localAxis.worldAxisDown();
            }
            if (Input.GetKey("l")) {       
                jointSelector.selected.localAxis.worldAxisLeft();     
            }
            if (Input.GetKey("j")) {    
                jointSelector.selected.localAxis.worldAxisRight();        
            }
            if (Input.GetKey("o")) { 
                jointSelector.selected.localAxis.worldAxisRotateClockwise();           
            }
            if (Input.GetKey("u")) {
                jointSelector.selected.localAxis.worldAxisRotateAntiClockwise(); 
            }
        }
        public void rotationAxisControls(){
            if (Input.GetKey("w")) {
                jointSelector.selected.localAxis.rotationAxisUp();
            }
            if (Input.GetKey("s")) {
                jointSelector.selected.localAxis.rotationAxisDown();
            }
            if (Input.GetKey("d")) {  
                jointSelector.selected.localAxis.rotationAxisRight();          
            }
            if (Input.GetKey("a")) { 
                jointSelector.selected.localAxis.rotationAxisLeft();           
            }
            if (Input.GetKeyDown("e")) { 
                jointSelector.selected.localAxis.rotationAxisScaleUp();           
            }
            if (Input.GetKeyDown("q")) {
                jointSelector.selected.localAxis.rotationAxisScaleDown();
            }
            if (Input.GetKeyDown("return")) {
                Vector3 newPoisition = jointSelector.selected.localAxis.placeAxis();
                jointSelector.collisionSphereSelector.moveCollisionSpheres(newPoisition);
            }
        }
        public void jointSelectorControls(){
            if (Input.GetKeyDown("right")) {
                jointSelector.previousJoint();
            }
            if (Input.GetKeyDown("left")) {
                jointSelector.nextJoint();
            }
            if (Input.GetKeyDown("/")) {
                jointSelector.selectJoint();
            } 
            if (Input.GetKeyDown(".")) {
                jointSelector.createJoint();
            } 
            if (Input.GetKeyDown("f")) {
                body.saveBody().writer();
            }  
        }
        public void collisionSphereSelectorControls(){
            if (Input.GetKeyDown("up")) {
                jointSelector.collisionSphereSelector.nextCollisionSphere();
            }
            if (Input.GetKeyDown("down")) {
                jointSelector.collisionSphereSelector.previousCollisionSphere();
            }
            if (Input.GetKeyDown("space")) {
                jointSelector.collisionSphereSelector.createCollisionSphere();
            }  
            if (Input.GetKeyDown("backspace")) {
                jointSelector.collisionSphereSelector.deleteCollisionSphere();
            }  
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
        public Body(Axis globalAxis, int amountOfJoints){
            this.globalAxis = globalAxis;
            bodyStructure = new Joint[amountOfJoints];
            keyGenerator = new KeyGenerator(amountOfJoints);
            reviveBody();
            editor = new Editor(this);
        }
        public Body(int path){
            editor = new Editor(new SaveBody().createBody(path));
        }

        public SaveBody saveBody(){
            Vector3 globalOrigin = globalAxis.origin;
            globalAxis.getWorldRotationInRadians(out float worldAngleY,out float worldAngleX,out float localAngleY);
            List<SaveJoint> savedJoints = new List<SaveJoint>();
            for (int i = 0; i<keyGenerator.maxKeys;i++){
                Joint joint = bodyStructure[i];
                if (joint != null){
                    savedJoints.Add(joint.saveJoint());
                }
            }
            return new SaveBody(
                keyGenerator.maxKeys,
                globalOrigin,
                worldAngleY,worldAngleX,localAngleY,
                savedJoints
                );
        }

        public void reviveBody(){
            if (keyGenerator.availableKeys == keyGenerator.maxKeys){
                int key = keyGenerator.getKey();
                Connection connection = new Connection(key);
                Axis axis = new Axis(globalAxis.origin,globalAxis.axisDistance);
                Joint addJoint = new Joint(this, keyGenerator.increaseKeysBy, axis, connection); 
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
            bool getOnlyActive = false;
            bool getFuture = true;
            List<Joint> firstJoint = getPastEnds();
            if (firstJoint != null) {
                tracker(
                    firstJoint,
                    getFuture,getOnlyActive,false,
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
            List<Joint> joints, bool pastOrFuture, bool getOnlyActive,bool getPastAndFuture,
            out List<Joint> tree, out List<Joint> end, out int treeSize
            ){
            treeSize = joints.Count;
            end = new List<Joint>();
            for (int i=0; i< treeSize; i++){
                Joint joint = joints[i];
                if (!joint.connection.used){
                    List<Joint> tracker = getPastAndFuture ? 
                        joint.connection.getAll(getOnlyActive):
                        joint.connection.nextConnections(pastOrFuture,getOnlyActive);
                    int trackerSize = tracker.Count;
                    if (trackerSize > 0){
                        joints.AddRange(tracker);
                        treeSize += trackerSize;
                    } else {
                        end.Add(joint);
                    }
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
                futureOnly,getOnlyActive, false,
                out connectionTree, out connectionEnd,
                out treeSize
                );
        }
        public void getPastConnections(
            bool getOnlyActive,
            out List<Joint> connectionTree, 
            out List<Joint> connectionEnd,
            out int treeSize
            ){
            bool pastOnly = false;
            connectionTracker(
                pastOnly, getOnlyActive, false,
                out connectionTree, out connectionEnd,
                out treeSize
                );
        }
        public void getAllConnections(
            bool pastOrFuture,bool getOnlyActive,
            out List<Joint> connectionTree, 
            out List<Joint> connectionEnd,
            out int treeSize
            ){
            connectionTracker(
                pastOrFuture, getOnlyActive, true,
                out connectionTree, out connectionEnd,
                out treeSize
                );
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
            int size = future.Count;
            for (int i =0; i<size;i++){
                future[i].connection.past.Remove(current);
            }
        }
        public void disconnectFromPast(){
            int size = past.Count;
            for (int i =0; i<size;i++){
                past[i].connection.future.Remove(current);
            }
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
        public List<Joint> getPast(bool getOnlyActive){
            return nextConnections(false,getOnlyActive);
        }
        public List<Joint> getFuture(bool getOnlyActive){
            return nextConnections(true,getOnlyActive);
        }
        public List<Joint> getAll(bool getOnlyActive){
            List<Joint> pastAndFuture = new List<Joint>();
            pastAndFuture.AddRange(getPast(getOnlyActive));
            used = false;
            pastAndFuture.AddRange(getFuture(getOnlyActive));
            return pastAndFuture;
        }
        void connectionTracker(
            bool pastOrFuture, bool getOnlyActive, bool getPastAndFuture,
            out List<Joint> tree, out List<Joint> end,
            out int treeSize
            ){
            tree = new List<Joint>{current};  
            if (getPastAndFuture)
                tree.AddRange(getAll(getOnlyActive));
            else 
                tree.AddRange(nextConnections(pastOrFuture,getOnlyActive));
            current.body.tracker(
                tree, pastOrFuture, getOnlyActive, getPastAndFuture,
                out tree, out end, out treeSize
                );
        }
        public List<int> pastIndex(){
            List<int> list = new List<int>();
            int count = past.Count;
            for (int i = 0; i<count;i++){
                list.Add(past[i].connection.indexInBody);
            }
            return list;
        }
        public List<int> futureIndex(){
            List<int> list = new List<int>();
            int count = future.Count;
            for (int i = 0; i<count;i++){
                list.Add(future[i].connection.indexInBody);
            }
            return list;
        }
    }

    public struct SaveAxis{
        public float angleX;
        public float angleY;
        public float distanceFromOrigin;
        public float worldAngleY;
        public float worldAngleX;
        public float localAngleY;
        public SaveAxis(
            float angleX,float angleY,
            float distanceFromOrigin, 
            float worldAngleX,float worldAngleY, float localAngleY
            ){
                this.angleX = angleX;
                this.angleY = angleY;
                this.distanceFromOrigin = distanceFromOrigin;
                this.worldAngleY = worldAngleY;
                this.worldAngleX = worldAngleX;
                this.localAngleY = localAngleY;
            }
        public override string ToString() {
            return $"{angleX} {angleY} {distanceFromOrigin} {worldAngleY} {worldAngleX} {localAngleY}\n";
        }
    }
    public class SaveBody{
        public int bodyStructureSize;
        public Vector3 origin;
        public float worldAngleY,worldAngleX,localAngleY;
        public List<SaveJoint> savedJoints;

        public SaveBody(){}
        public SaveBody(
            int bodyStructureSize,
            Vector3 origin,
            float worldAngleY, float worldAngleX, float localAngleY,
            List<SaveJoint> savedJoints
            ){
            this.bodyStructureSize = bodyStructureSize;
            this.origin = origin;
            this.worldAngleY = worldAngleY;
            this.worldAngleX = worldAngleX;
            this.localAngleY = localAngleY;
            this.savedJoints = savedJoints;
        }
        public Body createBody(int index){
            readFromFile(index);
            return createBody();
        }
        public Axis createlocalAxis(SaveAxis saveAxis,Axis globalAxis){
            Axis axis = new Axis(origin,5);
            if ( !float.IsNaN(saveAxis.angleX) && !float.IsNaN(saveAxis.angleY)){
                globalAxis.scaleRotationAxis(saveAxis.distanceFromOrigin);
                globalAxis.setRotationAxisInRadians(saveAxis.angleY,saveAxis.angleX);
                axis.placeAxis(globalAxis.rotationAxis);
            } else axis.placeAxis(globalAxis.origin);
            axis.setWorldRotationInRadians(saveAxis.worldAngleY,worldAngleX,localAngleY);
            return axis;
        }
        public Body createBody(){
            Axis globalAxis = new Axis(origin,5);
            Body body = new Body(globalAxis,bodyStructureSize);
            body.globalAxis.setWorldRotationInRadians(worldAngleY,worldAngleX,localAngleY);
            
            int count = savedJoints.Count;
            for (int i = 0; i< count;i++){
                SaveJoint saveJoint = savedJoints[i];
                Axis localAxis = createlocalAxis(saveJoint.localAxis,globalAxis);
                Connection connection = new Connection(saveJoint.indexInBody);
                Joint joint = new Joint(body, saveJoint.pointCloudSize,localAxis,connection);
                
                int listSize = saveJoint.pointCloud.Count;
                for (int j = 0; j<listSize;j++){
                    SaveCollisionSphere saveCollisionSphere = saveJoint.pointCloud[j];
                    Path path = new Path(body,saveJoint.indexInBody,saveCollisionSphere.indexInArray);
                    localAxis.scaleRotationAxis(saveCollisionSphere.distanceFromOrigin);
                    localAxis.setRotationAxisInRadians(saveCollisionSphere.localAxisAngleY,saveCollisionSphere.localAxisAngleX);
                    joint.pointCloud.collisionSpheres[saveCollisionSphere.indexInArray] = new CollisionSphere(path,localAxis.rotationAxis,saveCollisionSphere.sphereRadius);
                }

                List<int> newKeygeneratorFreeKeys = new List<int>();
                int newKeyCount = 0;
                for (int j = 0; j<saveJoint.pointCloudSize;j++){
                    if (joint.pointCloud.collisionSpheres[j] == null){
                        newKeygeneratorFreeKeys.Add(j);
                        newKeyCount += 1;
                    }
                }
                KeyGenerator keyGenerator = joint.pointCloud.keyGenerator;
                keyGenerator.freeKeys = newKeygeneratorFreeKeys;
                keyGenerator.availableKeys = newKeyCount;
                body.bodyStructure[saveJoint.indexInBody] = joint;
            }
            
            for (int i = 0; i<count;i++){
                SaveJoint saveJoint = savedJoints[i];
                List<Joint> past = getJoints(body,saveJoint.pastIndexes);
                List<Joint> future = getJoints(body,saveJoint.futureIndexes);

                Joint joint = body.bodyStructure[saveJoint.indexInBody];
                joint.connection.setPast(past);
                joint.connection.setFuture(future);
            }
            return body;
        }
        public List<Joint> getJoints(Body body,List<int> time){
            int count = time.Count;
            List<Joint> list = new List<Joint>();
            Joint[] bodyStructure = body.bodyStructure;
            for (int i = 0;i<count;i++){
                Joint joint = bodyStructure[time[i]];
                if (joint != null) list.Add(joint);
            }
            return list;
        }
        public void readFromFile(int index){
            string[] allData = File.ReadAllText($"Assets/v4/{index}.txt").Split("JointData");
            string[] bodyData = allData[0].Split("\n");
            bodyStructureSize = int.Parse(bodyData[0]);

            string[] vectorData = bodyData[1].Split(" ");
            float vecX = float.Parse(vectorData[0]);
            float vecY = float.Parse(vectorData[1]);
            float vecZ = float.Parse(vectorData[2]);
            origin = new Vector3(vecX,vecY,vecZ);
            worldAngleY = float.Parse(vectorData[3]);
            worldAngleX = float.Parse(vectorData[4]);
            localAngleY = float.Parse(vectorData[5]);

            savedJoints = new List<SaveJoint>();
            string[] jointData = allData[1].Split("\n");
            int sizeOfJoints = (jointData.Length-2)/6;
            for (int i = 0; i<sizeOfJoints; i++){
                int indexInBody = int.Parse(jointData[1+6*i]);
                string[] axisData = jointData[2+6*i].Split(" ");
                float angleX = float.Parse(axisData[0]);
                float angleY = float.Parse(axisData[1]);
                float distance = float.Parse(axisData[2]);
                float worldAngleX = float.Parse(axisData[3]);
                float worldAngleY = float.Parse(axisData[4]);
                float localAngleY = float.Parse(axisData[5]);
                SaveAxis axis = new SaveAxis(angleX,angleY,distance,worldAngleX,worldAngleY,localAngleY);
                int pointCloudSize = int.Parse(jointData[3+6*i]);
                string[] pointCloudData = jointData[4+6*i].Split(",");
                List<SaveCollisionSphere> collisionSpheres = new List<SaveCollisionSphere>();
                int amountOfSpheres = pointCloudData.Length;

                for (int j = 0; j<amountOfSpheres; j++){
                    string sphereData = pointCloudData[j];
                    if (sphereData != ""){
                        string[] data = sphereData.Split(" ");
                        int indexInArray = int.Parse(data[0]);
                        float localAxisAngleX = float.Parse(data[1]);
                        float localAxisAngleY = float.Parse(data[2]);
                        float distanceFromOrigin = float.Parse(data[3]);
                        float sphereRadius = float.Parse(data[4]);
                        SaveCollisionSphere saveCollisionSphere = 
                            new SaveCollisionSphere(indexInArray,localAxisAngleX,localAxisAngleY,distanceFromOrigin,sphereRadius);
                        collisionSpheres.Add(saveCollisionSphere);
                    }
                }
                List<int> pastData = createTime(jointData[5+6*i]);
                List<int> futureData = createTime(jointData[6+6*i]);
                
                savedJoints.Add(new SaveJoint(
                    indexInBody, axis, pointCloudSize, collisionSpheres,pastData,futureData
                    ));     
            }
        }
        List<int> createTime(string time){
            string[] timeData = time.Split(" ");
            int timeDataSize = timeData.Length;
            List<int> connections = new List<int>();
            for (int i = 0; i<timeDataSize;i++){
                string str = timeData[i];
                if (str != ""){
                    connections.Add(int.Parse(str));
                }
            }
            return connections;
        }
        
        public void writer(){
            File.WriteAllText("Assets/v4/1.txt",ToString());
        }

        public override string ToString() {
            string size = $"{bodyStructureSize}\n";
            string axis = $"{origin.x} {origin.y} {origin.z} {worldAngleY} {worldAngleX} {localAngleY}\n";
            int savedJointsSize = savedJoints.Count;
            string joints = "";
            for (int i = 0; i< savedJointsSize; i++){
                joints += savedJoints[i].ToString();
            }
            return $"{size}{axis}JointData\n{joints}JointData";
        }
    }
    public struct SaveJoint{
        public int indexInBody;
        public SaveAxis localAxis;
        public int pointCloudSize;
        public List<SaveCollisionSphere> pointCloud;
        public List<int> pastIndexes;
        public List<int> futureIndexes;
        
        public SaveJoint(
            int indexInBody,
            SaveAxis localAxis,
            int pointCloudSize,
            List<SaveCollisionSphere> pointCloud,
            List<int> pastIndexes,
            List<int> futureIndexes
            ){
                this.indexInBody = indexInBody;
                this.localAxis = localAxis;
                this.pointCloudSize = pointCloudSize;
                this.pointCloud = pointCloud;
                this.pastIndexes = pastIndexes;
                this.futureIndexes = futureIndexes;
        }

        string pointCloudToString(){
            string str = "";
            int listSize = pointCloud.Count;
            for (int i = 0; i<listSize; i++){
                str +=  pointCloud[i].ToString();
            }
            return str+"\n";
        }
        string timeToString(List<int> time){
            string str = "";
            int count = time.Count;
            for (int i = 0;i<count;i++){
                str += $"{time[i]} ";
            }
            return str+"\n";
        }

        public override string ToString() {
            string pointCloud = pointCloudToString();
            string past = timeToString(pastIndexes);
            string future = timeToString(futureIndexes);
            string axis = localAxis.ToString();
            return $"{indexInBody}\n{axis}{pointCloudSize}\n{pointCloud}{past}{future}";
        }
    }
    public struct SaveCollisionSphere{
        public int indexInArray;
        public float localAxisAngleX;
        public float localAxisAngleY;
        public float distanceFromOrigin;
        public float sphereRadius;
        public SaveCollisionSphere(
            int indexInArray,
            float localAxisAngleX, float localAxisAngleY,
            float distanceFromOrigin, float sphereRadius
            ){
                this.indexInArray = indexInArray;
                this.localAxisAngleX = localAxisAngleX;
                this.localAxisAngleY = localAxisAngleY;
                this.distanceFromOrigin = distanceFromOrigin;
                this.sphereRadius = sphereRadius;
        }
        public override string ToString() {
            return $"{indexInArray} {localAxisAngleX} {localAxisAngleY} {distanceFromOrigin} {sphereRadius},";
        }
    }

    public class Joint {
        public Body body;
        public Axis localAxis;
        public Connection connection;
        public PointCloud pointCloud;

        public Joint(){}
        public Joint(Body body, int amountOfSpheres, Axis localAxis,Connection connection){
            this.body = body;
            pointCloud = new PointCloud(amountOfSpheres);
            this.localAxis = localAxis;
            this.connection = connection;
            connection.current = this;
            pointCloud.joint = this;
        }

        public void setBody(Body body){
            this.body=body;
        }
        public SaveJoint saveJoint(){
            Vector3 jointOrigin = localAxis.origin;
            Vector3 globalOrigin = body.globalAxis.origin;
            body.globalAxis.getAngle(
                jointOrigin,
                globalOrigin,body.globalAxis.x,body.globalAxis.y,body.globalAxis.z,
                out float globalY,out float globalX
                );
            localAxis.getWorldRotationInRadians(out float worldAngleY,out float worldAngleX,out float localAngleY);
            float distanceFromOrigin = body.globalAxis.length(jointOrigin-globalOrigin);

            return new SaveJoint(
                connection.indexInBody,
                new SaveAxis(
                    globalY,globalX,
                    distanceFromOrigin,
                    worldAngleY,worldAngleX,localAngleY
                    ),
                pointCloud.keyGenerator.maxKeys,
                pointCloud.savePointCloud(),
                connection.pastIndex(),
                connection.futureIndex()
                );       
        }
        public Joint createJoint(){
            body.resizeArray(1);
            int key = body.keyGenerator.getKey();
            Connection connection = new Connection(key);
            connection.past.Add(this);
            Axis axis = new Axis(localAxis.origin,localAxis.axisDistance);
            Joint addJoint = new Joint(body,pointCloud.keyGenerator.increaseKeysBy, axis,connection);
            this.connection.future.Add(addJoint); 
            body.bodyStructure[key] = addJoint;
            return addJoint;
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

        public List<SaveCollisionSphere> savePointCloud(){
            List<SaveCollisionSphere> saveCollisionSpheres = new List<SaveCollisionSphere>();
            for (int i = 0; i<collisionSpheres.Length; i++){
                CollisionSphere collisionSphere = collisionSpheres[i];
                if (collisionSphere != null){
                    saveCollisionSpheres.Add(collisionSphere.saveCollisionSphere());
                }
            }
            return saveCollisionSpheres;
        }
        public CollisionSphere createSphere(){
            resizeArray(1);
            int sphereIndex = keyGenerator.getKey();
            Path path = new Path(joint.body,joint.connection.indexInBody,sphereIndex);
            CollisionSphere collisionSphere = new CollisionSphere(path,joint.localAxis.rotationAxis,1);
            collisionSpheres[sphereIndex] = collisionSphere;
            return collisionSphere;
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
        public void rotateSpheres(Vector4 quat,Vector3 rotationOrigin){
            int sphereCount = collisionSpheres.Length;
            for (int i = 0; i<sphereCount; i++){
                CollisionSphere collisionSphere = collisionSpheres[i];
                collisionSphere?.setOrigin(
                        joint.localAxis.quatRotate(
                            collisionSphere.sphere.origin,
                            rotationOrigin,
                            quat
                        )
                    );
            }
        }
        public void updateAllSphereColors(Color color){
            int sphereCount = collisionSpheres.Length;
            for (int i = 0; i<sphereCount; i++){
                collisionSpheres[i]?.sphere.updateColor(color);
            }
        }
        public void resetAllSphereColors(){
            int sphereCount = collisionSpheres.Length;
                for (int i = 0; i<sphereCount; i++){
                    collisionSpheres[i]?.sphere.resetColor();
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
        public SaveCollisionSphere saveCollisionSphere(){
            int key = path.jointKey;
            Joint joint = path.body.bodyStructure[key];
            Axis axis = joint.localAxis;
            float distanceFromOrigin = axis.length(sphere.origin-axis.origin);
            axis.getAngle(
                sphere.origin,
                axis.origin,axis.x,axis.y,axis.z,
                out float localY,out float localX
                );
            return new SaveCollisionSphere(
                    path.collisionSphereKey,
                    localX,localY,
                    distanceFromOrigin,sphere.radius
                );
        }
        public void setOrigin(Vector3 newOrigin){
            sphere.setOrigin(newOrigin);
        }
        public void moveOrigin(Vector3 newOrigin){
            sphere.moveOrigin(newOrigin);
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

        public Sphere(){}
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
