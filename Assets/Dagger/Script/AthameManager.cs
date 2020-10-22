using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private GameObject daggerObj;
    private ScreenShake cameraShake;
    private Transform daggerTransform;

    public GameObject shadow;
    private GameObject shadowObj;
    private Transform shadowTransform;

    public Sprite leftDagger;
    public Sprite rightDagger;
    public Sprite upDagger;
    public Sprite downDagger;

    private bool daggerIsMoving = false;

    private LinkedList<PathNode> longestPath;

    //used to accelerate the dagger as time goes on
    private float deltaMult;
    private float deltaMultAcceleration = 1;

    private float deltaX;
    private float deltaY;
    private Vector3 targetPosition = new Vector3(0, 0, 0);
    private int signOfVector;
    private int currentCount = 1;

    // Start is called before the first frame update
    void Start()
    {
        daggerObj = Instantiate(dagger, new Vector3(0, 0, -1), Quaternion.identity);
        shadowObj = Instantiate(shadow, new Vector3(0, 0, -1), Quaternion.identity);
        daggerTransform = daggerObj.GetComponent<Transform>().transform;
        shadowTransform = shadowObj.GetComponent<Transform>().transform;
        shadowTransform.Rotate(0, 0, 90);
        daggerObj.GetComponent<SpriteRenderer>().enabled = false;
        shadowObj.GetComponent<SpriteRenderer>().enabled = false;
        cameraShake = Camera.main.gameObject.GetComponent<ScreenShake>();
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
            if(Mathf.Abs(daggerTransform.position.y - daggerHeight) >= 2)
            {
                raiseDagger = false;
            }
        }

        previousIsRaised = isRaised;

        if(false /*when the dagger impacts with the board*/)
        {
            cameraShake.ShakeScreen(.25f, 10, .2f);
        }


        if (daggerIsMoving == true) {
            deltaMult += deltaMultAcceleration * Time.deltaTime;

            MoveDagger();

            Vector3 toTarget = targetPosition - daggerTransform.position;
            int sign = (int)(Mathf.Sign(toTarget.x) * Mathf.Sign(toTarget.y));
            if (sign != signOfVector)
            {
                daggerTransform.position = targetPosition;
                if (currentCount < longestPath.Count - 1)
                {
                    currentCount++;
                    SetDaggerDelta(longestPath.ElementAt(currentCount));
                }
                else
                {
                    daggerIsMoving = false;
                }
            }
        }
    }


    public void StartDaggerMovement()
    {
        daggerIsMoving = true;
        daggerObj.GetComponent<SpriteRenderer>().enabled = true;
        shadowObj.GetComponent<SpriteRenderer>().enabled = true;
        deltaMult = 1;

        //get longest path
        longestPath = null;
        foreach(Path p in NodeManager.Instance.paths)
        {
            var pathToCheck = p.GetLongestSequenceInPath();
            if (longestPath == null || pathToCheck.Count > longestPath.Count)
                longestPath = pathToCheck;
        }

        daggerObj.GetComponent<Transform>().position = TileManager.Instance.GetTileFromNode(longestPath.ElementAt(0)).GetComponent<Transform>().position + new Vector3(0, 0, -1);
        shadowObj.GetComponent<Transform>().position = TileManager.Instance.GetTileFromNode(longestPath.ElementAt(0)).GetComponent<Transform>().position + new Vector3(0, 0, -1);
        SetDaggerDelta(longestPath.ElementAt(1));
    }

    void SetDaggerDelta(PathNode target)
    {
        targetPosition = TileManager.Instance.GetTileFromNode(target).GetComponent<Transform>().position;
        Vector3 toTarget = targetPosition - daggerTransform.position;
        signOfVector = (int)(Mathf.Sign(toTarget.x) * Mathf.Sign(toTarget.y));

        deltaY = deltaMult * (targetPosition.y - daggerObj.GetComponent<Transform>().position.y) / 500;
        deltaX = deltaMult * (targetPosition.x - daggerObj.GetComponent<Transform>().position.x) / 500;
        float deltaCotan = Convert.ToSingle(Math.Atan2(deltaY, deltaX)/Math.PI);
        Debug.Log(deltaCotan);
        if (deltaCotan <= -.25f && deltaCotan > -.75f) //Down
        {
            daggerObj.GetComponent<SpriteRenderer>().sprite = downDagger;
        }
        if (deltaCotan <= .25f && deltaCotan > -.25f) //Right
        {
            daggerObj.GetComponent<SpriteRenderer>().sprite = rightDagger;
        }
        if(deltaCotan <= .75f && deltaCotan > .25f) //up
        {
            daggerObj.GetComponent<SpriteRenderer>().sprite = upDagger;
        }
        if(deltaCotan <= -.75f || deltaCotan > .75f) //Left
        {
            daggerObj.GetComponent<SpriteRenderer>().sprite = leftDagger;
        }
        

    }

    void MoveDagger()
    {
        daggerTransform.position = new Vector3(daggerTransform.position.x + deltaX, daggerTransform.position.y + deltaY, daggerTransform.position.z);
        shadowTransform.position = new Vector3(shadowTransform.position.x + deltaX, shadowTransform.position.y + deltaY, shadowTransform.position.z);
    }

    PathNode NextDaggerTarget()
    {
        return null;
    }
}

