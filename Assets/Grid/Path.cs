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
