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

	public void OnClick()
	{
		GetComponent<AudioSource>().clip = GetComponent<SoundHolder>().chainSound;
		GetComponent<AudioSource>().Play();
		//PlayOneShot(GetComponent<SoundHolder>().chainSound, 1.0f);
	}

	private void OnMouseOver()
	{
		//Left Click
		if (Input.GetMouseButtonDown(0))
		{
			OnClick();
		}
		//Right Click
		if (Input.GetMouseButtonDown(1))
		{
			OnClick();
		}
	}
}
