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
            axis.findAngle();
            print($"{axis.angleX* 180 / Mathf.PI} {axis.angleY* 180 / Mathf.PI}");
        }
        public void moveRotAxis(float angleX,float angleY){
            axis.moveRotationAxis(angleX,angleY);
            if (created) setGameObjects();
        }
        public void rotate(float angle){
            Vector4 quat = axis.angledAxis(angle,axis.rotationAxis);
            axis.origin = axis.rotate(axis.origin,axis.origin,quat);
            axis.x = axis.rotate(axis.origin,axis.x,quat);
            axis.y = axis.rotate(axis.origin,axis.y,quat);
            axis.z = axis.rotate(axis.origin,axis.z,quat);
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
        testAxis.moveRotAxis(60,200);
        testAxis.angle();
    }
    int count = 0;
    int count2 = 0;
    // Update is called once per frame
    void Update(){
        if (count == 30){
            // testAxis.rotate(1f);
            // testAxis.place(new Vector3(11f,0,11f));
            count = 0;
            count2++;
            print(testAxis.axis.origin);
        }
        if(count2 == 360) {
            count = 40;
            testAxis.deleteGameObjcts();
            };
        count++;
    }
}
