using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;



//visual representation of a PathNode
public class BaseTile : Tile
{
    //rotates the tile based on the type of click
    //right click -> clockwise rotation
    //left click -> counterclockwise rotation
    // Will not activate if the rotate coroutine is currently active
    void OnClick(bool isLeftClick)
    {
        if (!isRotating)
        {
            StartCoroutine(RotateOverTime(isLeftClick, 0.15f));
        }
        else
        {
            queuedRotate = true;
        }
    }

    // Handles input while mouse is hovering on the tile
    private void OnMouseOver()
    {
        //Left Click
        if (Input.GetMouseButtonDown(0))
        {
            OnClick(true);
        }
        //Right Click
        if (Input.GetMouseButtonDown(1))
        {
            OnClick(false);
        }
    }
}
