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
            Index index0 = new Index(
                    0, 
                    new IndexConnections[]{
                        connections(1,7f)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(-3,3,3),corner(3,3,3),
                                    corner(-3,-3,3),corner(3,-3,3)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(-3,3,-3),corner(3,3,-3),
                                    corner(-3,-3,-3),corner(3,-3,-3)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index1 = new Index(
                    1, 
                    new IndexConnections[]{
                        connections(2,2f)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index2 = new Index(
                    2, 
                    new IndexConnections[]{
                        connections(3,2f)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index3 = new Index(
                    3, 
                    new IndexConnections[]{
                        connections(4,6f),
                        connections(33,0f),
                        connections(34,0f)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index4 = new Index(
                    4, 
                    new IndexConnections[]{
                        connections(5,6f)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index5 = new Index(
                    5, 
                    new IndexConnections[]{
                        connections(6,4f)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index6 = new Index(
                    6, 
                    new IndexConnections[]{
                        connections(35,0f),
                        connections(36,0f)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                        );
            Index index7 = new Index(
                    7, 
                    new IndexConnections[]{
                        connections(9,6f)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                        );
            Index index8 = new Index(
                    8, 
                    new IndexConnections[]{
                        connections(10,6f)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index9 = new Index(
                    9, 
                    new IndexConnections[]{
                        connections(11,6f)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index10 = new Index(
                    10, 
                    new IndexConnections[]{
                        connections(12,6f)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index11 = new Index(
                    11, 
                    new IndexConnections[]{
                        connections(13,6f)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index12 = new Index(
                    12, 
                    new IndexConnections[]{
                        connections(14,6f)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index13 = new Index(
                    13, 
                    new IndexConnections[]{
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        });
            Index index14 = new Index(
                    14, 
                    new IndexConnections[]{
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index15 = new Index(
                    15, 
                    new IndexConnections[]{
                        connections(17,6f)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index16 = new Index(
                    16, 
                    new IndexConnections[]{
                        connections(18,6f)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index17 = new Index(
                    17, 
                    new IndexConnections[]{
                        connections(19,6f)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index18 = new Index(
                    18, 
                    new IndexConnections[]{
                        connections(20,6f)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index19 = new Index(
                    19, 
                    new IndexConnections[]{
                        connections(37,0),
                        connections(38,0),
                        connections(39,0)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index20 = new Index(
                    20, 
                    new IndexConnections[]{
                        connections(40,0),
                        connections(41,0),
                        connections(42,0),
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index21 = new Index(
                    21, 
                    new IndexConnections[]{
                        connections(22,2f)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index22 = new Index(
                    22, new IndexConnections[]{
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );  
            Index index23 = new Index(
                    23, 
                    new IndexConnections[]{
                        connections(30,2f)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index24 = new Index(
                    24, 
                    new IndexConnections[]{
                        connections(25,2f),
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index25 = new Index(
                    25, 
                    new IndexConnections[]{
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index26 = new Index(
                    26, new IndexConnections[]{
                        connections(27,2f),
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index27 = new Index(
                    27, 
                    new IndexConnections[]{
                    },
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index28 = new Index(
                    28, 
                    new IndexConnections[]{
                        connections(29,2f),
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index29 = new Index(
                    29, 
                    new IndexConnections[]{
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index30 = new Index(
                    30, 
                    new IndexConnections[]{
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index31 = new Index(
                    31, 
                    new IndexConnections[]{
                        connections(32,2f),
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index32 = new Index(
                    32, 
                    new IndexConnections[]{
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index33 = new Index(
                    33, 
                    new IndexConnections[]{
                        connections(15,6f)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index34 = new Index(
                    34, 
                    new IndexConnections[]{
                        connections(16,6f)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index35 = new Index(
                    35, 
                    new IndexConnections[]{
                        connections(7,6f)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index36 = new Index(
                    36, 
                    new IndexConnections[]{
                        connections(8,6f)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
           Index index37 = new Index(
                    37, 
                    new IndexConnections[]{
                        connections(21,2)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index38 = new Index(
                    38, 
                    new IndexConnections[]{
                        connections(23,2)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index39 = new Index(
                    39, 
                    new IndexConnections[]{
                        connections(31,2)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index40 = new Index(
                    40, 
                    new IndexConnections[]{
                        connections(24,2)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index41 = new Index(
                    41, 
                    new IndexConnections[]{
                        connections(26,2)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            Index index42 = new Index(
                    42, 
                    new IndexConnections[]{
                        connections(28,2)
                    }, 
                    new MeshStructure(){
                            drawCube = new Cube(){
                                frontSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    ),
                                backSquare = distanceFromCenter(
                                    corner(0,0,0),corner(0,0,0),
                                    corner(0,0,0),corner(0,0,0)
                                    )
                            },
                            deleteFromCube = new Cube[]{
                                new Cube(){
                                    frontSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        ),
                                    backSquare = distanceFromCenter(
                                        corner(0,0,0),corner(0,0,0),
                                        corner(0,0,0),corner(0,0,0)
                                        )
                                }
                            }
                        }
                    );
            jointList = new List<Index>{
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
