using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using UnityEngine;

public class AthameManager : MonoBehaviour
{
    //If statement with dagger touching board & leaving board @line 128
    //Scale up dagger to make it look like it's futher from table
    private float daggerScale = 1;
    //Theoretical height from board, only adjust Y in game
    private float daggerHeight = 0;

    private bool isRaised = true;
    private bool previousIsRaised = true;
    private bool changeDaggerHeight = false;
    private bool finishedDaggerBoard = false;
    private bool touchingBoard = false;
    private bool previousTouchingBoard = false;

    //directions: 
    //0 - Left
    //1 - Up
    //2 - Right
    //3 - Down
    private int direction = 0;

    public GameObject dagger;
    private GameObject daggerObj;
    private Transform daggerTransform;

    public GameObject shadow;
    private GameObject shadowObj;
    private Transform shadowTransform;

    private ScreenShake cameraShake;

    public Sprite leftDagger;
    public Sprite rightDagger;
    public Sprite upDagger;
    public Sprite downDagger;

    private bool daggerIsMoving = false;
	private bool soundIsPlaying = false;

    private LinkedList<PathNode> longestPath;

    //used to accelerate the dagger as time goes on
    private float deltaMult;
    private float deltaMultAcceleration = 1;

    private float deltaX = 0;
    private float deltaY = 0;
    private Vector3 targetPosition = new Vector3(0, 0, 0);
    private int signOfVector;
    private int currentCount = 1;

    // Start is called before the first frame update
    void Start()
    {
        daggerObj = Instantiate(dagger, new Vector3(-1, 0, -1), Quaternion.identity);
        shadowObj = Instantiate(shadow, new Vector3(-1, 0, -1), Quaternion.identity);
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
        //Check if isRaised is changed
        if (isRaised != previousIsRaised)
        {
            changeDaggerHeight = true;
            daggerHeight = daggerTransform.position.y;
        }
        //Raise/Lower dagger when changeDaggerHeight = true
        //Runs if isRaised is changed
        if (changeDaggerHeight == true)
        {
            if (isRaised)
            {
                RaiseDagger();
                touchingBoard = false;
                //Moment athame leaves board
                daggerObj.GetComponent<SpriteRenderer>().sprite = rightDagger;
            }
            else
            {
                LowerDagger();
            }
            //check if dagger traveled 2 units
            if(Mathf.Abs(daggerTransform.position.y - daggerHeight) >= 2)
            {
                changeDaggerHeight = false;
                
                if (finishedDaggerBoard) //If at end of path
                {
                    Debug.Log("finished");
                    daggerObj.GetComponent<SpriteRenderer>().enabled = false;
                    shadowObj.GetComponent<SpriteRenderer>().enabled = false;
                }
                else //If at start of path
                {
                    Debug.Log("start");
                    daggerIsMoving = true;
                    SetDaggerDelta(longestPath.ElementAt(currentCount));
                }
                if (!isRaised)
                {
                    //Moment dagger touches board
                    touchingBoard = true;
                }
                else
                {
                    touchingBoard = false;
                }
            }
        }

        previousIsRaised = isRaised;

        //moment touches board
        if (touchingBoard != previousTouchingBoard && touchingBoard) {
            cameraShake.ShakeScreen(.25f, 10, .2f);
            daggerObj.transform.Find("RockChunkEmmiter").gameObject.SetActive(true);
        } 
        else if (touchingBoard != previousTouchingBoard && !touchingBoard) { //Moment leaves board
            daggerObj.transform.Find("RockChunkEmmiter").gameObject.SetActive(false);
        }

        //Check if dagger should move
        if (daggerIsMoving == true) {
            deltaMult += deltaMultAcceleration * Time.deltaTime;

            

            Vector3 toTarget = targetPosition - daggerTransform.position;
            int sign = (int)(Mathf.Sign(toTarget.x) * Mathf.Sign(toTarget.y));
            if (sign != signOfVector)
            {
                daggerTransform.position = targetPosition;
                if (longestPath != null && currentCount < longestPath.Count - 1)
                {
                    currentCount++;
                    SetDaggerDelta(longestPath.ElementAt(currentCount));
                }
                else
                {
                    daggerIsMoving = false;
                    finishedDaggerBoard = true;
                    isRaised = true;
                }
            }
            MoveDagger();
        }

        daggerTransform = daggerObj.GetComponent<Transform>().transform;
        shadowTransform = shadowObj.GetComponent<Transform>().transform;
        previousTouchingBoard = touchingBoard;

        //athame dragging sound
        if (daggerIsMoving && !soundIsPlaying)	//starts if dagger moving and sound not started yet
		{
			GetComponent<AudioSource>().Play();
			soundIsPlaying = true;
		}
		else if (!daggerIsMoving && soundIsPlaying)		//stops if dagger has stopped and sound still playing
		{
			GetComponent<AudioSource>().Stop();
			soundIsPlaying = false;
		}
    }


