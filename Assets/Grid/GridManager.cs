using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//updates backend 
public class GridManager : MonoBehaviour
{
    static GridManager Instance;

    //holds
    List<List<Tile>> tiles;
    List<List<Tile>> pathNodes;
    List<Path> paths;

    // Start is called before the first frame update
    void Start()
    {
        if(Instance)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    //rotates a given tile 
    void RotatePathNode(int x, int y)
    {

    }

    void AddPath()
    {

    }

    void RemovePath(Path toRemove)
    {

    }
}
