using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;

    int width = 10;
    int height = 10;

    public float tileSize;
    public GameObject tilePref;

    private Tile[,] tiles;

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
        tiles = new Tile[width, height];
    }

    public void AddTile(int x, int y, TileTypes type)
    {
        Vector3 location = new Vector3(x * tileSize - width/2 * tileSize, y * tileSize - height / 2 * tileSize,0);
        Tile newTile =  Instantiate(tilePref, location, Quaternion.identity).GetComponent<Tile>();
        tiles[x, y] = newTile;
        newTile.gridX = x;
        newTile.gridY = y;
        newTile.mType = type;
        NodeManager.Instance.AddMethodToNodeEvent(x, y, newTile.SetPower);
        newTile.SetPower(0);
    }
}
