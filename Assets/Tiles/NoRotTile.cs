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
        if (!isRotating)
        {
            StartCoroutine(NoRotFeedback(isLeftClick, .05f));
        }
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

    // Gives user a bit of feedback for clicking on a no rotate tile by tilting it slightly
    public IEnumerator NoRotFeedback(bool isClockwise, float seconds, bool returning = false, int angle = 10)
    {
        isRotating = true;

        float secondsElapsed = 0;
        Quaternion initalRotation = transform.rotation;
        Quaternion rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, angle * (isClockwise ? 1 : -1)));

        while (secondsElapsed < seconds)
        {
            secondsElapsed += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(initalRotation, rotation, secondsElapsed / seconds);
            yield return null;
        }

        //ensure that the deired rotation is exactly reached
        transform.rotation = rotation;

        if(!returning)
        {
            StartCoroutine(NoRotFeedback(!isClockwise, seconds, true, angle - (angle * 2)));
        }

        yield break;
    }
}
