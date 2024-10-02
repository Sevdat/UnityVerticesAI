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
        public void getWorldRotation(out float worldAngleY,out float worldAngleX,out float localAngleY){
            axis.getWorldRotation(out worldAngleY,out worldAngleX,out localAngleY);
            worldAngleY *= radianToDegree;
            worldAngleX *= radianToDegree;
            localAngleY *= radianToDegree;
        }
        public void setWorldRotation(float worldAngleY,float worldAngleX,float localAngleY){
            axis.setWorldRotation(worldAngleY*degreeToRadian,worldAngleX*degreeToRadian,localAngleY*degreeToRadian);
            if (created) setGameObjects();
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
            angleY *= radianToDegree;
            angleX *= radianToDegree;
        }
        public Vector4 quat(float angle){
             return axis.quat(angle);
        }
        public void rotate(Vector4 quat, Vector3 rotationOrigin){
            axis.rotate(quat,rotationOrigin);
            if (created) setGameObjects(); 
        }
    }

    public class KeyGeneratorSimulation:SourceCode{
        KeyGenerator keyGenerator;
        public List<GameObject> maxKeys,availableKeys,increaseKeysBy,freeKeys;
        public Color maxKeysColor,availableKeysColor,increaseKeysByColor,freeKeysColor,capacityColor;
        bool created = false;
        public void createGenerator(int amount){
            if (!created){
                keyGenerator = new KeyGenerator(amount);
                maxKeys = new List<GameObject>();
                availableKeys = new List<GameObject>();
                increaseKeysBy= new List<GameObject>();
                freeKeys= new List<GameObject>();
                freeKeysColor = new Color(0,1,0,0);
                capacityColor = new Color(0,0,1,0);
                created = true;
                fixedDisplay(freeKeys,new Vector3(0,5,10),freeKeysColor);
            }
        }
        public void deleteGenerator(){
            if (created){
                keyGenerator = null;
                maxKeys = null;
                availableKeys = null;
                increaseKeysBy = null;
                freeKeys= null;
            }
        }
        public void fixedDisplay(List<GameObject> list, Vector3 vec, Color color){
            int keyGeneratorCapacity = keyGenerator.freeKeys.Capacity;
            int keyGeneratorAvailableKeys = keyGenerator.availableKeys;
            int listCapacity = list.Capacity;
            if (listCapacity != keyGeneratorCapacity){
                int add = keyGeneratorCapacity-listCapacity;
                for (int i = 0;i<add;i++){
                    GameObject key = Instantiate(dynamicClone);
                    key.GetComponent<Renderer>().material.color = capacityColor;
                    key.transform.position = new Vector3(2*i + listCapacity,0,0)+vec;
                    list.Add(key);
                }
            }

        }
        public void generateKeys(){ //not done
            keyGenerator.generateKeys();
            if (created){
                fixedDisplay(freeKeys, new Vector3(0,5,10),freeKeysColor);
            }  
        }
    }

    void Awake(){
        dynamicClone = originalObject;
        staticClone = originalObject;
        staticClone.isStatic = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
    KeyGeneratorSimulation keyGeneratorSimulation = new KeyGeneratorSimulation();
    List<GameObject> lol = new List<GameObject>();
    void Start(){
        keyGeneratorSimulation.createGenerator(6);
        
    }
    // Update is called once per frame
    void Update(){
        
    }
}
