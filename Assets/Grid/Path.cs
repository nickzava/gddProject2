using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-----Matthew's Domain----//
public class Path
{
    //holds the first node in the path
    private PathNode start;
    //holds all the nodes in this path
    private LinkedList<PathNode> mNodes;
    //nodes connected to base paths that feed into this path
    //only needed for second level paths
    public List<Path> dependancies;
    
    public int id { get; private set; }
    public int Count { get { return mNodes?.Count ?? 0; } }

    public Path(int id, PathNode start, List<Path> dependancies = null) 
    {
        this.id = id;
        this.dependancies = dependancies;
        mNodes = new LinkedList<PathNode>();

        LinkedList<PathNode> changedNodes = new LinkedList<PathNode>();
        void AddToChanged(IEnumerable<PathNode> toAdd)
        {
            if (toAdd == null) return;
            foreach (PathNode pn in toAdd)
            {
                changedNodes.AddLast(pn);
            }
        }

        //if the start node is part of another path, remove it and update its path
        this.start = start;
        Path startPath = start.mPath;
        changedNodes = startPath?.Clear() ?? changedNodes;
        AddNodeToPath(start);
        AddToChanged(startPath?.Remake());
        AddToChanged(Remake());

        foreach(PathNode pn in changedNodes)
        {
            pn.FinalizeState();
        }
    }

    void AddNodeToPath(PathNode toAdd)
    {
        toAdd.SetPath(this, id);
        mNodes.AddLast(toAdd);
    }

    void RemoveNode(PathNode toRemove)
    {
        mNodes.Remove(toRemove);
    }

    public int GetNodeCount()
    {
        return mNodes.Count;
    }

    //called if the path is changed
    //if the node is now connected to two different sorces they should NOT call this method but 
    //start a new path with an increased power levelinstead
    public IEnumerable<PathNode> Remake() 
    {
        //add start to path
        start.SetPath(this, id);
        mNodes = new LinkedList<PathNode>();
        mNodes.AddLast(start);

        //holds all nodes on the tree that have been checked
        HashSet<PathNode> checkedNodes = new HashSet<PathNode>();

        //holds nodes that could be a point for a new path
        List<PathNode> collisions = new List<PathNode>();
        List<List<Path>> newPathDependancies = new List<List<Path>>();

        //recursively adds this path to all nodes connected to this one
        void Propagate(PathNode node)
        {
            checkedNodes.Add(node);
            if (dependancies != null &&dependancies.Contains(node.mPath))
                return;
            //check for collisions with other paths
            Path otherPath = node.mPath;
            if (otherPath != null && otherPath.id != id)
            {
                //check to see if the other path is a dependancy of this path or vice versa
                if ((dependancies == null || !dependancies.Exists((thisDependancy) => { return thisDependancy == otherPath; }))
                    && (otherPath.dependancies == null || !otherPath.dependancies.Exists((otherDependancy) => { return otherDependancy == this; })))
                {
                    collisions.Add(node);
                    newPathDependancies.Add(new List<Path> { this, node.mPath });
                }
                return;
            }
            AddNodeToPath(node);
            foreach (PathNode p in node.connected)
            {
                if (!checkedNodes.Contains(p))
                {
                    Propagate(p);
                }
            }
        }

        //update graph values
        Propagate(start);

        //add any new paths that need to be created
        if(collisions.Count > 0)
        {
            PathNode GetNearestCombinePoint(PathNode searchStart)
            {
                bool IsCombinePoint(PathNode toCheck)
                {
                    return (toCheck.directions.Count > 2 &&
                        toCheck.mPath.start != toCheck);
                }
                //return search start if no valid node is found
                return NodeManager.Instance.FindClosestNode(IsCombinePoint,searchStart) ?? searchStart;
            }

            if (collisions.Count != 0)
            {
                NodeManager.Instance.AddPath(3, GetNearestCombinePoint(collisions[0]), newPathDependancies[0]);
                return checkedNodes;
            }

            //TODO multiple collisions
            Debug.Log("Multiple collisions! Path not created");
        }
        return checkedNodes;
    }

    //called before the path is changed or removed
    public LinkedList<PathNode> Clear() 
    {
        LinkedList<PathNode> changedNodes = mNodes;
        foreach(PathNode n in mNodes)
        {
            n.ClearPath();
        }
        return changedNodes;
    }

    //find a sequence of nodes in the path or directly next to the path that satisfies the given predicate
    public LinkedList<PathNode> GetShortestPath(System.Predicate<PathNode> predicate, PathNode beginning = null)
    {
        beginning = beginning ?? start;

        Queue<LinkedList<PathNode>> paths = new Queue<LinkedList<PathNode>>();
        LinkedList<PathNode> inital = new LinkedList<PathNode>();
        inital.AddLast(beginning);
        paths.Enqueue(inital);

        //bredth first search for a matching node
        while(paths.Count > 0)
        {
            LinkedList<PathNode> currentPath = paths.Dequeue();
            PathNode pathEnd = currentPath.Last.Value;
            foreach (PathNode toCheck in pathEnd.connected)
            {
                if (predicate(toCheck))
                {
                    currentPath.AddLast(toCheck);
                    return currentPath;
                }
                else if (toCheck.mPathId == id && !currentPath.Contains(toCheck))
                {
                    LinkedList<PathNode> currentCopy = new LinkedList<PathNode>(currentPath);
                    currentCopy.AddLast(toCheck);
                    paths.Enqueue(currentCopy);
                }
            }
        }

        //no path found
        return null;
    }

    public PathNode FindClosestNodeInPath(System.Predicate<PathNode> predicate, PathNode beginning = null)
    {
        beginning = beginning ?? start;

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
                else if (!checkedNodes.Contains(toCheck) && toCheck.mPath == this)
                {
                    addToCheckQueue(toCheck);
                }
            }
        }

        //no node found
        return null;
    }

    //returns a sequence of nodes from the beginning to toFind
    public LinkedList<PathNode> GetShortestPath(PathNode toFind, PathNode beginning = null)
    {
        return GetShortestPath((n) => { return n == toFind; }, beginning);
    }

    //returns a linked list of nodes that represents the longest unbroken sequence of path nodes in the path
    public LinkedList<PathNode> GetLongestSequenceInPath()
    {
        //local function used to recursively search for the longest path
        LinkedList<PathNode> LongestSubPath(PathNode start, HashSet<PathNode> checkedNodes)
        {
            checkedNodes.Add(start);
            LinkedList<PathNode> currentLongest = null;
            LinkedList<PathNode> temp;
            foreach(PathNode p in start.connected)
            {
                if (!checkedNodes.Contains(p))
                {
                    temp = LongestSubPath(p, new HashSet<PathNode>(checkedNodes));
                    if (currentLongest == null || temp.Count > currentLongest.Count)
                    {
                        currentLongest = temp;
                    }
                }
            }
            currentLongest = currentLongest == null ? new LinkedList<PathNode>() : currentLongest;
            currentLongest.AddFirst(start);
            return currentLongest;
        }

        return LongestSubPath(start, new HashSet<PathNode>());
    }
}
