using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class UnityPlugin : MonoBehaviour
{   

    public class KeyGeneratorSimulation:SourceCode{
        public KeyGenerator keyGenerator;
        public List<GameObject> maxKeys,availableKeys,increaseKeysBy,freeKeys;
        public Color maxKeysColor,availableKeysColor,increaseKeysByColor,freeKeysColor,capacityColor;
        public Vector3 displayVec = new Vector3(0,5,10);
        bool created = false;
        public void createGenerator(int amount){
            if (!created){
                keyGenerator = new KeyGenerator(amount);
                maxKeys = new List<GameObject>();
                availableKeys = new List<GameObject>();
                increaseKeysBy= new List<GameObject>();
                freeKeys= new List<GameObject>();
                freeKeysColor = new Color(0,1,0,0);
                maxKeysColor = new Color(0,0,0,0);
                availableKeysColor = new Color(0,1,1,0);
                increaseKeysByColor = new Color(1,1,0,0);
                capacityColor = new Color(0,0,1,0);
                created = true;
                showGenerator();
            }
        }
        public void deleteGameObjects(){
            if (created){
                delete(maxKeys);
                maxKeys = new List<GameObject>();
                delete(availableKeys);
                availableKeys = new List<GameObject>();
                delete(increaseKeysBy);
                increaseKeysBy = new List<GameObject>();
                delete(freeKeys);
                freeKeys = new List<GameObject>();
            }
        }
        public void replaceKeyGenerator(KeyGenerator newKeyGenerator){
            deleteGameObjects();
            keyGenerator = newKeyGenerator;
            created = true;
            showGenerator();
        }
        public void generateKeys(){
            keyGenerator.generateKeys();
            if (created){
            displayFreeKeysList(displayVec, freeKeysColor);
            displayList(maxKeys,keyGenerator.maxKeys,displayVec - new Vector3(0,1,0),maxKeysColor);
            displayList(availableKeys,keyGenerator.availableKeys,displayVec - new Vector3(0,2,0),availableKeysColor);
            } 
        }
        public void setLimit(int newLimit){
            keyGenerator.setIncreaseKeysBy(newLimit);
            if(created) {
                displayList(increaseKeysBy,keyGenerator.increaseKeysBy,displayVec - new Vector3(0,3,0),increaseKeysByColor);
            }
        }
        public int getKey(){
            int key = keyGenerator.getKey();
            if (created) {
                displayFreeKeysList(displayVec, freeKeysColor);
                displayList(availableKeys,keyGenerator.availableKeys,displayVec - new Vector3(0,2,0),availableKeysColor);
            }
            return key;
        }
        public void returnKey(int key){
            keyGenerator.returnKey(key);
            if (created) {
                displayFreeKeysList(displayVec, freeKeysColor);
                displayList(availableKeys,keyGenerator.availableKeys,displayVec - new Vector3(0,2,0),availableKeysColor);
            }
        }
        public void resetGenerator(int newMax){
            keyGenerator.resetGenerator(newMax);
            displayFreeKeysList(displayVec, freeKeysColor);
            freeKeys.TrimExcess();
            displayList(maxKeys,keyGenerator.maxKeys,displayVec - new Vector3(0,1,0),maxKeysColor);
            displayList(availableKeys,keyGenerator.availableKeys,displayVec - new Vector3(0,2,0),availableKeysColor);
        }
        void displayFreeKeysList(Vector3 vec, Color color){
            int keyGeneratorCapacity = keyGenerator.freeKeys.Capacity;
            int listCapacity = freeKeys.Capacity;
            int resize = keyGeneratorCapacity-listCapacity;
            if (resize> 0){
                for (int i = 0;i<resize;i++){
                    GameObject key = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    key.GetComponent<Renderer>().material.color = capacityColor;
                    key.transform.position = new Vector3(2*(i + listCapacity),0,0)+vec;
                    freeKeys.Add(key);
                }
            } else if (resize <0) {
                for (int i = listCapacity-1; i > -1 && i<-resize; i--){
                    Destroy(freeKeys[i]);
                    freeKeys.RemoveAt(i);
                }
            }
            if (freeKeys.Count >0){
                int keyGeneratorAvailableKeys = keyGenerator.availableKeys-1;
                int size = freeKeys.Count;
                Renderer keyColor = freeKeys[keyGeneratorAvailableKeys].GetComponent<Renderer>();
                if (keyColor.material.color == color){
                    for (int i = keyGeneratorAvailableKeys;i<size;i++){
                        Renderer render = freeKeys[i].GetComponent<Renderer>();
                        if (render.material.color == capacityColor) break;
                        render.material.color = capacityColor;
                    }
                    keyColor.material.color = color;
                } else {
                    for (int i = keyGeneratorAvailableKeys;i >-1;i--){
                        Renderer render = freeKeys[i].GetComponent<Renderer>();
                        if (render.material.color == color) break;
                        render.material.color = color;
                    }
                }
            }
        }
        void displayList(List<GameObject> list, int amount, Vector3 vec, Color color){
            int count = list.Count;
            int resize = amount - count;
            if (resize>0){
                for (int i = 0;i< resize;i++){
                    GameObject key = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    key.GetComponent<Renderer>().material.color = color;
                    key.transform.position = new Vector3(2*(i+count),0,0)+vec;
                    list.Add(key);
                }
            } else if (resize<0){
                for (int i = count-1; i>amount-1;i--){
                    Destroy(list[i]);
                    list.RemoveAt(i);
                }
            }
        }
        void delete(List<GameObject> list){
            int size = list.Count;
            for (int i = size-1;i>-1;i--){
                Destroy(list[i]);
            }
        }
        void showGenerator(){
            displayFreeKeysList(displayVec, freeKeysColor);
            displayList(maxKeys,keyGenerator.maxKeys,displayVec - new Vector3(0,1,0),maxKeysColor);
            displayList(availableKeys,keyGenerator.availableKeys,displayVec - new Vector3(0,2,0),availableKeysColor);
            displayList(increaseKeysBy,keyGenerator.increaseKeysBy,displayVec - new Vector3(0,3,0),increaseKeysByColor);
        }
    }

    void Start(){
        
    }
    // Update is called once per frame
    void Update(){
        
    }
}
