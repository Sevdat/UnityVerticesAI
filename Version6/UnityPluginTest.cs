using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityPluginTest : MonoBehaviour
{
    AxisSimulationTest testAxis;
    public class AxisSimulationTest:UnityPlugin{
        internal AxisSimulation axisSimulation = new AxisSimulation();
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
            axisSimulation.createAxis(vec,distance);
            Vector3 origin = axisSimulation.origin.transform.position;
            if (origin != vec) print($"originPositionError: expected {vec} got {origin}");
            
            Vector3 vecX = vec + new Vector3(distance,0,0);
            Vector3 x = axisSimulation.x.transform.position;
            if (x != vecX) print($"xPositionError: expected {vecX} got {x}");
            
            Vector3 vecY = vec + new Vector3(0,distance,0);
            Vector3 y = axisSimulation.y.transform.position;
            if (y != vecY) print($"yPositionError: expected {vecY} got {y}");
                        
            Vector3 vecZ = vec + new Vector3(0,0,distance);
            Vector3 z = axisSimulation.z.transform.position;
            if (z != vecZ) print($"zPositionError: expected {vecZ} got {z}");
        }
        internal void testMoveAxis(Vector3 add){
            testCreateAxis();
            axisSimulation.moveAxis(add);
            Vector3 origin = axisSimulation.origin.transform.position;
            if (origin != vec+add) print($"originMoveError: expected {vec} got {origin}");
            
            Vector3 vecX = vec + new Vector3(distance,0,0);
            Vector3 x = axisSimulation.x.transform.position;
            if (x != vecX+add) print($"xMoveError: expected {vecX} got {x}");
            
            Vector3 vecY = vec + new Vector3(0,distance,0);
            Vector3 y = axisSimulation.y.transform.position;
            if (y != vecY+add) print($"yMoveError: expected {vecY} got {y}");
                        
            Vector3 vecZ = vec + new Vector3(0,0,distance);
            Vector3 z = axisSimulation.z.transform.position;
            if (z != vecZ+add) print($"zMoveError: expected {vecZ} got {z}");
        }
        internal void testPlaceAxis(Vector3 newOrigin){
            testCreateAxis();
            axisSimulation.placeAxis(newOrigin);
            Vector3 origin = axisSimulation.origin.transform.position;
            if (origin != newOrigin) print($"originPlaceError: expected {vec} got {origin}");
            
            Vector3 vecX = newOrigin + new Vector3(distance,0,0);
            Vector3 x = axisSimulation.x.transform.position;
            if (x != vecX) print($"xPlaceError: expected {vecX} got {x}");
            
            Vector3 vecY = newOrigin + new Vector3(0,distance,0);
            Vector3 y = axisSimulation.y.transform.position;
            if (y != vecY) print($"yPlaceError: expected {vecY} got {y}");
                        
            Vector3 vecZ = newOrigin + new Vector3(0,0,distance);
            Vector3 z = axisSimulation.z.transform.position;
            if (z != vecZ) print($"zPlaceError: expected {vecZ} got {z}");
        }

        internal void testScaleAxis(){
            testCreateAxis();
            axisSimulation.scaleAxis(distance);
            float min = distance - accuracy;
            float max = distance + accuracy;
            float gotX = axisSimulation.axis.length(
                axisSimulation.x.transform.position - axisSimulation.origin.transform.position
                );
            if (gotX < min || max < gotX) print($"xScaleError: expected {distance} got {gotX}");

            float gotY = axisSimulation.axis.length(
            axisSimulation.y.transform.position - axisSimulation.origin.transform.position
            );
            if (gotX < min || max < gotX) print($"yScaleError: expected {distance} got {gotY}");

            float gotZ = axisSimulation.axis.length(
            axisSimulation.z.transform.position - axisSimulation.origin.transform.position
            );
            if (gotX < min || max < gotX) print($"zScaleError: expected {distance} got {gotZ}");           
        }
        internal void testScaleRotationAxis(){
            testCreateAxis();
            axisSimulation.scaleRotationAxis(distance);
            float min = distance - accuracy;
            float max = distance - accuracy;
            float gotRotationAxis = axisSimulation.axis.length(
                axisSimulation.rotationAxis.transform.position - axisSimulation.origin.transform.position
                );
            if (min < gotRotationAxis && gotRotationAxis < max) print($"xScaleError: expected {distance} got {gotRotationAxis}");         
        }
        internal void testSetAxis(){
            testCreateAxis();
            float minWorldAngleY = worldAngleY - accuracy, maxWorldAngleY = worldAngleY + accuracy;
            float minWorldAngleX = worldAngleX - accuracy, maxWorldAngleX = worldAngleX + accuracy;
            float minLocalAngleY = localAngleY - accuracy, maxLocalAngleY = localAngleY + accuracy;
            axisSimulation.setWorldRotation(worldAngleY,worldAngleX,localAngleY);
            axisSimulation.getWorldRotation(out float gotWorldAngleY,out float gotWorldAngleX,out float gotLocalAngleY);
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
            axisSimulation.moveRotationAxis(worldAngleY,worldAngleX);
            axisSimulation.getRotationAxisAngle(out float gotWorldAngleY,out float gotWorldAngleX);
            if (float.IsNaN(gotWorldAngleY)) print("gotWorldAngleY: NaN error");
            if (float.IsNaN(gotWorldAngleX)) print("gotWorldAngleX: NaN error");
            if (gotWorldAngleY < minWorldAngleY || maxWorldAngleY < gotWorldAngleY) print($"worldAngleY: expected {worldAngleY} got {gotWorldAngleY}");
            if (gotWorldAngleX < minWorldAngleX || maxWorldAngleX < gotWorldAngleX) print($"worldAngleX: expected {worldAngleX} got {gotWorldAngleX}");
        }
    }
    // Start is called before the first frame update
    void Start(){
        testAxis = new AxisSimulationTest();
        testAxis.setAngle(120,50,120);
        testAxis.testMoveRotationAxis();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
