using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileTypes
{
    L, I, T, X
}

public enum FluidTypes
{
    Base1, Base2, Combined
}

public abstract class Tile : MonoBehaviour
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

    protected SpriteRenderer spriteRenderer;
    public TileTypes mType;

    // Bool to track if tile is currently rotating
    protected bool isRotating;
    // Is true if user double clicks on a tile
    protected bool queuedRotate;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void Init(int x, int y, TileTypes type)
    {
        gridX = x;
        gridY = y;
        mType = type;
        NodeManager.Instance.AddMethodToNodeEvent(x, y, SetPower);
        SetPower(0);
    }

    public void SetPower(int power)
    {
        //if there is no image then use the last image in the array
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

    //rotation coroutine
    protected IEnumerator RotateOverTime(bool isClockwise, float seconds)
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
        NodeManager.Instance.RotatePathNode(gridX, gridY, isClockwise);

        isRotating = false;
        if (queuedRotate)
        {
            queuedRotate = false;
            StartCoroutine(RotateOverTime(isClockwise, seconds));
        }
        yield break;
    }
}
