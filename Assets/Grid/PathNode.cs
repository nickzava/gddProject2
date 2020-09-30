using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    //tiles this node is connected to
    public List<PathNode> connected { get; private set; }
    public int mPower { get; private set; }
    //the path this node is part of, NULL if mType is none
    private Path mPath;


    //0    up          //cross    //elbow    //line
    //1    right       // 0       // 0       //
    //2    down        //3 1      //  1      //3 1
    //3    left        // 2
    List<int> directions;

    //connects this node to another node
    //it's power level is equal to the sum of all the power sources connected to it
    void ConnectTo(List<PathNode> connections) 
    { 
        //if connected to 2 + power sorces create a new path
        //starting here with a power level that's equal to the sum
        //of the power levels feeding into it
    }

    //connects this node to another path
    void RemoveConnection(PathNode toRemove) { }

    void Rotate(bool clockwise)
    {
        foreach (int direction in directions)
        {
            ////right rotation
            //direction = (direction + 1) % 4;
            ////left rotation
            //direction = direction - 1 < 0 ? 3 : direction;
        }
    }
}
