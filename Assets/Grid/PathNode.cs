using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    //tiles this node is connected to
    public List<PathNode> connected { get; private set; }

    //the path this node is part of, mPath is null and id is 0 if it's not part of a path
    public Path mPath { get; private set; }
    public int mPathId { get; private set; }

    //valuse to track if there was a meaningful change since the last finalized state
    //these values are important because nodes are frequently removed and the immediatly readded to the same
    //path so tracking their state can prevent extra triggering of events in that case
    private bool changed = false;
    private int finalId;

    //called when there is a meaningful change to the node
    //should only be called for final states of a path, not during updating
    public delegate void NodeStateChangedDelegate(int id);
    public event NodeStateChangedDelegate NodeStateChanged;

    //0    up          //cross    //elbow    //line    //tee
    //1    right       // 0       //         // 0      //
    //2    down        //3X1      //3X       // X      //3X1
    //3    left        // 2       // 2       // 2      // 2
    public List<int> directions { get; private set; }

    public PathNode(List<int> directions)
    {
        this.directions = directions;
        this.connected = new List<PathNode>();
        ClearPath();
        FinalizeState();
    }

    //removes the node from its path
    public void ClearPath()
    {
        SetPath(null, 0);
    }

    //connects this node to a path
    public void SetPath(Path newPath, int pathId)
    {
        mPath = newPath;
        mPathId = pathId;
    }

    //called after a path update, if there was a meaningful change to the node then NodeStateChanged is invoked
    public void FinalizeState()
    {
        if(changed || mPathId != finalId)
        {
            finalId = mPathId;
            changed = false;
            NodeStateChanged?.Invoke(mPathId);
        }
    }

    //connects this node to another node, should only be called by rotating pieces that
    //wish to connect to this node
    void ConnectTo(PathNode connection) 
    {
        if (!connected.Contains(connection))
        {
            connected.Add(connection);
        }
    }

    //removes connection to another node, should be called by rotating nodes that are no longer connected to this one
    void RemoveConnection(PathNode toRemove) 
    {
        connected.Remove(toRemove);
    }

    //updates direction values to represent a rotation
    public List<int> Rotate(bool isClockwise)
    {
        if (isClockwise)
        {
            for (int i = 0; i < directions.Count; i++)
            {
                directions[i] = directions[i] == 0 ? 3 : directions[i] - 1;
            }
        }
        else
        {
            for (int i = 0; i < directions.Count; i++)
            {
                directions[i] = (directions[i] + 1) % 4;
            }
        }
        return directions;
    }

    //updates the node connections after the int representation of this node is rotated
    public void UpdateConnections(List<PathNode> newConncections)
    {
        changed = true;
        //remove connections to nodes not in newConnections
        foreach(PathNode node in connected)
        {
            if(!newConncections.Contains(node))
            {
                node.RemoveConnection(this);
            }
        }

        //add connections to new connections
        foreach (PathNode node in newConncections)
        {
            if (!connected.Contains(node))
            {
                node.ConnectTo(this);
            }
        }

        //set new connections
        connected = newConncections;
    }
}
