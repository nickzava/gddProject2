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

        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        gameObject.SetActive(true);
    }

    public void DisabeTable()
    {
        gameObject.SetActive(false);
    }
}
