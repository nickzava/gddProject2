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
    private static Dictionary<int,Sprite[]> fluids;

    //grid location of this tile
    public int gridX = 0;
    public int gridY = 0;

    protected SpriteRenderer baseRenderer;
    protected SpriteRenderer fluidRenderer;
    public TileTypes mType;

    // Bool to track if tile is currently rotating
    protected bool isRotating;
    // Is true if user double clicks on a tile
    protected bool queuedRotate;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        baseRenderer = gameObject.GetComponent<SpriteRenderer>();
        fluidRenderer = transform.Find("Fluid").GetComponent<SpriteRenderer>();
        if(fluids == null)
        {
            const int MAX_ID = 3;
            fluids = new Dictionary<int, Sprite[]>();
            for(int i = 1; i <= MAX_ID; i++)
            {
                fluids.Add(i, Resources.LoadAll<Sprite>("Fluids/" + i.ToString()));
            }
        }
    }

    public virtual void Init(int x, int y, TileTypes type)
    {
        gridX = x;
        gridY = y;
        mType = type;
        NodeManager.Instance.AddMethodToNodeEvent(x, y, SetPath);
        SetPath(0);
    }

    public virtual void SetPath(int id)
    {
        bool rendering = id != 0;
        fluidRenderer.enabled = rendering;
        if(rendering)
            fluidRenderer.sprite = GetSprite(mType,fluids[id]);
    }

    protected Sprite GetSprite(TileTypes type, in Sprite[] sprites)
    {
        string name ="";
        switch (type)
        {
            case TileTypes.L:
                name = "l";
                break;
            case TileTypes.I:
                name = "i";
                break;
            case TileTypes.T:
                name = "t";
                break;
            case TileTypes.X:
                name = "x";
                break;
        }
        for(int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i].name == name)
                return sprites[i];
        }
        return null;
    }

    //rotation coroutine
    public IEnumerator RotateOverTime(bool isClockwise, float seconds, int angle = 90)
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

		//update backend
		for (int i = 0; i < (angle / 90); i++)
		{
			NodeManager.Instance.RotatePathNode(gridX, gridY, isClockwise);
		}

        isRotating = false;
        if (queuedRotate)
        {
            queuedRotate = false;
            StartCoroutine(RotateOverTime(isClockwise, seconds));
        }

        yield break;
    }
}
