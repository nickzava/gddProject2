using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressBar : MonoBehaviour
{
    private static LevelProgressBar instance;
    private float percentage = 0;
    private float time = 0;
    private Image bar;
    private Material material;

    private void Awake()
    {
        if(instance)
        {
            Destroy(transform.root);
        }
        else
        {
            instance = this;
            bar = transform.Find("Bar").GetComponent<Image>();
            material = bar.material;
        }
    }

    private void Update()
    {
        time += Time.deltaTime;
        material.SetFloat("_CustomTime", time);
        Debug.Log(material.GetFloat("_CustomTime"));
    }

    public static float Percentage
    {
        get { return instance.percentage; }
        set
        {
            instance.percentage = value;
            instance.bar.fillAmount = value;
        }
    }
}
