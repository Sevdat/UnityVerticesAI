using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

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
        public float distance;
        public float angleY, sensitivitySpeedY, sensitivityAccelerationY,
                     angleX, sensitivitySpeedX, sensitivityAccelerationX,
                     speed, acceleration;
        
        public AroundAxis(){}
        public AroundAxis(Axis axis, Sphere sphere){
            this.sphere = sphere;
            this.axis = axis;
            angleY = 0;angleX = 0;
            sensitivitySpeedY = Mathf.PI/400f; sensitivityAccelerationY = 1;
            sensitivitySpeedX = Mathf.PI/400f; sensitivityAccelerationX = 1;
            speed = 0; acceleration = 1;
            distance = axis.axisDistance;
            sphere.origin = axis.origin + new Vector3(0,distance,0);
        }
        public void scale(float newDistance){
            if (newDistance>0){
                    distance = newDistance;
                    sphere.origin = axis.origin + axis.distanceFromOrigin(sphere.origin,axis.origin,newDistance);
                } else {
                    if (newDistance != 0) distance = Mathf.Abs(newDistance);
                    sphere.origin = axis.origin + axis.distanceFromOrigin(axis.origin,sphere.origin,distance);
                    get();
                }
            if (axis.renderAxis.created){
                updateAxis();
            }
        }
        public void get(){
            float tempAngleX = angleX;
            axis.getPointAroundOrigin(sphere.origin,out angleY, out angleX);
            if (!(angleY == 0f || angleY == Mathf.PI)) angleX = tempAngleX;
            axis.invertAxis(tempAngleX,ref angleY,ref angleX);
        }

        public void set(float angleY,float angleX){
            sphere.origin = axis.setPointAroundOrigin(angleY,angleX,distance);
            this.angleY = angleY;
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
        public void right(){
            sensitivitySpeedX *= sensitivityAccelerationX;
            angleX += sensitivitySpeedX;
            if (angleX>2*Mathf.PI) angleX -= 2*MathF.PI;
            set(angleY,angleX);
        }
        public void left(){
            sensitivitySpeedX *= sensitivityAccelerationX;
            angleX -= sensitivitySpeedX;
            if (angleX<0) angleX += 2*MathF.PI;
            set(angleY,angleX);      
        }
        public void up(){
            sensitivitySpeedY *= sensitivityAccelerationY;
            angleY += sensitivitySpeedY;
            if (angleY>2*Mathf.PI) angleY -= 2*MathF.PI;
            set(angleY,angleX);
        }
        public void down(){
            sensitivitySpeedY *= sensitivityAccelerationY;
            angleY -= sensitivitySpeedY;
            if (angleY<0) angleY += 2*MathF.PI;
            set(angleY,angleX);
        }
        
        public void scaleUp(){
            scale(distance + 1);
        }
        public void scaleDown(){
            scale(distance - 1);
        }
        public void updateSpeed(){
            speed *= acceleration;
        }
        public Vector3 placeAxis(){
            distance += speed;
            scale(distance);
            updateSpeed();
            return axis.placeAxis(sphere.origin);
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
            Vector3 point = origin + distanceFromOrigin(y,origin,distance);
            Vector4 rotY = angledAxis(angleY,x);
            Vector4 rotX = angledAxis(angleX,y);
            point = quatRotate(point,origin,rotY);
            point = quatRotate(point,origin,rotX);
            return point;
        }
        
        public void getPointAroundOrigin(Vector3 point, out float angleY,out float angleX){
            getAngle(point,origin,x,y,z,out angleY,out angleX);
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
        internal bool invertAxis(float angleXBeforeGet, ref float yAngle,ref float xAngle){
            bool inverted = MathF.Round(Mathf.Abs(xAngle - angleXBeforeGet) - Mathf.PI) == 0;
            if (inverted) {
                yAngle = 2*Mathf.PI -yAngle;
                xAngle = (angleXBeforeGet>xAngle)? xAngle+Mathf.PI: xAngle-Mathf.PI;
            }
            return inverted;
        }

        public void getWorldRotation(){
            Vector3 worldX = origin + new Vector3(axisDistance,0,0);
            Vector3 worldY = origin + new Vector3(0,axisDistance,0);
            Vector3 worldZ = origin + new Vector3(0,0,axisDistance);

            float tempX = worldAngleX;
            float tempLocalY = localAngleY;
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
           
            bool check = invertAxis(tempX,ref worldAngleY,ref worldAngleX);
            if (check) localAngleY = (tempLocalY>localAngleY)? localAngleY+Mathf.PI: localAngleY-Mathf.PI;
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
            spin.set(spin.angleY,spin.angleX);
            move.set(move.angleY,move.angleX);
            if (renderAxis.created){
                renderAxis.updateAxis();
                spin.updateAxis();
                move.updateAxis();
            }
            this.worldAngleY = worldAngleY;
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
        
        public void up(){
            worldAngleY += worldSpeedY;
            if (worldAngleY>2*Mathf.PI) worldAngleY -= 2*MathF.PI;
            setWorldRotation(worldAngleY,worldAngleX,localAngleY);
        }
        public void down(){
            worldAngleY -= worldSpeedY;
            if (worldAngleY<0) worldAngleY += 2*MathF.PI;
            setWorldRotation(worldAngleY,worldAngleX,localAngleY);  
        }
        public void right(){
            worldAngleX += worldSpeedX;
            if (worldAngleX>2*Mathf.PI) worldAngleX -= 2*MathF.PI;
            setWorldRotation(worldAngleY,worldAngleX,localAngleY);
        }
        public void left(){
            worldAngleX -= worldSpeedX;
            if (worldAngleX<0) worldAngleX += 2*MathF.PI;
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
                selected.axisDirection.sphere.updateColor(jointSelector.blue);
            }
        }
        public void createCollisionSphere(){
            collisionSpheres.Add(jointSelector.selected.pointCloud.createSphere());
            index = size;
            size += 1;
            if (selected != null) recolor(); else {
                selected = collisionSpheres[index];
                selected.axisDirection.sphere.updateColor(jointSelector.blue);
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
                    selected.axisDirection.sphere.updateColor(jointSelector.blue);
                } else selected = null;
            }
        }

        void recolor(){
            selected.axisDirection.sphere.updateColor(jointSelector.yellow);
            selected = collisionSpheres[index];
            selected.axisDirection.sphere.updateColor(jointSelector.blue);
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
                jointSelector.selected.localAxis.up();
            }
            if (Input.GetKey("k")) {
                jointSelector.selected.localAxis.down();
            }
            if (Input.GetKey("l")) {       
                jointSelector.selected.localAxis.right();     
            }
            if (Input.GetKey("j")) {    
                jointSelector.selected.localAxis.left();        
            }
            if (Input.GetKey("o")) { 
                jointSelector.selected.localAxis.clockwise();           
            }
            if (Input.GetKey("u")) {
                jointSelector.selected.localAxis.antiClockwise(); 
            }
        }
        public void rotationAxisControls(){
            if (Input.GetKey("w")) {
                jointSelector.selected.localAxis.spin.up();
            }
            if (Input.GetKey("s")) {
                jointSelector.selected.localAxis.spin.down();
            }
            if (Input.GetKey("d")) {  
                jointSelector.selected.localAxis.spin.right();          
            }
            if (Input.GetKey("a")) { 
                jointSelector.selected.localAxis.spin.left();           
            }
            if (Input.GetKeyDown("e")) { 
                jointSelector.selected.localAxis.spin.scaleUp();           
            }
            if (Input.GetKeyDown("q")) {
                jointSelector.selected.localAxis.spin.scaleDown();
            }
            // if (Input.GetKeyDown("return")) {
            //     Vector3 newPoisition = jointSelector.selected.localAxis.move.placeAxis();
            //     jointSelector.collisionSphereSelector.moveCollisionSpheres(newPoisition);
            // }
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
                new SaveBody(body).writer();
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
        public Body(int worldKey,Axis globalAxis, int amountOfJoints){
            this.worldKey = worldKey;
            this.globalAxis = globalAxis;
            bodyStructure = new Joint[amountOfJoints];
            keyGenerator = new KeyGenerator(amountOfJoints);
            reviveBody();
            editor = new Editor(this);
        }
        public Body(int path){
            
        }

        public string saveBody(){
            Vector3 globalOrigin = globalAxis.origin;
            globalAxis.getWorldRotationInRadians(out float worldAngleY,out float worldAngleX,out float localAngleY);
            List<int> jointIndexes = new List<int>();
            string strJointIndexes = "";
            for (int i = 0; i<keyGenerator.maxKeys;i++){
                Joint joint = bodyStructure[i];
                if (joint != null){
                    strJointIndexes += $" {i}";
                    jointIndexes.Add(i);
                }
            }
            string stringPath = $"Body_{worldKey}_";
            string bodyStructureSize = $"{stringPath}BodyStructureSize: {keyGenerator.maxKeys}\n";
            string allJointsInBody = $"{stringPath}AllJointsInBody:{strJointIndexes}\n";
            string globalOriginLocation = $"{stringPath}globalOriginLocation: {globalOrigin.x} {globalOrigin.y} {globalOrigin.z}\n";
            string globalAxisRotationXYZ = $"{stringPath}globalAxisRotationXYZ: {worldAngleY} {worldAngleX} {localAngleY}\n";

            return bodyStructureSize + allJointsInBody + globalOriginLocation + globalAxisRotationXYZ;

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
        public void arraySizeManager(int amount){
            if (amount>keyGenerator.maxKeys){
                resizeArray(amount,false);
            } else if (amount < keyGenerator.maxKeys){
                optimizeBody();
                resizeArray(amount,false);
            }
        }
        public void resizeArray(int amount,bool extra){
            int availableKeys = keyGenerator.availableKeys;
            int limitCheck = availableKeys - amount;
            if(limitCheck < 0) {
                int oldLimit = keyGenerator.increaseKeysBy;
                int oldMax = keyGenerator.maxKeys;
                int increaseby = extra? Mathf.Abs(limitCheck) + oldLimit:Mathf.Abs(limitCheck);
                keyGenerator.setIncreaseKeysBy(increaseby);
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
            int max = keyGenerator.maxKeys;
            int newSize = max - keyGenerator.availableKeys;
            Joint[] orginizedJoints = new Joint[newSize];
            int newIndex = 0;
            for (int i = 0; i < max; i++){
                Joint joint = bodyStructure[i];
                if (joint != null){
                    joint.connection.indexInBody = newIndex;
                    joint.pointCloud.optimizeCollisionSpheres();
                    orginizedJoints[newIndex] = joint; 
                    newIndex++;
                }
            }
            bodyStructure = orginizedJoints;
            keyGenerator.resetGenerator(newSize);
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
        public void disconnectFromFuture(){
            int size = future.Count;
            for (int i =0; i<size;i++){
                future[i].connection.past.Remove(current);
            }
            future = new List<Joint>();
        }
        public void disconnectFromPast(){
            int size = past.Count;
            for (int i =0; i<size;i++){
                past[i].connection.future.Remove(current);
            }
            past = new List<Joint>();
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
            if (getPastAndFuture){
                if (pastOrFuture) tree.AddRange(getFuture(getOnlyActive)); else tree.AddRange(getPast(getOnlyActive));
                tree.AddRange(getAll(getOnlyActive));
                }
            else 
                tree.AddRange(nextConnections(pastOrFuture,getOnlyActive));
            current.body.tracker(
                tree, pastOrFuture, getOnlyActive, getPastAndFuture,
                out tree, out end, out treeSize
                );
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
        public SaveBody(){}
        public SaveBody(Body body){
            this.body = body;
        }
        
        public void writer(){
            using(StreamWriter writetext = new StreamWriter($"Assets/v4/{body.worldKey}.txt")) {
                writetext.WriteLine(body.saveBody());
                int size = body.keyGenerator.maxKeys;
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
                string readText = readtext.ReadLine();
                string[] splitStr = readText.Split(": ");
                if (splitStr.Length == 2){
                    removeEmpty(splitStr[0].Split("_"), out List<string> instruction, out int instructionSize);
                    removeEmpty(splitStr[1].Split(" "), out List<string> values, out int valueSize);
                    

                }
            }
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
                if (check) body.arraySizeManager(amount);
            }
        }
        void allJointsInBodyInstruction(List<string> value){
            HashSet<int> set = new HashSet<int>();
            int size = value.Count;
            int maxKey = 0;
            bool check = false;
            for (int i = 0; i < size; i++){
                check = int.TryParse(value[i], out int key);
                if (check && !set.Contains(i)) {
                    set.Add(key);
                    if (key>maxKey) maxKey = key;
                } else if(!check) break;
            }
            if (maxKey>body.keyGenerator.maxKeys) body.resizeArray(maxKey,false);
            if (check) {
                Joint[] bodystructure = body.bodyStructure;
                for (int i = 0; i< body.keyGenerator.maxKeys; i++){
                    Joint joint = bodystructure[i];
                    if (joint !=null){
                        if (!set.Contains(joint.connection.indexInBody)) joint.deleteJoint();
                    }
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
        const string XYRotationAxisFromLocalAxis = "XYRotationAxisFromLocalAxis";
        const string XYRotationAxisSpeed = "XYRotationAxisSpeed";
        const string pastConnectionsInBody = "PastConnectionsInBody";
        const string futureConnectionsInBody = "FutureConnectionsInBody";

        public void jointInstructions(string jointKey, string instruction, List<string> value){
            bool checkKey = int.TryParse(jointKey, out int key);
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

                    break;
                    case spinX:
                    break;
                    case spinY:
                    break;
                    case spinSpeedAndAcceleration:
                    break;
                    case moveX:
                    break;
                    case moveY:
                    break;
                    case moveSpeedAndAcceleration:
                    break;                
                    case XYRotationAxisFromLocalAxis:
                    break;
                    case XYRotationAxisSpeed:
                    break;
                    case pastConnectionsInBody:
                    break;
                    case futureConnectionsInBody:
                    break;
                }
            }
        }

        void activeInstruction(Joint joint,List<string> value){
            joint.connection.active = value[0] == "True";
        }
        void distanceFromGlobalOriginInstruction(Joint joint,List<string> value){
            bool checkStr = float.TryParse(value[0], out float key);
            if (checkStr){
                joint.distanceFromGlobalOrigin(key);
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
                        joint.moveJoint(joint.body.globalAxis.setPointAroundOrigin(y,x,length) - joint.localAxis.origin);
                    } else {
                        localAxis.getAngle(localAxis.origin,globalAxis.origin,globalAxis.x,globalAxis.y,globalAxis.z,out float globalAngleY,out float globalAngleX);
                        if (checkY) globalAngleY = y;
                        if (checkX) globalAngleX = x;
                        joint.moveJoint(localAxis.setPointAroundOrigin(globalAngleY,globalAngleX,length) - localAxis.origin);
                }
            }
        }
        void localAxisRotationInstruction(Joint joint,List<string> value){
            if (value.Count>=3){
                bool checkY = float.TryParse(value[0], out float y);
                bool checkX = float.TryParse(value[1], out float x);
                bool checkLY = float.TryParse(value[1], out float ly);
                if (checkY && checkX && checkLY) {
                    ;
                }
            }
        }

        const string pointCloudSize = "PointCloudSize";
        const string allSpheresInJoint = "AllSpheresInJoint";
        const string XYFromLocalAxis = "XYFromLocalAxis";
        const string radius = "Radius";
        const string colorRGBA = "ColorRGBA";

        public void sphereInstructions(string instruction, string[] value){
            switch (instruction){
                case pointCloudSize:
                break;
                case allSpheresInJoint:
                break;
                case XYFromLocalAxis:
                break;
                case radius:
                break;
                case colorRGBA:
                break;
            }

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
        public string saveJoint(){
            Vector3 jointOrigin = localAxis.origin;
            Vector3 globalOrigin = body.globalAxis.origin;
            body.globalAxis.getAngle(
                jointOrigin,
                globalOrigin,body.globalAxis.x,body.globalAxis.y,body.globalAxis.z,
                out float globalY,out float globalX
                );
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
        public Joint createJoint(){
            body.resizeArray(1, true);
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
            body.keyGenerator.returnKey(connection.indexInBody);
            connection.disconnectFromFuture();
            connection.disconnectFromPast();
            pointCloud.deleteAllSpheres();
            body.bodyStructure[connection.indexInBody] = null;
        }
        public void distanceFromGlobalOrigin(float newDistance){
            Vector3 globalOrigin = body.globalAxis.origin;
            Vector3 localOrigin = localAxis.origin;
            float length = localAxis.length(localOrigin-globalOrigin);
            Vector3 direction = localAxis.direction(localOrigin,globalOrigin)*(newDistance-length);
            moveJoint(direction);
        }
        public void moveJoint(Vector3 add){
            localAxis.moveAxis(add);
            pointCloud.moveSpheres(add);
        }
        public void worldRotateJoint(float worldAngleY,float worldAngleX,float localAngleY){
            localAxis.setWorldRotation(worldAngleY,worldAngleX,localAngleY);
            pointCloud.rotateSpheres();
        }

        public void rotatePastHierarchy(){
            Vector4 quat = localAxis.spin.quat(localAxis.spin.speed);
            rotateHierarchy(quat, false,false,false);
            localAxis.spin.updateSpeed();
        }
        public void rotateFutureHierarchy(){
            Vector4 quat = localAxis.spin.quat(localAxis.spin.speed);
            rotateHierarchy(quat, true,false,false);
            localAxis.spin.updateSpeed();
        }
        public void rotateAllHierarchy(){
            Vector4 quat = localAxis.spin.quat(localAxis.spin.speed);
            rotateHierarchy(quat, false,false,true);
            localAxis.spin.updateSpeed();
        }

        public void movePastHierarchy(){
            Vector3 move = localAxis.move.placeAxis();
            moveHierarchy(move, false,false,false);
            localAxis.move.updateSpeed();
        }
        public void moveFutureHierarchy(){
            Vector3 move = localAxis.move.placeAxis();
            moveHierarchy(move, true,false,false);
            localAxis.move.updateSpeed();
        }
        public void moveAllHierarchy(){
            Vector3 move = localAxis.move.placeAxis();
            moveHierarchy(move, false,false,true);
            localAxis.move.updateSpeed();
        }

        void rotateHierarchy(Vector4 quat, bool pastOrFuture, bool rotateOnlyActive, bool rotateAll){
            initTree(pastOrFuture, rotateOnlyActive, rotateAll, out List<Joint> tree, out int size);  
            Vector3 rotationOrigin = localAxis.origin;
            for (int i = 0; i<size;i++){
                Joint joint = tree[i];
                List<Joint> joints = tree[i].connection.getAll(rotateOnlyActive);
                tree.AddRange(joints);
                size += joints.Count;
                joint.localAxis.rotate(quat,rotationOrigin);
                joint.pointCloud.rotateSpheres();
            }
            body.resetUsed(tree,size);
        }

        void moveHierarchy(Vector3 newVec, bool pastOrFuture, bool rotateOnlyActive,bool rotateAll){
            initTree(pastOrFuture, rotateOnlyActive, rotateAll, out List<Joint> tree, out int size);  
            for (int i = 0; i<size;i++){
                Joint joint = tree[i];
                List<Joint> joints = tree[i].connection.getAll(rotateOnlyActive);
                tree.AddRange(joints);
                size += joints.Count;
                moveJoint(newVec);
            }
            body.resetUsed(tree,size);
        }
        void initTree(bool pastOrFuture, bool rotateOnlyActive,bool rotateAll, out List<Joint> tree,out int size){
            tree = new List<Joint>{this};
            if (rotateAll) 
                tree.AddRange(connection.getAll(rotateOnlyActive)); 
                else if (pastOrFuture) 
                        tree.AddRange(connection.getFuture(rotateOnlyActive)); 
                    else 
                        tree.AddRange(connection.getPast(rotateOnlyActive)); 
            size = tree.Count;
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

        public string savePointCloud(out List<int> indexes, out int listSize){
            listSize = 0;
            int size = collisionSpheres.Length;
            indexes = new List<int>();
            string stringPath = $"Body_{joint.body.worldKey}_Joint_{joint.connection.indexInBody}_";
            string pointCloudSize = $"{stringPath}PointCloudSize: {size}\n";
            string allSpheresInJoint = $"{stringPath}AllSpheresInJoint: ";
            for (int i = 0; i<keyGenerator.maxKeys; i++){
                CollisionSphere collisionSphere = collisionSpheres[i];
                if (collisionSphere != null) {
                    allSpheresInJoint += $"{i} ";
                    indexes.Add(i);
                    listSize++;
                }
            }
            return pointCloudSize+allSpheresInJoint+"\n";
        }
        public CollisionSphere createSphere(){
            resizeArray(1, true);
            int sphereIndex = keyGenerator.getKey();
            Path path = new Path(joint.body,joint,sphereIndex);
            CollisionSphere collisionSphere = new CollisionSphere(path,joint.localAxis.spin.sphere.origin,1);
            collisionSpheres[sphereIndex] = collisionSphere;
            return collisionSphere;
        }
        public void deleteSphere(int key){
            CollisionSphere remove = collisionSpheres[key];
            if(remove != null){
                keyGenerator.returnKey(key);
                collisionSpheres[key].axisDirection.sphere.destroySphere();
                collisionSpheres[key] = null;
            }
        }
        public void deleteAllSpheres(){
            int size = collisionSpheres.Length;
            for (int i = 0; i<size;i++){
                deleteSphere(i);
            }
        }
        public void rotateSpheres(){
            int sphereCount = collisionSpheres.Length;
            for (int i = 0; i<sphereCount; i++){
                CollisionSphere collisionSphere = collisionSpheres[i];
                collisionSphere?.axisDirection.resetOrigin();
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
                collisionSpheres[i]?.axisDirection.sphere.updateColor(color);
            }
        }
        public void resetAllSphereColors(){
            int sphereCount = collisionSpheres.Length;
                for (int i = 0; i<sphereCount; i++){
                    collisionSpheres[i]?.axisDirection.sphere.resetColor();
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
        public void resizeArray(int amount, bool extra){
            int availableKeys = keyGenerator.availableKeys;
            int limitCheck = availableKeys - amount;
            if(limitCheck < 0) {
                int oldLimit = keyGenerator.increaseKeysBy;
                int oldMax = keyGenerator.maxKeys;
                int increaseby = extra? Mathf.Abs(limitCheck) + oldLimit:Mathf.Abs(limitCheck);
                keyGenerator.setIncreaseKeysBy(increaseby);
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
                    collision.path.setJoint(joint);
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
        public AroundAxis axisDirection;

        public CollisionSphere(){}
        public CollisionSphere(Path path,Vector3 origin,float radius){
            this.path = path;
            axisDirection.sphere = new Sphere(origin,radius);
        }
        public string saveCollisionSphere(){
            Axis axis = path.joint.localAxis;
            Sphere sphere = axisDirection.sphere;
            float distanceFromOrigin = axis.length(axisDirection.sphere.origin-axis.origin);
            axis.getAngle(
                axisDirection.sphere.origin,
                axis.origin,axis.x,axis.y,axis.z,
                out float localY,out float localX
                );
            string stringPath = $"Body_{path.body.worldKey}_Joint_{path.joint.connection.indexInBody}_Sphere_{path.collisionSphereKey}_";
            string distanceFromLocalOrigin = $"{stringPath}DistanceFromLocalOrigin: {distanceFromOrigin}\n";
            string XYFromLocalAxis = $"{stringPath}XYFromLocalAxis: {localY} {localX}\n";
            string radius = $"{stringPath}Radius: {sphere.radius}\n";
            string color = $"{stringPath}ColorRGBA: {sphere.color.r} {sphere.color.g} {sphere.color.b} {sphere.color.a}\n";
            return distanceFromLocalOrigin + XYFromLocalAxis + radius + color;
        }
        public void setOrigin(Vector3 newOrigin){
            axisDirection.sphere.setOrigin(newOrigin);
        }
        public void moveOrigin(Vector3 newOrigin){
            axisDirection.sphere.moveOrigin(newOrigin);
        }
        public void setRadius(float newRadius){
            axisDirection.sphere.setRadius(newRadius);
        }
    }
    public class Sphere{
        public float radius;
        public Vector3 origin;
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
