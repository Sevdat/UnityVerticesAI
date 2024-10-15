using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityPluginTest : MonoBehaviour
{
    AxisSimulationTest testAxis;
    public class AxisSimulationTest:SourceCode{
        internal Axis axis = new Axis();
        float accuracy = 0.1f;
        float worldAngleY = 0,worldAngleX = 0,localAngleY = 0;
        Vector3 vec = new Vector3(0,0,0); 
        float distance = 5f;

        public AxisSimulationTest(){}
        public AxisSimulationTest(Vector3 origin, float distance,float worldAngleY,float worldAngleX,float localAngleY){
            vec = origin;
            this.distance = distance;
        }
        public void setDistance(float newDistance){
            distance = newDistance;
        }
        public void setVec(Vector3 newVec){
            vec = newVec;
        }
        public void setAccuracy(float newAccuracy){
            accuracy = newAccuracy;
        }
        public void setAngle(float worldAngleY,float worldAngleX,float localAngleY){
            this.worldAngleY = worldAngleY;
            this.worldAngleX = worldAngleX;
            this.localAngleY = localAngleY;
        }
        internal void testCreateAxis(){
            Vector3 origin = axis.origin;
            if (origin != vec) print($"originPositionError: expected {vec} got {origin}");
            
            Vector3 vecX = vec + new Vector3(distance,0,0);
            Vector3 x = axis.x;
            if (x != vecX) print($"xPositionError: expected {vecX} got {x}");
            
            Vector3 vecY = vec + new Vector3(0,distance,0);
            Vector3 y = axis.y;
            if (y != vecY) print($"yPositionError: expected {vecY} got {y}");
                        
            Vector3 vecZ = vec + new Vector3(0,0,distance);
            Vector3 z = axis.z;
            if (z != vecZ) print($"zPositionError: expected {vecZ} got {z}");
        }
        internal void testMoveAxis(Vector3 add){
            testCreateAxis();
            axis.moveAxis(add);
            Vector3 origin = axis.origin;
            if (origin != vec+add) print($"originMoveError: expected {vec} got {origin}");
            
            Vector3 vecX = vec + new Vector3(distance,0,0);
            Vector3 x = axis.x;
            if (x != vecX+add) print($"xMoveError: expected {vecX} got {x}");
            
            Vector3 vecY = vec + new Vector3(0,distance,0);
            Vector3 y = axis.y;
            if (y != vecY+add) print($"yMoveError: expected {vecY} got {y}");
                        
            Vector3 vecZ = vec + new Vector3(0,0,distance);
            Vector3 z = axis.z;
            if (z != vecZ+add) print($"zMoveError: expected {vecZ} got {z}");
        }
        internal void testPlaceAxis(Vector3 newOrigin){
            testCreateAxis();
            axis.placeAxis(newOrigin);
            Vector3 origin = axis.origin;
            if (origin != newOrigin) print($"originPlaceError: expected {vec} got {origin}");
            
            Vector3 vecX = newOrigin + new Vector3(distance,0,0);
            Vector3 x = axis.x;
            if (x != vecX) print($"xPlaceError: expected {vecX} got {x}");
            
            Vector3 vecY = newOrigin + new Vector3(0,distance,0);
            Vector3 y = axis.y;
            if (y != vecY) print($"yPlaceError: expected {vecY} got {y}");
                        
            Vector3 vecZ = newOrigin + new Vector3(0,0,distance);
            Vector3 z = axis.z;
            if (z != vecZ) print($"zPlaceError: expected {vecZ} got {z}");
        }

        internal void testScaleAxis(){
            testCreateAxis();
            axis.scaleAxis(distance);
            float min = distance - accuracy;
            float max = distance + accuracy;
            float gotX = axis.length(
                axis.x - axis.origin
                );
            if (gotX < min || max < gotX) print($"xScaleError: expected {distance} got {gotX}");

            float gotY = axis.length(axis.y - axis.origin);
            if (gotX < min || max < gotX) print($"yScaleError: expected {distance} got {gotY}");

            float gotZ = axis.length(axis.z - axis.origin);
            if (gotX < min || max < gotX) print($"zScaleError: expected {distance} got {gotZ}");           
        }
        internal void testScaleRotationAxis(){
            testCreateAxis();
            axis.scaleRotationAxis(distance);
            float min = distance - accuracy;
            float max = distance - accuracy;
            float gotRotationAxis = axis.length(axis.rotationAxis - axis.origin);
            if (min < gotRotationAxis && gotRotationAxis < max) print($"xScaleError: expected {distance} got {gotRotationAxis}");         
        }
        internal void testSetAxis(){
            testCreateAxis();
            float minWorldAngleY = worldAngleY - accuracy, maxWorldAngleY = worldAngleY + accuracy;
            float minWorldAngleX = worldAngleX - accuracy, maxWorldAngleX = worldAngleX + accuracy;
            float minLocalAngleY = localAngleY - accuracy, maxLocalAngleY = localAngleY + accuracy;
            axis.setWorldRotation(worldAngleY,worldAngleX,localAngleY);
            axis.getWorldRotation(out float gotWorldAngleY,out float gotWorldAngleX,out float gotLocalAngleY);
            if (float.IsNaN(gotWorldAngleY)) print("gotWorldAngleY: NaN error");
            if (float.IsNaN(gotWorldAngleX)) print("gotWorldAngleX: NaN error");
            if (float.IsNaN(gotLocalAngleY)) print("gotLocalAngleY: NaN error");
            if (gotWorldAngleY < minWorldAngleY || maxWorldAngleY < gotWorldAngleY) print($"worldAngleY: expected {worldAngleY} got {gotWorldAngleY}");
            if (gotWorldAngleX < minWorldAngleX || maxWorldAngleX < gotWorldAngleX) print($"worldAngleX: expected {worldAngleX} got {gotWorldAngleX}");
            if (gotLocalAngleY < minLocalAngleY || maxLocalAngleY < gotLocalAngleY) print($"localAngleY: expected {localAngleY} got {gotLocalAngleY}");
        }
        internal void testMoveRotationAxis(){
            testCreateAxis();
            float minWorldAngleY = worldAngleY - accuracy, maxWorldAngleY = worldAngleY + accuracy;
            float minWorldAngleX = worldAngleX - accuracy, maxWorldAngleX = worldAngleX + accuracy;
            axis.moveRotationAxis(worldAngleY,worldAngleX);
            axis.getRotationAxisAngle(out float gotWorldAngleY,out float gotWorldAngleX);
            if (float.IsNaN(gotWorldAngleY)) print("gotWorldAngleY: NaN error");
            if (float.IsNaN(gotWorldAngleX)) print("gotWorldAngleX: NaN error");
            if (gotWorldAngleY < minWorldAngleY || maxWorldAngleY < gotWorldAngleY) print($"worldAngleY: expected {worldAngleY} got {gotWorldAngleY}");
            if (gotWorldAngleX < minWorldAngleX || maxWorldAngleX < gotWorldAngleX) print($"worldAngleX: expected {worldAngleX} got {gotWorldAngleX}");
        }
    }
    
    class Experiment:SourceCode{
        public void strt(){
            Axis ax = new Axis(new Vector3(5,5,5),5);
            Body lol = new Body(ax,10);
        }
    }
    Experiment exp = new Experiment();
    void Start(){
        exp.strt();

    }
    int time = 0;
    int count = 0;
    // Update is called once per frame
    void Update()
    {
        // if (time == 60 && count!= 360) {
        //     lol.rotate(lol.quat(Mathf.PI/180),lol.rotationAxis);
        //     time = 0;
        //     count++;
        //     print(count);
        //     }
        // time++;
        
    }
}
