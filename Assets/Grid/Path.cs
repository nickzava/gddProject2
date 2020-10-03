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

    public Path(PathNode start) 
    {
        this.pathBeginning = start;
        mNodes = new LinkedList<PathNode>();
        Remake();
    }

    void AddNodeToPath(PathNode toAdd)
    {
        toAdd.SetPath(this, 1);
        mNodes.AddLast(toAdd);
    }

    //called if the path is changed
    //if the node is now connected to two different sorces they should NOT call this method but 
    //start a new path with an increased power levelinstead
    public void Remake() 
    {
        //holds all nodes on the tree that have been checked
        HashSet<PathNode> checkedNodes = new HashSet<PathNode>();

        //holds nodes that could be a point for a new path
        List<PathNode> newPathPoints = new List<PathNode>();

        void Propagate(PathNode node)
        {
            if (node.mPath != this)
            {
                AddNodeToPath(node);
            }
            checkedNodes.Add(node);
            foreach (PathNode p in node.connected)
            {
                if (!checkedNodes.Contains(p))
                {
                    Propagate(p);
                }
            }
        }

        //update graph values
        Propagate(pathBeginning);
        var longest = GetLongest();
        foreach(PathNode p in longest)
        {
            p.SetPath(this, 2);
        }
    }

    //called before the path is removed
    public void Clear() 
    { 
        foreach(PathNode n in mNodes)
        {
            n.ClearPath();
        }
        pathBeginning.SetPath(this, 1);
        mNodes = new LinkedList<PathNode>();
        mNodes.AddLast(pathBeginning);
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

        return LongestSubPath(pathBeginning, new HashSet<PathNode>());
    }
}
