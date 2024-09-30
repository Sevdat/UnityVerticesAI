using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class UnityPlugin : MonoBehaviour
{   
    public GameObject originalObject;
    static GameObject dynamicClone;
    GameObject staticClone;

    public class AxisSimulation:SourceCode {
        public Axis axis;
        public GameObject origin,x,y,z,rotationAxis;
        public bool created = false;
        public float degreeToRadian = Mathf.PI/180;
        public float radianToDegree = 180/Mathf.PI;
        public void createAxis(Vector3 vec, float distance){
            if (!created){
                axis = new Axis(vec,distance);
                origin = Instantiate(dynamicClone);
                x = Instantiate(dynamicClone);
                y = Instantiate(dynamicClone);
                z = Instantiate(dynamicClone);
                rotationAxis = Instantiate(dynamicClone);
                setColor(
                    new Color(1,0,0,0),new Color(0,1,0,0),new Color(0,0,1,0),
                    new Color(1,1,1,0),new Color(0,0,0,0)
                    );
                setGameObjects();
                created = true;
            }
        }
        public void delete(){
            axis = null;
            deleteGameObjects();
        }
        public void deleteGameObjects(){
            if (created){
                Destroy(origin);
                Destroy(x);
                Destroy(y);
                Destroy(z);
                Destroy(rotationAxis);
                created = false;
            }
        }
        public void setGameObjects(){
            origin.transform.position = axis.origin;
            x.transform.position = axis.x;
            y.transform.position = axis.y;
            z.transform.position = axis.z;
            rotationAxis.transform.position = axis.rotationAxis;
        }
        public void setColor(
            Color colorX,Color colorY,Color colorZ,
            Color colorOrigin, Color colorRotationAxis 
            ){
            origin.GetComponent<Renderer>().material.color = colorOrigin;
            x.GetComponent<Renderer>().material.color = colorX;
            y.GetComponent<Renderer>().material.color = colorY;
            z.GetComponent<Renderer>().material.color = colorZ;
            rotationAxis.GetComponent<Renderer>().material.color = colorRotationAxis;
        }
        public void moveAxis(Vector3 add){
            axis.moveAxis(add);
            if (created) setGameObjects();
        }
        public void placeAxis(Vector3 newOrigin){
            axis.placeAxis(newOrigin);
            if (created) setGameObjects();
        }
        public void scaleAxis(float newDistance){
            axis.scaleAxis(newDistance);
            if (created) setGameObjects();
        }
        public void scaleRotationAxis(float newDistance){
            axis.scaleRotationAxis(newDistance);
            if (created) setGameObjects();
        }
        public void setAxis(float worldAngleY,float worldAngleX,float localAngleY){
            axis.setAxis(worldAngleY*degreeToRadian,worldAngleX*degreeToRadian,localAngleY*degreeToRadian);
            if (created) setGameObjects();
        }
        public void getWorldRotation(out float worldAngleY,out float worldAngleX,out float localAngleY){
            axis.getWorldRotation(out worldAngleY,out worldAngleX,out localAngleY);
            worldAngleY *= radianToDegree;
            worldAngleX *= radianToDegree;
            localAngleY *= radianToDegree;
        }
        public void moveRotationAxis(float addAngleY,float addAngleX){
            axis.moveRotationAxis(addAngleY*degreeToRadian,addAngleX*degreeToRadian);
            if (created) setGameObjects();
        }
        public void setRotationAxis(float setAngleY,float setAngleX){
            axis.setRotationAxis(setAngleY*degreeToRadian,setAngleX*degreeToRadian);
            if (created) setGameObjects();
        }
        public void getRotationAxisAngle(out float angleY,out float angleX){
            axis.getRotationAxisAngle(out angleY,out angleX);
            angleX *= radianToDegree;
            angleY *= radianToDegree;
        }
        public Vector4 quat(float angle){
             return axis.quat(angle);
        }
        public void rotate(Vector4 quat, Vector3 rotationOrigin){
            axis.rotate(quat,rotationOrigin);
            if (created) setGameObjects(); 
        }
    }
    AxisSimulationTest testAxis;
    public class AxisSimulationTest{
        internal AxisSimulation axisSimulation = new AxisSimulation();
        float accuracy = 0.01f;
        float worldAngleY,worldAngleX,localAngleY;
        Vector3 vec = new Vector3(0,0,0); 
        float distance = 5f;

        public AxisSimulationTest(){}
        public AxisSimulationTest(Vector3 origin, float distance,float worldAngleY,float worldAngleX,float localAngleY){
            vec = origin;
            this.distance = distance;
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

        internal void testScaleAxis(){
            testCreateAxis();
            axisSimulation.scaleAxis(distance);
            float min = distance - accuracy;
            float max = distance - accuracy;
            float gotX = axisSimulation.axis.length(
                axisSimulation.x.transform.position - axisSimulation.origin.transform.position
                );
            if (min < gotX && gotX < max) print($"xPositionError: expected {distance} got {gotX}");
            
            float gotY = axisSimulation.axis.length(
            axisSimulation.y.transform.position - axisSimulation.origin.transform.position
            );
            if (min < gotY && gotY < max) print($"yPositionError: expected {distance} got {gotY}");

            float gotZ = axisSimulation.axis.length(
            axisSimulation.z.transform.position - axisSimulation.origin.transform.position
            );
            if (min < gotZ && gotZ < max) print($"zPositionError: expected {distance} got {gotZ}");           
        }
    }
    void Awake(){
        dynamicClone = originalObject;
        staticClone = originalObject;
        staticClone.isStatic = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Start(){
        testAxis = new AxisSimulationTest();
        testAxis.testScaleAxis();
    }
    
    // Update is called once per frame
    void Update(){
        
    }
}
