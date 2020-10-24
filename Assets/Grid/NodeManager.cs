using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//updates backend 
public class NodeManager : MonoBehaviour
{
    public static NodeManager Instance;
	[Tooltip("Set to true to generate level on start")]
	public bool generateLevelOnStart; //set to true to generate level 


	int seed;    //currently assigned manually in Unity
	int width = 10;
    int height = 10;

    //holds a 2D list of all path nodes
    //access using GetNode
    PathNode[,] pathNodes;
    //holds all active paths
    public List<Path> paths;

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
		if (generateLevelOnStart)
		{
			GridInit(true);
			NodeInit();
			PathInit();
		}
    }

    //creates a grid of path nodes
	//if noRotation true then level will generate with non-rotatable tiles
    void GridInit(bool noRotation)
    {
		float XTcounter = 0;		//adjusts spawn rate of X and T based on how many already spawned

		Random.InitState(seed);
		for (int x = 0; x < width; x++)
        {
			for (int y = 0; y < height; y++)
			{
				List<int> connections = new List<int>();
				//weighted randomness, I and L are more common than X and T
				int typeNum = (int)Mathf.Floor(Mathf.Pow(Random.value, 2) * (4 - (XTcounter / 10)));
				if (typeNum > 4) { typeNum = 4; }
				TileTypes type = (TileTypes)typeNum;
				switch (type)
				{
					case TileTypes.L:
						XTcounter -= .15f;
						connections.Add(3);
						connections.Add(2);
						break;
					case TileTypes.T:
						XTcounter++;
						connections.Add(0);
						connections.Add(3);
						connections.Add(2);
						break;
					case TileTypes.I:
						XTcounter -= .15f;
						connections.Add(0);
						connections.Add(2);
						break;
					case TileTypes.X:
						XTcounter++;
						connections.Add(3);
						connections.Add(2);
						connections.Add(1);
						connections.Add(0);
						break;
					default:
						break;
				}
				bool noRot = false;
				if (noRotation)
				{
					if (Random.value < .08f)        //5% chance of tile being no rotation
					{
						noRot = true;
					}
				}

				pathNodes[x, y] = new PathNode(connections);
				//TileManager.Instance.AddTile(x, y, type, noRot);
				int rotations = Random.Range(0, 4);
				switch (rotations)
				{
					case 0:
						TileManager.Instance.AddTile(x, y, type, noRot);
						break;
					case 1:
						StartCoroutine(TileManager.Instance.AddTile(x, y, type, noRot).RotateOverTime(true, 0.01f, 90));
						break;
					case 2:
						StartCoroutine(TileManager.Instance.AddTile(x, y, type, noRot).RotateOverTime(true, 0.01f, 180));
						break;
					case 3:
						StartCoroutine(TileManager.Instance.AddTile(x, y, type, noRot).RotateOverTime(true, 0.01f, 270));
						break;
				}
			}
        }
    }

    //connects nodes nodes together 
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

    //initalizes paths
    void PathInit(bool secondFluid = true)
    {
        paths = new List<Path>();
        AddPath(1,GetNode(0, 0));
        if (secondFluid)
        {
            AddPath(2, GetNode(width - 1, height - 1));
        }
    }

    //adds a new path
    public void AddPath(int id, PathNode start, List<Path> dependancies = null)
    {
        Path newPath = new Path(id, start, dependancies);
        paths.Add(newPath);
    }

    //destroys a path
    public void RemovePath(Path toRemove)
    {
        paths.Remove(toRemove);

        var toUpdate = toRemove.Clear();
        if (toRemove.dependancies != null)
        {
            foreach(Path p in toRemove.dependancies)
            {
                p.Remake();
            }
        }

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
        if (toRotate == null)
            return;

        //rotate the node and get the int representation of it's directions
        List<int> intDirections = toRotate.Rotate(isClockwise);

        //clear path before graph is changed
        Path nodePath = toRotate.mPath;
        changedNodes = nodePath?.Clear() ?? changedNodes;

        //update the node with its new connections
        toRotate.UpdateConnections(GetNodesFromInt(x, y, intDirections));

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
            foreach (Path p in dependantPath.dependancies)
            {
                //check to see if the dependant node is either no longer a part of the base path, or if
                //the node is no longer connected to the dependant path
                int dependantId = dependantPath.id;
                bool isPartOfDependant(PathNode toCheck)
                {
                    return toCheck.mPathId == dependantId;
                }
                if (p.GetNodeCount() == 0 || p.FindClosestNodeInPath(isPartOfDependant) == null)
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
    public PathNode GetNode(int x, int y)
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

    //duplicate of path shortest path, but search isn't restricted to a single path
    public PathNode FindClosestNode(System.Predicate<PathNode> predicate, PathNode beginning)
    {
        Queue<PathNode> toCheckQueue = new Queue<PathNode>();
        HashSet<PathNode> checkedNodes = new HashSet<PathNode>();

        void addToCheckQueue(PathNode checkThis)
        {
            toCheckQueue.Enqueue(checkThis);
            checkedNodes.Add(checkThis);
        }

        addToCheckQueue(beginning);

        //bredth first search for a matching node
        while (toCheckQueue.Count > 0)
        {
            PathNode node = toCheckQueue.Dequeue();
            foreach (PathNode toCheck in node.connected)
            {
                if (predicate(toCheck))
                {
                    return toCheck;
                }
                else if (!checkedNodes.Contains(toCheck))
                {
                    addToCheckQueue(toCheck);
                }
            }
        }

        //no node found
        return null;
    }

	//Generate level with given seed, width, and height
	public void GenerateLevel(int _seed, int _width, int _height, bool secondFluid, bool noRotation)
	{
		seed = _seed;
		width = _width;
		height = _height;
		TileManager.Instance.SetWidthHeight(_width, _height);

		GridInit(noRotation);
		NodeInit();
		PathInit(secondFluid);
	}
}
