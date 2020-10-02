using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoManager : MonoBehaviour
{
    [SerializeField]
    ProtoTile tileObject;

    public ProtoTile[,] grid = new ProtoTile[2, 3];

    // Start is called before the first frame update
    void Start()
    {
        // Hardcoded initial setup for the prototype
        grid[0, 0] = Instantiate(tileObject);
        grid[0, 0].tileType = 't';

        grid[0, 1] = Instantiate(tileObject);
        grid[0, 1].tileType = 'x';

        grid[0, 2] = Instantiate(tileObject);
        grid[0, 2].tileType = 'l';

        grid[1, 0] = Instantiate(tileObject);
        grid[1, 0].tileType = 'l';

        grid[1, 1] = Instantiate(tileObject);
        grid[1, 1].tileType = 't';

        grid[1, 2] = Instantiate(tileObject);
        grid[1, 2].tileType = 'i';

        // Repositions all tiles in the scene
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int n = 0; n < grid.GetLength(1); n++)
            {
                grid[i, n].transform.position = new Vector3(-8f + (2.5f * i), 3.5f - (2.5f * n), 0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        grid[0, 0].power = true;

        // Power all adjacent!
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int n = 0; n < grid.GetLength(1); n++)
            {
                if (grid[i, n].power)
                {
                    PowerAdjacent(i, n);
                }
            }
        }
    }

    public void PowerAdjacent(int x, int y)
    {
        // Runs for each cardinal direction
        for (int i = 0; i < 5; i++)
        {
            // For each direction, first checks if there is a tile in bounds. Then, checks if the sender is facing that direction.
            switch (i)
            {
                case 0:
                    if (y - 1 >= 0 && y - 1 < grid.GetLength(1))
                    {
                        if (grid[x, y].connections[i] == 1 && grid[x, y - 1].connections[2] == 1)
                        {
                            grid[x, y - 1].power = true;
                        }
                        else
                        {
                            grid[x, y - 1].power = false;
                        }
                    }

                    break;
                case 1:
                    if (x + 1 >= 0 && x + 1 < grid.GetLength(0))
                    {
                        if (grid[x, y].connections[i] == 1 && grid[x + 1, y].connections[3] == 1)
                        {
                            grid[x + 1, y].power = true;
                        }
                        else
                        {
                            grid[x + 1, y].power = false;
                        }
                    }
                    break;
                case 2:
                    if (y + 1 >= 0 && y + 1 < grid.GetLength(1))
                    {
                        if (grid[x, y].connections[i] == 1 && grid[x, y + 1].connections[0] == 1)
                        {
                            grid[x, y + 1].power = true;
                        }
                        else
                        {
                            grid[x, y + 1].power = false;
                        }
                    }
                    break;
                case 3:
                    if (x - 1 >= 0 && x - 1 < grid.GetLength(0))
                    {
                        if (grid[x, y].connections[i] == 1 && grid[x - 1, y].connections[1] == 1)
                        {
                            grid[x - 1, y].power = true;
                        }
                        else
                        {
                            grid[x - 1, y].power = false;
                        }
                    }
                    break;
            }
        }
    }
}
