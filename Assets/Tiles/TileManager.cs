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
	public GameObject noRotPref;

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

    public Tile AddTile(int x, int y, TileTypes type, bool noRot)
    {
        //calculate location for new tile
        Vector3 location = new Vector3(x * tileSize - width/2 * tileSize, y * tileSize - height / 2 * tileSize,0);

        //create new tile
        Tile newTile;
        GameObject newGo = Instantiate(tilePref, location, Quaternion.identity);
		if (!noRot)
		{
			newTile = newGo.AddComponent<BaseTile>();
		}
		else    //if noRot is true, then a noRotPrefab is instantiated
        {
            newTile = newGo.AddComponent<NoRotTile>();
            newGo.transform.Find("Chains").GetComponent<SpriteRenderer>().enabled = true;
        }
        newTile.Init(x, y, type);

        //add tile to data structures
        tiles[x, y] = newTile;
        pathNodeToTile.Add(NodeManager.Instance.GetNode(x, y), newTile);

		return newTile;
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
        GameObject.FindObjectOfType<AthameManager>().ClearPath();
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
