using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;



//visual representation of a PathNode
public class NoRotTile : Tile
{
    // No rotate tile -> no functionality on click
    void OnClick(bool isLeftClick)
    {
        Debug.Log("Tried to rotate no-rotate tile");
    }

    // Still has input in case we want visual feedback for the no rotate tile
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


	//changes color of noRot tiles (commented out because it's hard to see the tint)
	//public override void SetPower(int power)
	//{
	//	base.SetPower(power);
	//	spriteRenderer.color = new Color(132, 132, 255, 255);
	//}
}
