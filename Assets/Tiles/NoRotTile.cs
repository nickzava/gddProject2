using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;



//No rotation tile -> no rotation on click
public class NoRotTile : Tile
{
    private static Sprite[] noRotImages;

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
