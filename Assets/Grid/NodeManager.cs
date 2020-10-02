using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//updates backend 
public class NodeManager : MonoBehaviour
{
    public static NodeManager Instance;

    int width = 10;
    int height = 10;

    //holds a 2D list of all path nodes
    //access using GetNode
    PathNode[,] pathNodes;
    //holds all active paths
    List<Path> paths;

    // Start is called before the first frame update
    void Awake()
    {
        if(Instance)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        pathNodes = new PathNode[width,height];
        paths = new List<Path>();
    }

    private void Start()
    {
        GridInit();
        NodeInit();
        PathInit();
    }

    void GridInit()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                List<int> connections = new List<int>();
                TileTypes type = (TileTypes)Random.Range(0, 4);
                switch (type)
                {
                    case TileTypes.L:
                        connections.Add(3);
                        connections.Add(2);
                        break;
                    case TileTypes.T:
                        connections.Add(3);
                        connections.Add(2);
                        connections.Add(1);
                        break;
                    case TileTypes.I:
                        connections.Add(2);
                        connections.Add(0);
                        break;
                    case TileTypes.X:
                        connections.Add(3);
                        connections.Add(2);
                        connections.Add(1);
                        connections.Add(0);
                        break;
                    default:
                        break;
                }

                pathNodes[x, y] = new PathNode(connections);
                TileManager.Instance.AddTile(x, y, type);
            }
        }
    }

    void NodeInit()
    {
        PathNode toInit;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                toInit = GetNode(x, y);
                toInit.UpdateConnections(GetNodesFromInt(x, y, toInit.directions));
            }
        }
    }

    void PathInit()
    {
        paths.Add(new Path(GetNode(0, 0)));
    }

    //rotates a given tile 
    public void RotatePathNode(int x, int y, bool isClockwise)
    {
        //get the node to rotate
        PathNode toRotate = GetNode(x, y);
        //rotate the node and get the int representation of it's directions
        List<int> intDirections = toRotate?.Rotate(isClockwise);
        //update the node with its new connections
        toRotate?.UpdateConnections(GetNodesFromInt(x, y, intDirections));
    }

    //gets a node from the 
    private PathNode GetNode(int x, int y)
    {
        if (x < width &&
            x >= 0 &&
            y < height &&
            y >= 0)
            return pathNodes[x,y];
        return null;
    }

    //used to be notified of a change in the node's state
    public void AddMethodToNodeEvent(int x, int y, PathNode.NodeStateChangedDelegate method)
    {
        GetNode(x, y).NodeStateChanged += method;
    }

    //gets a list of nodes from an int representation of the tile's orientation
    List<PathNode> GetNodesFromInt(int x, int y, in List<int> connections)
    {
        List<PathNode> nodes = new List<PathNode>();
        PathNode tempNode = null;
        foreach(int direction in connections)
        {
            switch(direction)
            {
                case 0:
                    tempNode = GetNode(x, y + 1);
                    if (tempNode != null && !tempNode.directions.Contains(2))
                        tempNode = null;
                    break;
                case 1:
                    tempNode = GetNode(x + 1, y);
                    if (tempNode != null && !tempNode.directions.Contains(3))
                        tempNode = null;
                    break;           
                case 2:
                    tempNode = GetNode(x, y - 1);
                    if (tempNode != null && !tempNode.directions.Contains(0))
                        tempNode = null;
                    break;           
                case 3:
                    tempNode = GetNode(x - 1, y);
                    if (tempNode != null && !tempNode.directions.Contains(1))
                        tempNode = null;
                    break;
            }
            if(tempNode != null)
            {
                nodes.Add(tempNode);
            }
        }
        return nodes;
    }

    //creates a new path
    public void AddPath()
    {

    }

    //destroys a path
    public void RemovePath(Path toRemove)
    {

    }
}
