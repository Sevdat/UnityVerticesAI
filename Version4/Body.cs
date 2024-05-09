using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Body : MonoBehaviour
{
    public HumanBody body = new HumanBody();
    public class HumanBody: WorldBuilder.bodyStructure{
        public void fun(){
            index index0 = new index(
                    0, 
                    new indexConnections[]{
                        connections(1,7f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    5,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    3,5,3, new boundryRange[]{
                                    range(-2,2)
                                })   
                            }
                        })
                    });
            index index1 = new index(
                    1, new indexConnections[]{
                        connections(2,2f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index2 = new index(
                    2, new indexConnections[]{
                        connections(3,2f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index3 = new index(
                    3, new indexConnections[]{
                        connections(4,6f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        }),
                        connections(33,0f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        }),
                        connections(34,0f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index4 = new index(
                    4, new indexConnections[]{
                        connections(5,6f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index5 = new index(
                    5, new indexConnections[]{
                        connections(6,4f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index6 = new index(
                    6, new indexConnections[]{
                        connections(35,0f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        }),
                        connections(36,0f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index7 = new index(
                    7, new indexConnections[]{
                        connections(9,6f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index8 = new index(
                    8, new indexConnections[]{
                        connections(10,6f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index9 = new index(
                    9, new indexConnections[]{
                        connections(11,6f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index10 = new index(
                    10, new indexConnections[]{
                        connections(12,6f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index11 = new index(
                    11, new indexConnections[]{
                        connections(13,6f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index12 = new index(
                    12, new indexConnections[]{
                        connections(14,6f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index13 = new index(
                    13, new indexConnections[]{
                    });
            index index14 = new index(
                    14, new indexConnections[]{}
                    );
            index index15 = new index(
                    15, new indexConnections[]{
                        connections(17,6f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index16 = new index(
                    16, new indexConnections[]{
                        connections(18,6f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index17 = new index(
                    17, new indexConnections[]{
                        connections(19,6f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index18 = new index(
                    18, new indexConnections[]{
                        connections(20,6f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index19 = new index(
                    19, new indexConnections[]{
                        connections(37,0, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        }),
                        connections(38,0, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        }),
                        connections(39,0, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index20 = new index(
                    20, new indexConnections[]{
                        connections(40,0, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        }),
                        connections(41,0, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        }),
                        connections(42,0, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        }),
                    });
            index index21 = new index(
                    21, new indexConnections[]{
                        connections(22,2f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index22 = new index(
                    22, new indexConnections[]{}
                    );  
            index index23 = new index(
                    23, new indexConnections[]{
                        connections(30,2f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index24 = new index(
                    24, new indexConnections[]{
                        connections(25,2f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        }),
                    });
            index index25 = new index(
                    25, new indexConnections[]{}
                    );
            index index26 = new index(
                    26, new indexConnections[]{
                        connections(27,2f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        }),
                    });
            index index27 = new index(
                    27, new indexConnections[]{}
                    );
            index index28 = new index(
                    28, new indexConnections[]{
                        connections(29,2f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        }),
                    });
            index index29 = new index(
                    29, new indexConnections[]{
                    });
            index index30 = new index(
                    30, new indexConnections[]{}
                    );
            index index31 = new index(
                    31, new indexConnections[]{
                        connections(32,2f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        }),
                    });
            index index32 = new index(
                    32, new indexConnections[]{}
                    );
            index index33 = new index(
                    33, new indexConnections[]{
                        connections(15,6f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index34 = new index(
                    34, new indexConnections[]{
                        connections(16,6f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index35 = new index(
                    35, new indexConnections[]{
                        connections(7,6f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index36 = new index(
                    36, new indexConnections[]{
                        connections(8,6f, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
           index index37 = new index(
                    37, new indexConnections[]{
                        connections(21,2, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index38 = new index(
                    38, new indexConnections[]{
                        connections(23,2, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index39 = new index(
                    39, new indexConnections[]{
                        connections(31,2, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index40 = new index(
                    40, new indexConnections[]{
                        connections(24,2, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index41 = new index(
                    41, new indexConnections[]{
                        connections(26,2, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            index index42 = new index(
                    42, new indexConnections[]{
                        connections(28,2, new meshBoundry[]{
                            new meshBoundry(){
                                firstLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),
                                secondLine = boundry(
                                    0,0,0, new boundryRange[]{
                                    range(-2,2)
                                }),   
                            }
                        })
                    });
            jointList = new List<index>{
                index0,index1,index26,index27,index28,index29,
                index30,index31,index32,index2,index3,index4,
                index5,index6,index17,index18,index19,
                index20,index21,index22,index7,index8,index9,
                index10,index11,index12,index13,index14,
                index15,index16,index23,index24,
                index25,index33,index34,
                index35,index36,index37,index38,index39,
                index40,index41,index42
            };
            sortList();
        }
    }

    void Start(){
        body = new HumanBody();
        body.fun();
        Vector3 startPoint = new Vector3(20,50,20);
        body.jointHierarchy(startPoint);
        body.globalPoint(startPoint);

        //Right Arm
        body.rotateLocal(90f,33,3,false);
        body.rotateLocal(-45f,15,3,false);
        body.rotateLocal(-45f,17,3,false);
        //Right Hand
        body.rotateLocal(75f,37,2,false);
        body.rotateLocal(-60f,37,1,false);
        body.rotateLocal(25f,21,1,false);

        body.rotateLocal(-60f,38,1,false);
        body.rotateLocal(25f,23,1,false);

        body.rotateLocal(-75f,39,2,false);
        body.rotateLocal(-60f,39,1,false);
        body.rotateLocal(25f,31,1,false);
        //Right Leg
        body.rotateLocal(60f,35,3,false);
        body.rotateLocal(-60f,7,3,false);
        body.rotateLocal(-90f,11,1,false);

        //Left Arm
        body.rotateLocal(-90f,34,3,false);
        body.rotateLocal(45f,16,3,false);
        body.rotateLocal(45f,18,3,false);
        //Right Hand
        body.rotateLocal(-75f,40,2,false);
        body.rotateLocal(-60f,40,1,false);
        body.rotateLocal(25f,28,1,false);

        body.rotateLocal(-60f,41,1,false);
        body.rotateLocal(25f,26,1,false);

        body.rotateLocal(75f,42,2,false);
        body.rotateLocal(-60f,42,1,false);
        body.rotateLocal(25f,24,1,false);
        //Left Leg
        body.rotateLocal(-60f,36,3,false);
        body.rotateLocal(60f,8,3,false);
        body.rotateLocal(-90f,12,1,false);
        bod = new Vector3[]{
            new Vector3(25,11,-70)
        };
        body.diagonal(new Vector3(5,10,-20),new Vector3(25,11,70),10);
    }

    float time = 0;
    bool once = true;
    Vector3[] bod;
    void Update(){
        // if (once){
        //     renumberIndex(
        //         jointList
        //         );
        // }
        time += Time.deltaTime;
        if (time >0.01f){
            draw(0);
            time = 0f;
        }
    }
    public void draw(int choice){

            body.drawLocal(false);
            // body.moveGlobal(-1f,1);
            // body.globalPoint(1f,2);
            body.drawLocal(true);

    }
}
        // chest = WorldBuilder.moveObject(
        //     new Vector3(0f,0f,-1f),chest
        // );
        // chest = WorldBuilder.rotateObject(
        //     0,1,WorldBuilder.rotateZ,move,chest
        // );
        // WorldBuilder.createOrDeleteObject(joints.hip,true);

    //         IEnumerator Lol(){
    //     yield return joints.moveHipY();
    //     yield return joints.moveHipZ();
    // }

    //     WorldBuilder.createOrDeleteObject(joints.globalBody, false);
    // print(joints.localKneeAngle);
    // joints.moveKnee(xyMove(xAngle,zAngle));
    // joints.tempArray(joints.globalBody,0.1f);
    // joints.drawBody();

    //     public class bodyStructure : WorldBuilder{
    //     public Vector3 x = new Vector3(3,0,0);
    //     public Vector3 y = new Vector3(0,3,0);
    //     public Vector3 z = new Vector3(0,0,3);
    //     public static Vector3[] globalBody = new Vector3[]{
    //         new Vector3(20f,18f,20f),
    //         new Vector3(20f,12f,20f),
    //         new Vector3(20f,4f,20f),
    //         new Vector3(20f,2f,20f),
    //         new Vector3(20f,2f,25f),
    //     };
    //     public global globalRotation;
    //     public static local[] limbArray;
    //     public class global {
    //         public Vector3[] globalCross;
    //         public int[] globalIndex;
    //         public void moveLimb(float alphaAngles, Vector3 localRotationAxis){
    //             int index = VectorManipulator.localCrossIndex(localRotationAxis);
    //             Vector3 rotationAxis = globalCross[index];
    //             globalBody = BodyCreator.rotatePart(alphaAngles,globalIndex,rotationAxis,globalBody);

    //             for (int i =0; i<limbArray.Length;i++){
    //                 local rotateLocal = limbArray[i];
    //                 globalCross = BodyCreator.rotateAxis(
    //                     alphaAngles,rotateLocal.localCross,
    //                     rotationAxis,index,globalBody
    //                     );
    //             }
    //         }
    //         public void draw(bool drawOrDelete){
    //             Vector3[] body = BodyCreator.loadParts(globalIndex,globalBody);
    //             BitArrayManipulator.createOrDeleteObject(body, drawOrDelete);
    //         }
    //     }
    //     public class local {
    //         public Vector3[] localCross;
    //         public int[] globalIndex;
    //         public void moveLimb(float angle, Vector3 localRotationAxis){
    //             int index = VectorManipulator.localCrossIndex(localRotationAxis);
    //             Vector3 rotationAxis = localCross[index];
    //             globalBody = BodyCreator.rotatePart(angle,globalIndex,rotationAxis,globalBody);
    //         }
    //         public void moveAxis(float alphaAngles, Vector3 localRotationAxis){
    //             int index = VectorManipulator.localCrossIndex(localRotationAxis);
    //             Vector3 rotationAxis = localCross[index];
    //             localCross = BodyCreator.rotateAxis(
    //                 alphaAngles,localCross,rotationAxis,index,globalBody
    //                 );
    //         }
    //         public void draw(bool drawOrDelete){
    //             Vector3[] body = BodyCreator.loadParts(globalIndex,globalBody);
    //             BitArrayManipulator.createOrDeleteObject(body, drawOrDelete);
    //         }
    //         public void drawAxis(bool drawOrDelete){
    //             Vector3 origin = globalBody[globalIndex[0]];
    //             Vector3[] addedOrigin = VectorManipulator.addToArray(localCross,origin);
    //             BitArrayManipulator.createOrDeleteObject(addedOrigin, drawOrDelete);
    //         }
    //     }
    //     public void rotateLimb(int index, float angle, Vector3 axis, bool drawOrDelete){
    //         local limb = limbArray[index];
    //         limb.moveLimb(angle,axis);
    //         limb.draw(drawOrDelete);
    //     }
    //     public void rotateLimbAxis(int index, float angle, Vector3 axis, bool drawOrDelete){
    //         local limbAxis = limbArray[index];
    //         limbAxis.moveAxis(angle,axis);
    //         limbAxis.drawAxis(drawOrDelete);
    //     }
    //     public void rotateGlobally(float angle, Vector3 axis, bool drawOrDelete){
    //         globalRotation.moveLimb(angle,axis);
    //         globalRotation.draw(drawOrDelete);
    //     }
    //     public void initBody(){
    //         globalRotation = new global(){
    //                 globalCross = new Vector3[]{x,y,z},
    //                 globalIndex = new int[]{0,1,2,3,4}
    //             };
    //         limbArray = new local[]{
    //             new local(){
    //                 localCross = new Vector3[]{x,y,z},
    //                 globalIndex = new int[]{0,1,2,3,4}
    //             },
    //             new local(){
    //                 localCross = new Vector3[]{x,y,z},
    //                 globalIndex = new int[]{1,2,3,4}
    //             },
    //             new local(){
    //                 localCross = new Vector3[]{x,y,z},
    //                 globalIndex = new int[]{2,3,4}
    //             },
    //         };
    //     }
    // }  

    //     public bodyStructure initBody(Vector3[][] body){
    //     Vector3 x = new Vector3(3,0,0);
    //     Vector3 y = new Vector3(0,3,0);
    //     Vector3 z = new Vector3(0,0,3);
    //     int size = 0;
    //     for (int i = 0; i < body.Length; i++){
    //         size += body[i].Length;
    //     }
    //     Vector3[] vec = new Vector3[size+size*3];
    //     int count = 0;
    //     for (int i = 0; i<body.Length;i++){
    //         Vector3[] bodyVec = body[i];
    //         for (int e = 0; e< bodyVec.Length;e++){
    //             vec[count*4] = bodyVec[e];
    //             vec[count*4+1] = x;
    //             vec[count*4+2] = y;
    //             vec[count*4+3] = z;
    //             count+=1;
    //         }
    //     }
    //     bodyStructure createBody = new bodyStructure(){
    //         globalBody = vec,
    //         limbArray = new bodyPart[size]
    //     };
    //     return createBody;
    // }
    // public Vector3[] createLines(Vector3 startPoint, int[] substract){
    //     int size = substract.Length;
    //     Vector3[] verticalLine = new Vector3[substract.Length];
    //     for (int i = 0;i<size;i++){
    //         startPoint -= new Vector3(0,substract[i],0);
    //         verticalLine[i] = startPoint;
    //     }
    //     return verticalLine;
    // }
    // public void hierarchy(bodyStructure joints, int index, int[] connected){
    //     for (int i = 0; i<connected.Length;i++){
    //         connected[i] = connected[i]*4;
    //     }
    //     bodyPart move = new bodyPart(){
    //         index = index,
    //         connected = connected
    //     };
    //     joints.limbArray[index] = move;
    // }

    //     public void rotate(bodyStructure joints,float angle, int index,int rotationAxis){
    //     int originIndex = index*4;
    //     Vector3 origin = joints.globalBody[originIndex];
    //     int rotationIndex = originIndex+rotationAxis;
    //     int[] connected = joints.limbArray[index].connected;
    //     int size = connected.Length;  
    //     Vector4 quat = WorldBuilder.QuaternionClass.angledAxis(angle,joints.globalBody[rotationIndex]);

    //     for (int i = 0; i<size;i++){
    //         int indexForGlobal = connected[i];
    //        joints.globalBody[indexForGlobal]= WorldBuilder.QuaternionClass.rotate(
    //             origin,joints.globalBody[indexForGlobal],quat
    //         );
    //         if (indexForGlobal != originIndex && rotationAxis !=1)
    //         joints.globalBody[indexForGlobal+1]= WorldBuilder.QuaternionClass.rotate(
    //             origin,joints.globalBody[indexForGlobal+1],quat
    //         );
    //         if (indexForGlobal != originIndex && rotationAxis !=2)
    //         joints.globalBody[indexForGlobal+2]= WorldBuilder.QuaternionClass.rotate(
    //             origin,joints.globalBody[indexForGlobal+2],quat
    //         );
    //         if (indexForGlobal != originIndex && rotationAxis !=3)
    //         joints.globalBody[indexForGlobal+3]= WorldBuilder.QuaternionClass.rotate(
    //             origin,joints.globalBody[indexForGlobal+3],quat
    //         );
    //     }
    // }

        //     int[] verticalLine = new int[]{
        //     0,
        //     2,
        //     1,
        //     1,
        //     3,0,0,
        //     3,0,0,
        //     2,
        //     1,0,
        //     1,0,0,0,0,0,0,0,
        //     1,0,0,0,0,0,
        //     2,0,
        //     3,0,
        //     3,0

        // };
