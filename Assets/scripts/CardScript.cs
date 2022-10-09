using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScript : MonoBehaviour
{
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Quaternion endRotation = Quaternion.Euler(0f, 180f, 0f);
    private Quaternion flippedRotation;

    private float speed = 9.0f;
    private float arcHeight = -0.7f;
    private float zRotationAngle = 0.0f; // in radians

    private bool isMoving = false;
    private bool flipOnce = false; // for a one-time flip triggered by clicking on this card
    private bool doFlip = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isMoving)
        {
            // since the cards start at different heights but they end up at the same height (and at a height potentially lower than the original starting height), we want a bit of an arc?
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

                // by rotating about the x-axis when rotating about the y-axis we can affect the z-axis to get my desired effect. discovered this accidentally lol
                // this might be helpful in understanding why: https://www.reddit.com/r/Unity3D/comments/k03d4s/why_transformrotatex_y_0_also_rotates_zaxis/
                // but better to not mess with each axis individually and try quaternions instead
                // https://forum.unity.com/threads/rotate-object-towards-objects-at-different-distances.732425/ the quaternion slerp bit is super helpful
                // https://forum.unity.com/threads/understanding-rotations-in-local-and-world-space-quaternions.153330/

                float xDistCovered = Math.Abs(transform.position.x - startPosition.x);

                // make sure to take into account flipping if needed
                Quaternion endRot = doFlip ? endRotation : Quaternion.Euler(0f, 0f, (zRotationAngle * 180 / Mathf.PI));

                transform.rotation = Quaternion.Slerp(Quaternion.Euler(0f, 0f, 0f), endRot, xDistCovered / xDistance);
            }
            else
            {
                transform.position = new Vector3(endPosition.x, endPosition.y, endPosition.z);
                isMoving = false;
            }
        }

        if (flipOnce)
        {
            // flip the card on click
            transform.rotation = Quaternion.Lerp(transform.rotation, flippedRotation, Time.deltaTime * 10f);
        }
    }

    public void setStartPosition(Vector3 start)
    {
        transform.position = start;

        // make a separate vector3 for the position so we can remember what the original start position was
        // and not overwrite its values
        startPosition = new Vector3(start.x, start.y, start.z);
    }

    public Vector3 getPosition()
    {
        return transform.position;
    }

    public void setEndPosition(Vector3 end)
    {
        endPosition = end;
    }

    // angle is in radians here
    public void setZRotationAngle(float angle)
    {
        zRotationAngle = angle;

        // if flip
        // roate about y, then rotate about z - have to do the operations backwards
        endRotation = transform.rotation * Quaternion.Euler(0f, 0f, (angle * 180) / Mathf.PI) * Quaternion.Euler(0f, 180f, 0f);
    }

    public void toggleFlip(bool flip)
    {
        doFlip = flip;
    }

    public string Name { get; set; } = "card";

    public void placeCard()
    {
        isMoving = true;
    }

    public void doFlipOnce()
    {
        flipOnce = true;

        flippedRotation = transform.rotation * Quaternion.Euler(0, 180, 0);
    }

    public bool isStatic()
    {
        return !isMoving;
    }
}
