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

    private static Sprite[] baseImages;
	//public AudioClip soundEffect;
	//public AudioClip chainSound;

	protected override void Awake()
    {
        base.Awake();
        if (baseImages == null)
        {
            baseImages = Resources.LoadAll<Sprite>("BaseSprites");
        }
		//soundEffect = GetComponent<AudioSource>();
	}

    public override void Init(int x, int y, TileTypes type)
    {
        base.Init(x, y, type);
        baseRenderer.sprite = GetSprite(type,baseImages);
    }

    public void OnClick(bool isLeftClick)
    {
        if (!isRotating)
        {
            StartCoroutine(RotateOverTime(isLeftClick, 0.15f));
        }
        else
        {
            queuedRotate = true;
        }
		GetComponent<AudioSource>().clip = GetComponent<SoundHolder>().soundEffect;
		GetComponent<AudioSource>().Play();
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
