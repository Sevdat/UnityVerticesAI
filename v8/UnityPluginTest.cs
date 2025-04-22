using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class UnityPluginTest : MonoBehaviour
{
    public class AxisTest:SourceCode {
        internal Axis axis;
        float accuracy = 0.1f;

        public AxisTest(){}
        public AxisTest(Vector3 origin, float distance){
            // axis = new Axis(origin,distance);
            testCreateAxis(origin,distance);
        }

        internal void testCreateAxis(Vector3 origin, float distance){
            if (origin != axis.origin) print($"originPositionError: expected {origin} got {origin}");

            Vector3 vecX = origin + new Vector3(distance,0,0);
            Vector3 x = axis.x;
            if (x != vecX) print($"xPositionError: expected {vecX} got {x}");

            Vector3 vecY = origin + new Vector3(0,distance,0);
            Vector3 y = axis.y;
            if (y != vecY) print($"yPositionError: expected {vecY} got {y}");

            Vector3 vecZ = origin + new Vector3(0,0,distance);
            Vector3 z = axis.z;
            if (z != vecZ) print($"zPositionError: expected {vecZ} got {z}");
        }

        internal void testMoveAxis(Vector3 add){
            Vector3 expected,got;
            Vector3 oldOrigin = axis.origin;
            Vector3 oldX = axis.origin;
            Vector3 oldY = axis.origin;
            Vector3 oldZ = axis.origin;
            Vector3 oldRotationAxis = axis.spinFuture.sphere.origin;
            axis.moveAxis(add);

            expected = oldOrigin + add;
            got= axis.origin;
            if (expected != got) print($"originMoveError: expected {expected} got {got}");

            expected = oldX + add;
            got = axis.x;
            if (expected != got) print($"originMoveError: expected {expected} got {got}");

            expected = oldY + add;
            got = axis.y;
            if (expected != got) print($"originMoveError: expected {expected} got {got}");

            expected = oldZ + add;
            got = axis.z;
            if (expected != got) print($"originMoveError: expected {expected} got {got}");
            
            expected = oldRotationAxis + add;
            got = axis.spinFuture.sphere.origin;
            if (expected != got) print($"originMoveError: expected {expected} got {got}");
        }
        internal void testPlaceAxis(Vector3 newOrigin){
            Vector3 expected,got;
            Vector3 add = newOrigin - axis.origin; 
            Vector3 oldOrigin = axis.origin;
            Vector3 oldX = axis.origin;
            Vector3 oldY = axis.origin;
            Vector3 oldZ = axis.origin;
            Vector3 oldRotationAxis = axis.spinFuture.sphere.origin;
            axis.placeAxis(newOrigin);

            expected = oldOrigin + add;
            got= axis.origin;
            if (expected != got) print($"originPlaceError: expected {expected} got {got}");

            expected = oldX + add;
            got = axis.x;
            if (expected != got) print($"xPlaceError: expected {expected} got {got}");

            expected = oldY + add;
            got = axis.y;
            if (expected != got) print($"yPlaceError: expected {expected} got {got}");

            expected = oldZ + add;
            got = axis.z;
            if (expected != got) print($"zPlaceError: expected {expected} got {got}");
            
            expected = oldRotationAxis + add;
            got = axis.spinFuture.sphere.origin;
            if (expected != got) print($"rotationAxisPlaceError: expected {expected} got {got}");
        }
        internal void testScaleAxis(float distance){
            axis.scaleAxis(distance);
            float min = distance - accuracy;
            float max = distance + accuracy;
            float gotX = axis.length(axis.x - axis.origin);
            if (gotX < min || max < gotX) print($"xScaleError: expected {distance} got {gotX}");

            float gotY = axis.length(axis.y - axis.origin);
            if (gotX < min || max < gotX) print($"yScaleError: expected {distance} got {gotY}");

            float gotZ = axis.length(axis.z - axis.origin);
            if (gotX < min || max < gotX) print($"zScaleError: expected {distance} got {gotZ}");           
        }
        internal void testScaleRotationAxis(float distance){
            axis.spinFuture.scale(distance);
            float min = distance - accuracy;
            float max = distance - accuracy;
            float gotRotationAxis = axis.length(axis.spinFuture.sphere.origin - axis.origin);
            if (min < gotRotationAxis && gotRotationAxis < max) print($"xScaleError: expected {distance} got {gotRotationAxis}");         
        }
        internal void testSetAxis(float worldAngleY, float worldAngleX, float localAngleY){
            float minWorldAngleY = worldAngleY - accuracy, maxWorldAngleY = worldAngleY + accuracy;
            float minWorldAngleX = worldAngleX - accuracy, maxWorldAngleX = worldAngleX + accuracy;
            float minLocalAngleY = localAngleY - accuracy, maxLocalAngleY = localAngleY + accuracy;
            axis.setWorldRotationInDegrees(worldAngleY,worldAngleX,localAngleY);
            axis.getWorldRotationInDegrees(out float gotWorldAngleY,out float gotWorldAngleX,out float gotLocalAngleY);
                        print($"{gotWorldAngleY} {gotWorldAngleX} {gotLocalAngleY}");
            if (float.IsNaN(gotWorldAngleY)) print("gotWorldAngleY: NaN error");
            if (float.IsNaN(gotWorldAngleX)) print("gotWorldAngleX: NaN error");
            if (float.IsNaN(gotLocalAngleY)) print("gotLocalAngleY: NaN error");
            if (gotWorldAngleY < minWorldAngleY || maxWorldAngleY < gotWorldAngleY) print($"worldAngleY: expected {worldAngleY} got {gotWorldAngleY}");
            if (gotWorldAngleX < minWorldAngleX || maxWorldAngleX < gotWorldAngleX) print($"worldAngleX: expected {worldAngleX} got {gotWorldAngleX}");
            if (gotLocalAngleY < minLocalAngleY || maxLocalAngleY < gotLocalAngleY) print($"localAngleY: expected {localAngleY} got {gotLocalAngleY}");
        }
        internal void testSetRotationAxis(float angleY, float angleX){
            float minWorldAngleY = angleY - accuracy, maxWorldAngleY = angleY + accuracy;
            float minWorldAngleX = angleX - accuracy, maxWorldAngleX = angleX + accuracy;
            axis.spinFuture.setInDegrees(angleY,angleX);
            axis.spinFuture.getInDegrees(out float gotWorldAngleY,out float gotWorldAngleX);
            if (float.IsNaN(gotWorldAngleY)) print("gotWorldAngleY: NaN error");
            if (float.IsNaN(gotWorldAngleX)) print("gotWorldAngleX: NaN error");
            if (gotWorldAngleY < minWorldAngleY || maxWorldAngleY < gotWorldAngleY) print($"worldAngleY: expected {angleY} got {gotWorldAngleY}");
            if (gotWorldAngleX < minWorldAngleX || maxWorldAngleX < gotWorldAngleX) print($"worldAngleX: expected {angleX} got {gotWorldAngleX}");
        }
    }
    Experiment exp = new Experiment();
    class Experiment:SourceCode{
        public Axis ax;
        public Body lol;
        int count = 0;
        int time = 0;
        public void strt(){
            lol = new Body(0);
        }
        public void readWrite(){
            lol.editor.readWrite();
        }
        public void trackWriter(){
            lol.editor.trackWriter();
        }
        public void readTextFiles(){
            if (time > 1){
                print(count);
                // lol.editor.reader(count);
                count++;
                if (count>35) count = 0;
                time = 0;
            } else time++;
        }
    }
    class MyData {
        public string name;
        public int score;
        public bool isActive;
    }

    void Start(){
        exp.strt();
    }
    void LateUpdate(){
        exp.readWrite();
    }



    // void Start(){
    //     // Create an instance of your data class
    //     MyData data = new MyData {
    //         name = "Player1",
    //         score = 100,
    //         isActive = true
    //     };

    //     string json = JsonUtility.ToJson(data, true);
    //     string path = Path.Combine(Application.dataPath, "MyData.json");
    //     File.WriteAllText(path, json);
    //     print("JSON file created at: " + path);
    // }
}