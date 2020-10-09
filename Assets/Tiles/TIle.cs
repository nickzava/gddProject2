using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum TileTypes
{
    L, I, T, X
}

public enum FluidTypes
{
    Base1, Base2, Combined
}

//visual representation of a PathNode
public class Tile : MonoBehaviour
{
    [Header("Images")]
    [SerializeField]
    private List<Sprite> LImages;
    [SerializeField]
    private List<Sprite> TImages;
    [SerializeField]
    private List<Sprite> IImages;
    [SerializeField]
    private List<Sprite> XImages;

    //grid location of this tile
    public int gridX = 0;
    public int gridY = 0;

    SpriteRenderer spriteRenderer;
    public TileTypes mType;

    // Bool to track if tile is currently rotating
    private bool isRotating;
    // Is true if user double clicks on a tile
    private bool queuedRotate;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    //rotation coroutine
    IEnumerator RotateOverTime(bool isClockwise, float seconds)
    {
        isRotating = true;

        float secondsElapsed = 0;
        Quaternion initalRotation = transform.rotation;
        Quaternion rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, 90 * (isClockwise ? 1 : -1)));

        while (secondsElapsed < seconds)
        {
            secondsElapsed += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(initalRotation, rotation, secondsElapsed / seconds);
            yield return null;
        }

        //ensure that the deired rotation is exactly reached
        transform.rotation = rotation;

        //update backend
        NodeManager.Instance.RotatePathNode(gridX, gridY, true);
        isRotating = false;
        if (queuedRotate)
        {
            queuedRotate = false;
            StartCoroutine(RotateOverTime(isClockwise, seconds));
        }
        yield break;
    }

    public void SetPower(int power)
    {
        power = power >= LImages.Count ? LImages.Count - 1 : power;
        switch (mType)
        {
            case TileTypes.L:
                spriteRenderer.sprite = LImages[power];
                break;
            case TileTypes.T:
                spriteRenderer.sprite = TImages[power];
                break;
            case TileTypes.I:
                spriteRenderer.sprite = IImages[power];
                break;
            case TileTypes.X:
                spriteRenderer.sprite = XImages[power];
                break;
        }
    }

    //rotates the tile based on the type of click
    //right click -> clockwise rotation
    //left click -> counterclockwise rotation
    // Will not activate if the rotate coroutine is currently active
    void OnClick(bool isLeftClick)
    {
        if (!isRotating)
        {
            StartCoroutine(RotateOverTime(!isLeftClick, 0.15f));
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
