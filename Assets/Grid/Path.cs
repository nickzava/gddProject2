using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-----Matthew's Domain----//
public class Path
{
    //holds the first node in the path
    PathNode pathBeginning;
    //holds all the nodes in this path
    LinkedList<PathNode> mNodes;

    public Path(PathNode start) { }

    //updates the type of all nodes in this path based on the change made in this path
    public void UpdatePathValues(PathNode changedNode, List<PathNode> newNodeConnections) { }

    //called a tile is changed to be connected to a path they weren't previously connected to
    //if they are now connected to two different sorces they should start a new path instead
    public void AddNodes(PathNode newConnection) { }

    //called before the path is removed
    public void Clear() { }
}
