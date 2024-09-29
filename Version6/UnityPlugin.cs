using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnityPlugin : MonoBehaviour
{   
    public GameObject originalObject;
    static GameObject dynamicClone;
    GameObject staticClone;

    AxisSimulation testAxis;
    public class AxisSimulation:SourceCode {
        public Axis axis;
        public bool created = false;
        public GameObject origin,x,y,z,rotationAxis;
        public void init(){
            if (!created){
                origin = Instantiate(dynamicClone);
                x = Instantiate(dynamicClone);
                y = Instantiate(dynamicClone);
                z = Instantiate(dynamicClone);
                rotationAxis = Instantiate(dynamicClone);
                created = true;
            }
        }
        public void deleteGameObjcts(){
            if (created){
                Destroy(origin);
                Destroy(x);
                Destroy(y);
                Destroy(z);
                Destroy(rotationAxis);
                created = false;
            }
        }
        public void setColor(
            Color colorX,Color colorY ,Color colorZ,
            Color colorOrigin, Color colorRotationAxis 
            ){
            origin.GetComponent<Renderer>().material.color = colorOrigin;
            x.GetComponent<Renderer>().material.color = colorX;
            y.GetComponent<Renderer>().material.color = colorY;
            z.GetComponent<Renderer>().material.color = colorZ;
            rotationAxis.GetComponent<Renderer>().material.color = colorRotationAxis;
        }
        public void setGameObjects(){
            origin.transform.position = axis.origin;
            x.transform.position = axis.x;
            y.transform.position = axis.y;
            z.transform.position = axis.z;
            rotationAxis.transform.position = axis.rotationAxis;
        }
        public void create(Vector3 vec, float distance){
            axis = new Axis(vec,distance);
            init();
            setGameObjects();
        }
        public void delete(){
            axis = null;
            deleteGameObjcts();
        }
        public void move(Vector3 add){
            axis.moveAxis(add);
            if (created) setGameObjects();
        }
        public void place(Vector3 newOrigin){
            axis.placeAxis(newOrigin);
            if (created) setGameObjects();
        }
        public void setAxis(float worldAngleY,float worldAngleX,float localAngleY){
            axis.setAxis(worldAngleY,worldAngleX,localAngleY);
            if (created) setGameObjects();
        }
        public void getWorldRotation(){
            axis.getWorldRotation(out float angleX,out float angleY,out float localY);
            print($"{angleX* 180 / Mathf.PI} {angleY* 180 / Mathf.PI} {localY* 180 / Mathf.PI}");
        }
        public void moveRotationAxis(float addAngleY,float addAngleX){
            axis.moveRotationAxis(addAngleY,addAngleX);
            if (created) setGameObjects();
        }
        public void setRotationAxis(float setAngleY,float setAngleX){
            axis.setRotationAxis(setAngleY,setAngleX);
            if (created) setGameObjects();
        }
        public void getRotationAxisAngle(out float angleY,out float angleX){
            axis.getRotationAxisAngle(out angleY,out angleX);
        }
        public Vector4 quat(float angle){
             return axis.quat(angle);
        }
        public void rotate(Vector4 quat, Vector3 rotationOrigin){
            axis.rotate(quat,rotationOrigin);
            if (created) setGameObjects(); 
        }
        public void set(float rotateWorldY,float rotateWorldX,float rotateLocalY){
            axis.setAxis(rotateWorldY,rotateWorldX,rotateLocalY);
            if (created) setGameObjects();
        }
    }
    void Awake(){
        dynamicClone = originalObject;
        staticClone = originalObject;
        staticClone.isStatic = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Start(){
        testAxis = new AxisSimulation();
        testAxis.init();
        testAxis.create(new Vector3(11,0,11), 5);
        testAxis.setAxis(170.0f,189.0f,196);
        testAxis.setColor(
            new Color(1,0,0,1),new Color(1,0,0,1),new Color(1,0,0,1),
            new Color(1,0,0,1),new Color(0,0,1,0)
            );
        testAxis.getWorldRotation();
    }
    int time = 0;
    int count = 0;

    // Update is called once per frame
    void Update(){
        if (time == 30){
        testAxis.setAxis(30.0f,40.0f,count);
        count++;
        time = 0;
        }    
        time++;
    }
}
