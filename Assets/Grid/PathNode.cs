using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    //tiles this node is connected to
    public List<PathNode> connected { get; private set; }
    public int mPower { get; private set; }
    //the path this node is part of, NULL if power is 0 / isn't connected to a power source
    public Path mPath { get; private set; }

    public delegate void NodeStateChangedDelegate(int power);
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
    }

    public void ClearPath()
    {
        mPath = null;
        mPower = 0;
        NodeStateChanged?.Invoke(mPower);
    }

    public void SetPath(Path newPath, int pathPower)
    {
        mPath = newPath;
        mPower = pathPower;
        NodeStateChanged?.Invoke(mPower);
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

    //removes connection to another path
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
                directions[i] = (directions[i] + 1) % 4;
            }
        }
        else
        {
            for (int i = 0; i < directions.Count; i++)
            {
                directions[i] = directions[i] - 1 < 0 ? 3 : directions[i] - 1;
            }
        }
        return directions;
    }

    public void UpdateConnections(List<PathNode> newConncections)
    {
        //if the node has a path, update the path first
        //temporary value is created so the path refrence isn't
        //lost while the node is updated
        Path oldPath = mPath;
        oldPath?.Clear();

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

        //Remake paths next to this one
        for(int i = 0; i < connected.Count; i++)
        {
            if(connected[i].mPath != null && connected[i].mPath != oldPath)
            {
                connected[i].mPath.Remake();
                //can break after the first remake,
                //any paths that collide with a remade path
                //will also be updated
                break;
            }
        }

        //remake paths
        if(mPath != oldPath)
            oldPath?.Remake();
        mPath?.Remake();
    }
}
