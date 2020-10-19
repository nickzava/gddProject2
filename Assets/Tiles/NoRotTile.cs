using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;



//visual representation of a PathNode
public class NoRotTile : Tile
{
    private static Sprite[] noRotImages;

    // No rotate tile -> no functionality on click
    void OnClick(bool isLeftClick)
    {

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

    protected override void Awake()
    {
        base.Awake();
        if (noRotImages == null)
        {
            noRotImages = Resources.LoadAll<Sprite>("NoRotSprites");
        }
    }

    public override void Init(int x, int y, TileTypes type)
    {
        base.Init(x, y, type);
        baseRenderer.sprite = GetSprite(type, noRotImages);
    }

    //changes color of noRot tiles (commented out because it's hard to see the tint)
    //public override void SetPower(int power)
    //{
    //	base.SetPower(power);
    //	spriteRenderer.color = new Color(132, 132, 255, 255);
    //}
}
