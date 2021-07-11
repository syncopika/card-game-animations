using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScript : MonoBehaviour
{
    //private Transform card;
    private string cardName = "card";

    private Vector3 endPosition;
    private Vector3 startPosition;

    private float speed = 9.0f;
    private float arcHeight = -0.7f;
    private float xRotationAngle = 0.0f; // in radians

    private bool isMoving = false;
    private bool isRotating = false;
    private bool doFlip = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            // since the cards start at different heights but they end up at the same height, we want a bit of an arc?
            // https://luminaryapps.com/blog/arcing-projectiles-in-unity/
            if (transform.position.x < endPosition.x)
            {
                float xDistance = Math.Abs(endPosition.x - startPosition.x);
                float yDistance = Math.Abs(endPosition.y - startPosition.y);
                float nextX = Mathf.MoveTowards(transform.position.x, endPosition.x, speed * Time.deltaTime);
                float nextY = Mathf.MoveTowards(transform.position.y, endPosition.y, speed * Time.deltaTime * (yDistance / xDistance));
                float baseZ = Mathf.Lerp(transform.position.z, endPosition.z, (nextX - startPosition.x) / xDistance);
                float arc = arcHeight * (nextX - startPosition.x) * (nextX - endPosition.x) / (-0.25f * xDistance * xDistance);
                transform.position = new Vector3(nextX, nextY, baseZ + arc);
            }
            else
            {
                transform.position = new Vector3(endPosition.x, endPosition.y, endPosition.z);
                isMoving = false;
            }
        }

        if (isRotating && doFlip)
        {
            if (transform.eulerAngles.y < 180)
            {
                // flip the card across the y-axis (so it goes from face-up to face-down or vice-versa)
                // we start at eulerAngles.y being 0 deg
                float angle = (2*Mathf.PI) / (endPosition.x - startPosition.x); // each card has a different rate of rotation when being flipped based on distance from the deck

                // by rotating about the x-axis when rotating about the y-axis we can affect the z-axis to get my desired effect. discovered this accidentally lol
                // this might be helpful in understanding why: https://www.reddit.com/r/Unity3D/comments/k03d4s/why_transformrotatex_y_0_also_rotates_zaxis/
                transform.Rotate(xRotationAngle, angle * speed, 0);
            }
            else
            {
                // keep this rotation but ensure card is fully flipped across y-axis
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180, transform.eulerAngles.z);
                isRotating = false;
            }
        }

        if(isRotating && !doFlip)
        {
            float deg = (180 * xRotationAngle) / (float)Math.PI;

            // kinda weird but this time since we're not rotating about y, we can just rotate about z.
            if (isMoving)
            {
                transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, new Vector3(0, 0, deg), Time.deltaTime);
            }
            else
            {
                isRotating = false;
            }
        }

    }

    public void setStartPosition(Vector3 start)
    {
        transform.position = start;
        startPosition = new Vector3(start.x, start.y, start.z);
    }

    public void setEndPosition(Vector3 end)
    {
        endPosition = end;
    }

    public void setXRotationAngle(float angle)
    {
        xRotationAngle = angle;
    }

    public void toggleFlip(bool flip)
    {
        doFlip = flip;
    }

    public void setName(string name)
    {
        cardName = name;
    }

    public void placeCard()
    {
        isMoving = true;
        isRotating = true;
    }

    public void flipCard()
    {
        // TODO
    }
}
