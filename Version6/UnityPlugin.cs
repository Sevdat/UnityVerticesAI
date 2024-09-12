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
        public GameObject origin,x,y,z;
        public void init(){
            origin = Instantiate(dynamicClone);
            x = Instantiate(dynamicClone);
            y = Instantiate(dynamicClone);
            z = Instantiate(dynamicClone);
        }
        public void place(Vector3 vec, float distance){
            axis = new Axis(vec,distance);
            origin.transform.position = axis.origin;
            x.transform.position = axis.x;
            y.transform.position = axis.y;
            z.transform.position = axis.z;
        }

        public void rotate(float angle, Vector3 rotationAxis){
            Vector4 angledAxis = axis.angledAxis(angle, rotationAxis);

            axis.origin = axis.rotate(axis.origin,axis.origin,angledAxis);
            axis.x = axis.rotate(axis.origin,axis.x,angledAxis);
            axis.y = axis.rotate(axis.origin,axis.y,angledAxis);
            axis.z = axis.rotate(axis.origin,axis.z,angledAxis);

            origin.transform.position = axis.origin;
            x.transform.position = axis.x;
            y.transform.position = axis.y;
            z.transform.position = axis.z;
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
        testAxis.place(new Vector3(0,0,0), 5);
    }
    int count = 0;
    int count2 = 0;
    // Update is called once per frame
    void Update()
    {
        if (count == 30){
            testAxis.rotate(1,new Vector3(1,1,1));
            count = 0;
            count2++;
            print(testAxis.axis.origin);
        }
        if(count2 == 360) count = 40;
        count++;
    }
}
