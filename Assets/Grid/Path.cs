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
    public List<PathNode> dependancies;
    
    public int id { get; private set; }

    public Path(int id, PathNode start, List<PathNode> dependancies = null) 
    {
        this.id = id;
        this.dependancies = dependancies;
        mNodes = new LinkedList<PathNode>();

        this.start = start;
        AddNodeToPath(start);
        start.FinalizeState();

        Remake();
    }

    void AddNodeToPath(PathNode toAdd)
    {
        toAdd.SetPath(this, id);
        mNodes.AddLast(toAdd);
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
        List<PathNode> newPathPoints = new List<PathNode>();

        //recursively adds this path to all nodes connected to this one
        void Propagate(PathNode node)
        {
            checkedNodes.Add(node);
            if (dependancies != null &&dependancies.Contains(node))
                return;
            //collisions with other paths
            Path path = node.mPath;
            if (path != null && node.mPath != this)
            {
                //check to see if the other path is a dependancy of this path or vice versa
                if ((dependancies == null || !dependancies.Exists((dependancy) => { return dependancy.mPath == path; }))
                    && (path.dependancies == null || !path.dependancies.Exists((dependancy) => { return dependancy.mPath == this; })))
                {
                    newPathPoints.Add(node);
                    return;
                }
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
        if(newPathPoints.Count > 1)
        {
            //TODO handle multiple collisions
        }
        else if (newPathPoints.Count != 0)
        {
            Path otherPath = newPathPoints[0].mPath;
            List<PathNode> dependancies = new List<PathNode>();
            dependancies.Add(newPathPoints[0].connected.Find((node) => { return node.mPath == this; }));
            dependancies.Add(newPathPoints[0].connected.Find((node) => { return node.mPath == otherPath; }));
            NodeManager.Instance.AddPath(3,newPathPoints[0],dependancies);
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

    LinkedList<PathNode> GetShortestPath(PathNode toFind, PathNode beginning = null)
    {
        beginning = beginning ?? start;

        Queue<LinkedList<PathNode>> paths = new Queue<LinkedList<PathNode>>();
        LinkedList<PathNode> inital = new LinkedList<PathNode>();
        inital.AddLast(beginning);
        paths.Enqueue(inital);

        //bredth first search for toFind
        while(paths.Count > 0)
        {
            LinkedList<PathNode> currentPath = paths.Dequeue();
            PathNode pathEnd = currentPath.Last.Value;
            foreach (PathNode toCheck in pathEnd.connected)
            {
                if(toCheck == toFind)
                {
                    return currentPath;
                }
                LinkedList<PathNode> currentCopy = new LinkedList<PathNode>(currentPath);
                currentCopy.AddLast(toCheck);
                paths.Enqueue(currentCopy);
            }
        }

        //no path found
        return null;
    }

    LinkedList<PathNode> GetLongest()
    {
        LinkedList<PathNode> LongestSubPath(PathNode start, HashSet<PathNode> checkedNodes)
        {
            checkedNodes.Add(start);
            LinkedList<PathNode> currentLongest = null;
            LinkedList<PathNode> temp;
            checkedNodes.Add(start);
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
