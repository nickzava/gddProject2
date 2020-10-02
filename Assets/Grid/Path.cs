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

    //BEFORE a nodes connections are changed it should be removed from it's active path
    public void RemoveNodeFromPath(PathNode toBeChanged) 
    {
        //holds all nodes on the tree that have been checked
        HashSet<PathNode> checkedNodes = new HashSet<PathNode>();

        //starting from the given node, removes this path from any node it encounters
        void ClearPath(PathNode toClear)
        {
            checkedNodes.Add(toClear);
            toClear.ClearPath();
            foreach(PathNode p in toClear.connected)
            {
                if(!checkedNodes.Contains(p))
                {
                    ClearPath(p);
                }
            }
        }

        //traverses tree until the given node is found
        PathNode FindNode(PathNode toFind, PathNode current)
        {
            checkedNodes.Add(current);
            PathNode temp;
            foreach(PathNode p in current.connected)
            {
                if (!checkedNodes.Contains(p))
                {
                    if (p == toFind)
                    {
                        return toFind;
                    }
                    temp = FindNode(toFind, current);
                    if (temp != null)
                    {
                        return temp;
                    }
                }
            }
            return null;
        }

        if (pathBeginning != toBeChanged)
        {
            ClearPath(FindNode(toBeChanged, pathBeginning));
        }
        else
        {
            checkedNodes.Add(pathBeginning);
            foreach (PathNode p in pathBeginning.connected)
            {
                ClearPath(p);
            }
        }
    }

    //called if the path is changed
    //if the node is now connected to two different sorces they should NOT call this method but 
    //start a new path with an increased power levelinstead
    public void Remake() 
    {
        //holds all nodes on the tree that have been checked
        HashSet<PathNode> checkedNodes = new HashSet<PathNode>();

        void Propagate(PathNode node)
        {
            if (node.mPath != this)
            {
                node.SetPath(this, 1);
                mNodes.AddLast(node);
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

        Propagate(pathBeginning);
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
}
