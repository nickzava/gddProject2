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
    private Dictionary<PathNode, Tile> pathNodeToTile;

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
        pathNodeToTile = new Dictionary<PathNode, Tile>();
    }

    public void AddTile(int x, int y, TileTypes type)
    {
        //calculate location for new tile
        Vector3 location = new Vector3(x * tileSize - width/2 * tileSize, y * tileSize - height / 2 * tileSize,0);

        //create new tile
        Tile newTile =  Instantiate(tilePref, location, Quaternion.identity).GetComponent<Tile>();
        newTile.Init(x, y, type);

        //add tile to data structures
        tiles[x, y] = newTile;
        pathNodeToTile.Add(NodeManager.Instance.GetNode(x, y), newTile);
    }

	//clears all tiles
	public void ClearTiles()
	{
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				if (tiles[x, y] != null)
				{
					Destroy(tiles[x, y].gameObject);
				}
			}
		}
	}
    
    //used to get the tile that corresponds to the back end node
    public Tile GetTileFromNode(PathNode node)
    {
        return pathNodeToTile[node];
    }

	//sets width and height of tile array
	public void SetWidthHeight(int _width, int _height)
	{
		width = _width;
		height = _height;
	}
}
