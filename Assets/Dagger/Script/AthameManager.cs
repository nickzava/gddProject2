using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AthameManager : MonoBehaviour
{
    //Scale up dagger to make it look like it's futher from table
    private float daggerScale = 1;
    //Theoretical height from board, only adjust Y in game
    private float daggerHeight = 0;

    public bool isRaised = false;
    private bool previousIsRaised = false;
    private bool raiseDagger = false;
    //directions: 
    //0 - Left
    //1 - Up
    //2 - Right
    //3 - Down
    private int direction = 0;

    public GameObject dagger;
    private Transform daggerTransform;
    public GameObject shadow;
    private Transform shadowTransform;


    // Start is called before the first frame update
    void Start()
    {
        daggerTransform = dagger.GetComponent<Transform>().transform;
        shadowTransform = shadow.GetComponent<Transform>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(isRaised != previousIsRaised)
        {
            raiseDagger = true;
            daggerHeight = daggerTransform.position.y;
        }

        if (raiseDagger == true)
        {
            if (!isRaised)
            {
                daggerTransform.position = new Vector3(daggerTransform.position.x, daggerTransform.position.y - .0025f, daggerTransform.position.z);
                shadowTransform.position = new Vector3(shadowTransform.position.x + .0025f, shadowTransform.position.y, shadowTransform.position.z);
            }
            else
            {
                daggerTransform.position = new Vector3(daggerTransform.position.x, daggerTransform.position.y + .0025f, daggerTransform.position.z);
                shadowTransform.position = new Vector3(shadowTransform.position.x - .0025f, shadowTransform.position.y, shadowTransform.position.z);
            }
            Debug.Log(Mathf.Abs(daggerTransform.position.y - daggerHeight));
            if(Mathf.Abs(daggerTransform.position.y - daggerHeight) >= 2)
            {
                raiseDagger = false;
            }
        }

        previousIsRaised = isRaised;

        if (Input.GetKey("w"))
        {
            daggerTransform.position = new Vector3(daggerTransform.position.x, daggerTransform.position.y + .005f, daggerTransform.position.z);
            shadowTransform.position = new Vector3(shadowTransform.position.x, shadowTransform.position.y + .005f, shadowTransform.position.z);
        }
        if (Input.GetKey("s"))
        {
            daggerTransform.position = new Vector3(daggerTransform.position.x, daggerTransform.position.y - .005f, daggerTransform.position.z);
            shadowTransform.position = new Vector3(shadowTransform.position.x, shadowTransform.position.y - .005f, shadowTransform.position.z);
        }
        if (Input.GetKey("a"))
        {
            daggerTransform.position = new Vector3(daggerTransform.position.x - .005f, daggerTransform.position.y, daggerTransform.position.z);
            shadowTransform.position = new Vector3(shadowTransform.position.x - .005f, shadowTransform.position.y, shadowTransform.position.z);
        }
        if (Input.GetKey("d"))
        {
            daggerTransform.position = new Vector3(daggerTransform.position.x + .005f, daggerTransform.position.y, daggerTransform.position.z);
            shadowTransform.position = new Vector3(shadowTransform.position.x + .005f, shadowTransform.position.y, shadowTransform.position.z);
        }
    }
}
