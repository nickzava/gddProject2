using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//updates backend 
public class NodeManager : MonoBehaviour
{
    public static NodeManager Instance;
	public int seed;	//currently assigned manually in Unity

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
		Random.InitState(seed);
		for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                List<int> connections = new List<int>();
				//weighted randomness, I and L are more common than X and T
                int typeNum = (int)Mathf.Floor(Mathf.Pow(Random.value, 2) * 4);
                TileTypes type = (TileTypes)typeNum;
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
        AddPath(1,GetNode(0, 0));
        AddPath(2, GetNode(width - 1, height - 1));
    }

    public void AddPath(int id, PathNode start, List<PathNode> dependancies = null)
    {
        Path newPath = new Path(id, start, dependancies);
        paths.Add(newPath);
    }

    //destroys a path
    public void RemovePath(Path toRemove)
    {
        paths.Remove(toRemove);

        var toUpdate = toRemove.Clear();
        foreach (PathNode pn in toUpdate)
        {
            pn.FinalizeState();
        }
    }

    //rotates a given tile 
    public void RotatePathNode(int x, int y, bool isClockwise)
    {
        //track all changed nodes
        LinkedList<PathNode> changedNodes = new LinkedList<PathNode>();
        void AddToChanged(IEnumerable<PathNode> toAdd)
        {
            foreach(PathNode pn in toAdd)
            {
                changedNodes.AddLast(pn);
            }
        }

        //get the node to rotate
        PathNode toRotate = GetNode(x, y);
        //rotate the node and get the int representation of it's directions
        List<int> intDirections = toRotate?.Rotate(isClockwise);

        //clear path before graph is changed
        Path nodePath = toRotate.mPath;
        changedNodes = nodePath?.Clear() ?? changedNodes;

        //update the node with its new connections
        toRotate?.UpdateConnections(GetNodesFromInt(x, y, intDirections));

        //if this node has a path, remake it, otherwise look for other paths that should be remade
        //only one path that is touching this node needs to be remade, if there is more than one path 
        //touching this node a collision will occur remaking all colliding paths
        if (nodePath != null)
        {
            AddToChanged(nodePath.Remake());
        }
        else
        {
            List<PathNode> connected = toRotate.connected;
            //Remake paths next to this one
            for (int i = 0; i < connected.Count; i++)
            {
                if (connected[i].mPath != null)
                {
                    AddToChanged(connected[i].mPath.Remake());
                    break;
                }
            }
        }

        //check to make sure all dependant paths are still connected to 
        //their dependancies, if not remove secondary paths
        List<Path> toRemove = null;
        foreach (Path dependantPath in paths)
        {
            if (dependantPath.dependancies == null)
                continue;
            foreach (PathNode pn in dependantPath.dependancies)
            {
                //check to see if the dependant node is either no longer a part of the base path, or if
                //the node is no longer connected to the dependant path
                if (pn.mPath == null || !pn.connected.Exists((depNode) => { return depNode.mPathId == dependantPath.id; }))
                {
                    toRemove = toRemove ?? new List<Path>();
                    toRemove.Add(dependantPath);
                }
            }
        }
        if (toRemove != null)
        {
            foreach (Path remove in toRemove)
            {
                RemovePath(remove);
            }
        }

        foreach (PathNode p in changedNodes)
        {
            p.FinalizeState();
        }
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
}
