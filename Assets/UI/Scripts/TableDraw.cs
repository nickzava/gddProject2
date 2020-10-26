using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableDraw : MonoBehaviour
{
    private float tileDimension = 1;
    private void Awake()
    {
        // Dont enable, buttons take care of this
        //gameObject.SetActive(false);
    }

    public void DrawTable(int _width, int _height)
    {
        // Sets the size
        float width = (_width * tileDimension) + tileDimension;
        float height = (_height * tileDimension) + tileDimension;

        // Sets the table to the correct size, then moves it to the center of the board
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        gameObject.GetComponent<RectTransform>().transform.position = new Vector3(0, 0, 0);
        if (height >= 10)
        {
            // Makes it so 10x10 tables work
            gameObject.GetComponent<RectTransform>().transform.position = new Vector3(-0.5f, 0, 0);
        }
        gameObject.SetActive(true);

        
    }

    public void DisabeTable()
    {
        gameObject.SetActive(false);
    }
}
