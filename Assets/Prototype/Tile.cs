using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    Sprite XOff;
    [SerializeField]
    Sprite XOn;
    [SerializeField]
    Sprite IOff;
    [SerializeField]
    Sprite IOn;
    [SerializeField]
    Sprite TOff;
    [SerializeField]
    Sprite TOn;
    [SerializeField]
    Sprite LOff;
    [SerializeField]
    Sprite LOn;

    [SerializeField]
    SpriteRenderer spriteRenderer;
    

    public char tileType;
    public bool power;
    // Connections is always a 4-int binary array that starts from the north side and goes clockwise
    public int[] connections;

    // Start is called before the first frame update
    void Start()
    {
        // Fills out the connection array based on tile type and changes the sprite
        switch (tileType)
        {
            case 'x':
                connections = new int[] { 1, 1, 1, 1 };
                spriteRenderer.sprite = XOff;
                break;
            case 'i':
                connections = new int[] { 1, 0, 1, 0 };
                spriteRenderer.sprite = IOff;
                break;
            case 't':
                connections = new int[] { 0, 1, 1, 1 };
                spriteRenderer.sprite = TOff;
                break;
            case 'l':
                connections = new int[] { 0, 0, 1, 1 };
                spriteRenderer.sprite = LOff;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {


        // Checks if powered, then changes sprite accordingly
        switch(tileType)
        {
            case 'x':
                if (power)
                    spriteRenderer.sprite = XOn;
                else
                    spriteRenderer.sprite = XOff;
                break;
            case 'i':
                if (power)
                    spriteRenderer.sprite = IOn;
                else
                    spriteRenderer.sprite = IOff;
                break;
            case 't':
                if (power)
                    spriteRenderer.sprite = TOn;
                else
                    spriteRenderer.sprite = TOff;
                break;
            case 'l':
                if (power)
                    spriteRenderer.sprite = LOn;
                else
                    spriteRenderer.sprite = LOff;
                break;
        }
    }

    private void OnMouseDown()
    {
        Rotate();
    }

    // Rotate function changes the orientation and modifies the connection array to reflect the new orientation
    void Rotate()
    {
        gameObject.transform.rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, -90f));

        int[] tempArray = new int[4];
        // Values of temparray become the rotated form of the original array
        tempArray[0] = connections[3];
        tempArray[1] = connections[0];
        tempArray[2] = connections[1];
        tempArray[3] = connections[2];

        connections = tempArray;
    }


}
