using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//visual representation of a PathNode
public class Tile : MonoBehaviour
{
    //Hold a refrence to the backend node it represents
    PathNode mNode;

    // Start is called before the first frame update
    void Start()
    {

    }

    //rotation coroutine
    IEnumerator RotateOverTime(Quaternion rotation, float seconds)
    {
        yield break;
    }

    //rotates the tile based on the type of click
    //right click -> clockwise rotation
    //left click -> counterclockwise rotation
    void OnClick(bool isLeftClick)
    {

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