    public void StartDaggerMovement()
    {
        isRaised = false;
        finishedDaggerBoard = false;
        daggerObj.GetComponent<SpriteRenderer>().enabled = true;
        shadowObj.GetComponent<SpriteRenderer>().enabled = true;
        deltaMult = 1;
        currentCount = 1;

        //get longest path
        longestPath = null;
        foreach (Path p in NodeManager.Instance.paths)
        {
            var pathToCheck = p.GetLongestSequenceInPath();
            if (longestPath == null || pathToCheck.Count > longestPath.Count)
                longestPath = pathToCheck;
        }
        if (longestPath == null)
        {
            Debug.LogError("Got Null path from node manager!");
        }
        daggerObj.GetComponent<Transform>().position = TileManager.Instance.GetTileFromNode(longestPath.ElementAt(0)).GetComponent<Transform>().position + new Vector3(0, 2, -1);
        shadowObj.GetComponent<Transform>().position = TileManager.Instance.GetTileFromNode(longestPath.ElementAt(0)).GetComponent<Transform>().position + new Vector3(-2, 0, -1);

    }

    public void CancelDaggerMovement()
    {
        daggerObj.GetComponent<SpriteRenderer>().enabled = false;
        shadowObj.GetComponent<SpriteRenderer>().enabled = false;
        deltaX = 0;
        deltaY = 0;
        daggerIsMoving = false;
        changeDaggerHeight = false;
        isRaised = true;
        previousIsRaised = true;
}

    void SetDaggerDelta(PathNode target)
    {
        targetPosition = TileManager.Instance.GetTileFromNode(target).GetComponent<Transform>().position;
        Vector3 toTarget = targetPosition - daggerTransform.position;
        signOfVector = (int)(Mathf.Sign(toTarget.x) * Mathf.Sign(toTarget.y));

        deltaY = deltaMult * (targetPosition.y - daggerObj.GetComponent<Transform>().position.y) / 500;
        deltaX = deltaMult * (targetPosition.x - daggerObj.GetComponent<Transform>().position.x) / 500;

        float deltaCotan = Convert.ToSingle(Math.Atan2(deltaY, deltaX)/Math.PI);
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

    private float VerticalDaggerDelta()
    {
        float delta = 0;

        return delta;
    }

    private void RaiseDagger()
    {
        daggerTransform.position = new Vector3(daggerTransform.position.x, daggerTransform.position.y + .0025f, daggerTransform.position.z);
        shadowTransform.position = new Vector3(shadowTransform.position.x - .0025f, shadowTransform.position.y, shadowTransform.position.z);
    }

    private void LowerDagger()
    {
        daggerTransform.position = new Vector3(daggerTransform.position.x, daggerTransform.position.y - .0025f, daggerTransform.position.z);
        shadowTransform.position = new Vector3(shadowTransform.position.x + .0025f, shadowTransform.position.y, shadowTransform.position.z);
    }

    void MoveDagger()
    {
        daggerTransform.position = new Vector3(daggerTransform.position.x + deltaX, daggerTransform.position.y + deltaY, daggerTransform.position.z);
        shadowTransform.position = new Vector3(shadowTransform.position.x + deltaX, shadowTransform.position.y + deltaY, shadowTransform.position.z);
        //Debug.Log(deltaX + ", " + deltaY + " + " + deltaMult);
    }

    public void ClearPath()
    {
        longestPath = null;
        daggerIsMoving = false;
    }
}

