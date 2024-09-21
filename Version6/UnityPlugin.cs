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
    AxisSimulation testAxis2;
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
        public void setColor(int r,int g,int b,int a){
            origin.GetComponent<Renderer>().material.color = new Color(r,g,b,a);
            x.GetComponent<Renderer>().material.color = new Color(r,g,b,a);
            y.GetComponent<Renderer>().material.color = new Color(r,g,b,a);
            z.GetComponent<Renderer>().material.color = new Color(r,g,b,a);
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
        public void angle(){
            axis.findAngle(out float angleX,out float angleY,out float localY);
            print($"{angleX* 180 / Mathf.PI} {angleY* 180 / Mathf.PI} {localY* 180 / Mathf.PI}");
        }
        public void moveRotAxis(float angleX,float angleY){
            axis.rotateRotationAxis(angleX,angleY);
            if (created) setGameObjects();
        }
        public void setTheAxis(float worldAngleY,float worldAngleX,float localAngleY){
            axis.setAxis(worldAngleY,worldAngleX,localAngleY);
            if (created) setGameObjects();
        }
        public void rotate(float angle){
            Vector4 quat = axis.angledAxis(angle,axis.rotationAxis);
            axis.origin = axis.rotate(axis.origin,quat);
            axis.x = axis.rotate(axis.x,quat);
            axis.y = axis.rotate(axis.y,quat);
            axis.z = axis.rotate(axis.z,quat);
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
    void Start()
    {
        testAxis = new AxisSimulation();
        testAxis.init();
        testAxis.create(new Vector3(11,0,11), 5);
        testAxis.setTheAxis(30.0f,40.0f,40.0f);
        testAxis.setColor(1,0,0,1);
        testAxis.angle();

    }
    int time = 0;
    int count = 0;
    int count2 = 0;
    int count3 = 0;
    int count4 = 0;
    int stage = 0;

    // Update is called once per frame
    void Update(){
        // if (time == 30){
        // testAxis.setTheAxis(30.0f,30.0f,count);
        // count++;
        // time = 0;
        // }
        

        // if (time == 30){
        //     testAxis.set(count2,count3,count4);
        //     if (stage == 0) count2++;
        //     if (stage == 1) count3++;
        //     if (stage == 2) count4++;
        //     time = 0;
        //     count+=1;
        // }
        // if(count == 45) {stage += 1; count = 0;}
           
        time++;
    }
}
